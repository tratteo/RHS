// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : CharacterKinematic.cs
//
// All Rights Reserved

using GibFrame;
using System;
using UnityEngine;

public class CharacterKinematic : CharacterComponent, IShadowOwner
{
    [Header("Channels")]
    [SerializeField, Guarded] private AnimationChannelEvent animationChannelEvent;
    [SerializeField, Guarded] private InputChannelEvent inputChannel;
    [Header("Parameters")]
    [SerializeField] private float movementSpeed = 5F;
    [Header("Dodge")]
    [SerializeField] private float dodgeDuration = 0.3F;
    [SerializeField] private int invulnerabilityTimeSteps = 10;
    private Vector2 traslation;
    private float dodgeTimer = 0F;
    private int invulnerabilityCurrentSteps;
    private float startJumpY = 0F;

    public bool IsJumping { get; private set; } = false;

    private bool IsDodging => dodgeTimer > 0F;

    public event Action<bool> OnChangeGroundedState;

    public override void CommonFixedUpdate(float fixedDeltaTime)
    {
        if (!IsDodging && !IsJumping)
        {
            Rigidbody.velocity = traslation;
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
                Invulnerability(false);
                invulnerabilityCurrentSteps = -1;
            }
        }
        if (Mathf.Approximately(Rigidbody.velocity.magnitude, 0F))
        {
            animationChannelEvent.Broadcast(new AnimationChannelEvent.AnimationData(CharacterAnimator.IDLE, null));
        }
        else
        {
            animationChannelEvent.Broadcast(new AnimationChannelEvent.AnimationData(CharacterAnimator.RUN, Rigidbody.velocity.magnitude));
        }

        if (IsJumping && startJumpY > transform.position.y)
        {
            transform.position = new Vector3(transform.position.x, startJumpY, transform.position.z);
            Rigidbody.gravityScale = 0F;
            IsJumping = false;
            Rigidbody.velocity = Vector2.zero;
            OnChangeGroundedState?.Invoke(true);
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
                Move(Vector2.zero);
                Invulnerability(false);
            }
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            Rigidbody.gravityScale = 3F;
            IsJumping = true;
            OnChangeGroundedState?.Invoke(false);
            startJumpY = transform.position.y;
            Vector2 verticalVel = Rigidbody.velocity * Vector2.up;
            Rigidbody.velocity = new Vector2(Rigidbody.velocity.x, 10F);
            //Rigidbody.AddForce((Vector2.up - verticalVel.normalized) * 1000F, ForceMode2D.Impulse);
        }
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

    private void Invulnerability(bool state)
    {
        EventChannel.BroadcastInvulnerability(state);
    }

    private void OnInput(Inputs.InputData data)
    {
        switch (data.Type)
        {
            case Inputs.InputType.MOVE:
                Inputs.DirectionInputData directionData = data as Inputs.DirectionInputData;
                Move(directionData.Direction);
                break;

            case Inputs.InputType.DODGE:
                if (IsDodging) return;
                Inputs.DirectionInputData dodgeDir = data as Inputs.DirectionInputData;
                Rigidbody.AddForce(dodgeDir.Direction * 40F, ForceMode2D.Impulse);
                dodgeTimer = dodgeDuration;
                invulnerabilityCurrentSteps = invulnerabilityTimeSteps;
                Invulnerability(true);
                break;
        }
    }
}