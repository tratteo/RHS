// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : CharacterKinematic.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.Performance;
using System;
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
    [Header("References")]
    [SerializeField] private ParticleSystem footstepsSystem;
    private ParticleSystemRenderer footstepsSystemRenderer;
    private Vector2 traslation;
    private float dodgeTimer = 0F;
    private int invulnerabilityCurrentSteps;
    private UpdateJob rechargeDodgesJob;
    private int dodgesCharges = 0;
    private Vector3 externVelocity = Vector2.zero;

    public bool IsInvulnerable => invulnerabilityCurrentSteps > 0;

    public bool IsDodging => dodgeTimer > 0F;

    public event Action<bool> OnInvulnerability;

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
                if (invulnerabilityCurrentSteps <= 0)
                {
                    OnInvulnerability?.Invoke(false);
                    invulnerabilityCurrentSteps = 0;
                }
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
                OnInvulnerability?.Invoke(false);
            }
        }

        rechargeDodgesJob.Step(deltaTime);
        if (dodgesCharges >= dodgesCount) rechargeDodgesJob.Suspend();

        if (Input.GetKeyDown(KeyCode.D))
        {
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

    public float GetCombinedSign(Vector3 vector)
    {
        return Mathf.Sign(transform.localScale.x * vector.x);
    }

    protected override void OnDeath(bool win)
    {
        base.OnDeath(win);
        traslation = Vector2.zero;
        animationChannelEvent.Broadcast(new AnimationChannelEvent.AnimationData(AnimatorDriver.IDLE, null));
    }

    protected override void Awake()
    {
        base.Awake();
        rechargeDodgesJob = new UpdateJob(new Callback(RechargeDodge), dodgeCooldown);
        dodgesCharges = dodgesCount;
        footstepsSystemRenderer = footstepsSystem.GetComponent<ParticleSystemRenderer>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus.OnStunEvent.Invocation += OnStun;
        inputChannel.OnEvent += OnInput;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.OnStunEvent.Invocation -= OnStun;
        inputChannel.OnEvent -= OnInput;
    }

    private void OnStun(bool stun)
    {
        Manager.Kinematic.Move(Vector2.zero);
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
                if (!Active || Manager.Combat.IsStunned)
                {
                    animationChannelEvent.Broadcast(new AnimationChannelEvent.AnimationData(AnimatorDriver.IDLE, null));
                    return;
                }

                Inputs.DirectionInputData directionData = data as Inputs.DirectionInputData;
                Move(directionData.Direction);
                if (Mathf.Approximately(directionData.Direction.magnitude, 0F))
                {
                    animationChannelEvent.Broadcast(new AnimationChannelEvent.AnimationData(AnimatorDriver.IDLE, null));
                }
                else
                {
                    //Debug.Log(GetCombinedSign(directionData.Direction));
                    animationChannelEvent.Broadcast(new AnimationChannelEvent.AnimationData(AnimatorDriver.RUN, GetCombinedSign(directionData.Direction)));

                    if (UnityEngine.Random.value < 0.01F)
                    {
                        footstepsSystem.Play();
                        footstepsSystemRenderer.flip = new Vector3(Rigidbody.velocity.x > 0 ? 1F : -1F, 0F, 0F);
                    }
                }
                break;

            case Inputs.InputType.DODGE:
                if (IsDodging || dodgesCharges < 1) return;
                Inputs.DirectionInputData dodgeDir = data as Inputs.DirectionInputData;
                AddExternalForce(dodgeDir.Direction * dodgeForce);
                dodgeTimer = dodgeDuration;
                invulnerabilityCurrentSteps = invulnerabilityTimeSteps;
                OnInvulnerability?.Invoke(true);
                dodgesCharges--;
                if (!rechargeDodgesJob.Active)
                {
                    rechargeDodgesJob.Resume();
                }
                break;
        }
    }
}