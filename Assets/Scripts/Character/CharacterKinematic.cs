// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : CharacterKinematic.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.Performance;
using UnityEngine;

public class CharacterKinematic : CharacterComponent, IMultiCooldownOwner, IManagedRigidbody
{
    [Header("Channels")]
    [SerializeField, Guarded] private AnimationChannelEvent animationChannelEvent;
    [SerializeField, Guarded] private InputChannelEvent inputChannel;
    [Header("Physics")]
    [SerializeField] private float movementSpeed = 5F;
    [Range(0F, 1F)] [SerializeField] private float dampening = 0.1F;
    [Header("Dodge")]
    [SerializeField] private float dodgeForce = 1400F;
    [SerializeField] private float dodgeDuration = 0.3F;
    [SerializeField] private int dodgesCount = 3;
    [SerializeField] private float dodgeCooldown = 4F;
    [SerializeField] private int invulnerabilityTimeSteps = 10;
    private Vector2 traslation;
    private float dodgeTimer = 0F;
    private int invulnerabilityCurrentSteps;
    private UpdateJob rechargeDodgesJob;
    private int dodgesCharges = 0;
    private Vector3 externVelocity = Vector2.zero;

    private bool IsDodging => dodgeTimer > 0F;

    public override void CommonFixedUpdate(float fixedDeltaTime)
    {
        externVelocity = Vector2.Lerp(externVelocity, Vector2.zero, dampening);
        Rigidbody.velocity = externVelocity;
        if (!IsDodging)
        {
            Rigidbody.velocity += traslation;
            invulnerabilityCurrentSteps = 0;
        }
        else
        {
            if (invulnerabilityCurrentSteps > 0)
            {
                invulnerabilityCurrentSteps--;
            }
            else if (invulnerabilityCurrentSteps == 0)
            {
                EventChannel.BroadcastInvulnerability(false);
                invulnerabilityCurrentSteps = -1;
            }
        }
    }

    public void Move(Vector2 direction, float speedMultiplier = 1F)
    {
        traslation = movementSpeed * speedMultiplier * direction.normalized;
    }

    public override void CommonUpdate(float deltaTime)
    {
        if (IsDodging)
        {
            dodgeTimer -= deltaTime;
            if (dodgeTimer <= 0)
            {
                dodgeTimer = 0F;
                EventChannel.BroadcastInvulnerability(false);
            }
        }

        rechargeDodgesJob.Step(deltaTime);
        if (dodgesCharges >= dodgesCount) rechargeDodgesJob.Suspend();

        if (Input.GetKeyDown(KeyCode.D))
        {
            //externVelocity += new Vector2(transform.right.x, transform.right.y) * 20F;
            // Rigidbody.AddForce(transform.right * 2000F, ForceMode2D.Impulse);
            inputChannel.Broadcast(new Inputs.DirectionInputData(Inputs.InputType.DODGE, transform.right));
        }
    }

    public int GetResourcesAmount() => dodgesCharges;

    public float GetCooldown() => dodgeCooldown;

    public float GetCooldownPercentage()
    {
        if (dodgesCharges >= dodgesCount)
        {
            return 0;
        }
        return 1F - rechargeDodgesJob.GetUpdateProgress();
    }

    public Sprite GetIcon() => null;

    public void AddExternalForce(Vector3 force)
    {
        externVelocity += force;
    }

    protected override void Awake()
    {
        base.Awake();
        rechargeDodgesJob = new UpdateJob(new Callback(RechargeDodge), dodgeCooldown);
        dodgesCharges = dodgesCount;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        inputChannel.OnEvent += OnInput;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        inputChannel.OnEvent -= OnInput;
    }

    private void RechargeDodge()
    {
        if (dodgesCharges < dodgesCount)
        {
            dodgesCharges++;
        }
    }

    private void OnInput(Inputs.InputData data)
    {
        switch (data.Type)
        {
            case Inputs.InputType.MOVE:
                Inputs.DirectionInputData directionData = data as Inputs.DirectionInputData;
                Move(directionData.Direction);
                if (Mathf.Approximately(directionData.Direction.magnitude, 0F))
                {
                    animationChannelEvent.Broadcast(new AnimationChannelEvent.AnimationData(AnimatorDriver.IDLE, null));
                }
                else
                {
                    animationChannelEvent.Broadcast(new AnimationChannelEvent.AnimationData(AnimatorDriver.RUN, directionData.Direction.magnitude));
                }
                break;

            case Inputs.InputType.DODGE:
                if (IsDodging || dodgesCharges < 1) return;
                Inputs.DirectionInputData dodgeDir = data as Inputs.DirectionInputData;
                AddExternalForce(dodgeDir.Direction * dodgeForce);
                //Rigidbody.AddForce(dodgeDir.Direction * dodgeForce, ForceMode2D.Impulse);
                dodgeTimer = dodgeDuration;
                invulnerabilityCurrentSteps = invulnerabilityTimeSteps;
                EventChannel.BroadcastInvulnerability(true);
                dodgesCharges--;
                if (!rechargeDodgesJob.Active)
                {
                    rechargeDodgesJob.Resume();
                }
                break;
        }
    }
}