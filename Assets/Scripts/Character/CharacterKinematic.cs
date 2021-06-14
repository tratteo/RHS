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
    [Header("Physics")]
    [SerializeField] private float movementSpeed = 5F;
    [Range(0F, 1F)] [SerializeField] private float forceDampening = 0.1F;
    [Header("Dodge")]
    [SerializeField] private float dodgeForce = 1400F;
    [SerializeField] private float dodgeDuration = 0.3F;
    [SerializeField] private int dodgesCount = 3;
    [SerializeField] private float dodgeCooldown = 4F;
    [SerializeField] private int invulnerabilityTimeSteps = 10;
    [Header("References")]
    [SerializeField] private ParticleSystem footstepsSystem;
    [SerializeField] private ParticleSystem dashSystem;
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
        externVelocity = Vector2.Lerp(externVelocity, Vector2.zero, forceDampening);
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
        else if (Mathf.Approximately(traslation.magnitude, 0F))
        {
            AnimatorDriver.DriveAnimation(new AnimatorDriver.AnimationData(AnimatorDriver.IDLE));
        }
        else
        {
            AnimatorDriver.DriveAnimation(new AnimatorDriver.AnimationData(AnimatorDriver.RUN, false, GetRelativeTraslationSign(traslation)));
        }

        rechargeDodgesJob.Step(deltaTime);
        if (dodgesCharges >= dodgesCount) rechargeDodgesJob.Suspend();

        if (Input.GetKeyDown(KeyCode.D))
        {
            InputBus.Broadcast(new Inputs.DirectionInputData(Inputs.InputType.DODGE, transform.right));
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

    public float GetRelativeTraslationSign(Vector3 vector)
    {
        return Mathf.Sign(transform.localScale.x * vector.x);
    }

    protected override void OnGameEnded(bool win)
    {
        base.OnGameEnded(win);
        traslation = Vector2.zero;
        Rigidbody.velocity = Vector2.zero;
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
        Manager.Combat.OnStun += OnStun;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Manager.Combat.OnStun -= OnStun;
    }

    protected override void OnInput(Inputs.InputData data)
    {
        if (Manager.Combat.IsStunned || !Active) return;
        switch (data.Type)
        {
            case Inputs.InputType.MOVE:
                Inputs.DirectionInputData directionData = data as Inputs.DirectionInputData;
                Move(directionData.Direction);
                if (!Mathf.Approximately(directionData.Direction.magnitude, 0F))
                {
                    if (UnityEngine.Random.value < 0.01F)
                    {
                        footstepsSystem.Play();
                        footstepsSystemRenderer.flip = new Vector3(Rigidbody.velocity.x > 0 ? 1F : -1F, 0F, 0F);
                    }
                }
                break;

            case Inputs.InputType.DODGE:
                Inputs.DirectionInputData dodgeDir = data as Inputs.DirectionInputData;
                if (IsDodging || dodgesCharges < 1 || Mathf.Approximately(dodgeDir.Direction.magnitude, 0F)) return;
                AddExternalForce(dodgeDir.Direction * dodgeForce);
                dodgeTimer = dodgeDuration;
                invulnerabilityCurrentSteps = invulnerabilityTimeSteps;
                OnInvulnerability?.Invoke(true);
                dodgesCharges--;
                if (!rechargeDodgesJob.Active)
                {
                    rechargeDodgesJob.Resume();
                }
                dashSystem.Play();
                dashSystem.GetComponent<ParticleSystemRenderer>().flip = new Vector3(Rigidbody.velocity.x > 0 ? 1F : -1F, 0F, 0F);
                if (GetRelativeTraslationSign(dodgeDir.Direction) > 0F)
                {
                    AnimatorDriver.DriveAnimation(new AnimatorDriver.AnimationData(AnimatorDriver.DASH_F));
                }
                else
                {
                    AnimatorDriver.DriveAnimation(new AnimatorDriver.AnimationData(AnimatorDriver.DASH_B));
                }
                break;
        }
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
}