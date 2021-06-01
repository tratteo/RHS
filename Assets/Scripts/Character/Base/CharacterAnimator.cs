// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : CharacterAnimator.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public const string RUN = "run";
    public const string IDLE = "idle";
    private Animator animator;
    [Header("Channels")]
    [SerializeField, Guarded] private AnimationChannelEvent animationChannelEvent;

    private int speedHash;
    private string currentPlaying;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        speedHash = Animator.StringToHash("Speed");
    }

    private void OnEnable()
    {
        animationChannelEvent.OnEvent += OnAnimationEvent;
    }

    private void OnDisable()
    {
        animationChannelEvent.OnEvent -= OnAnimationEvent;
    }

    private void OnAnimationEvent(AnimationChannelEvent.AnimationData data)
    {
        if (currentPlaying == data.Id) return;
        switch (data.Id)
        {
            case RUN:
                float speed = data.GetArgAs<float>();
                //animator.SetFloat(speedHash, speed);
                break;

            case IDLE:
                break;
        }
        animator.Play(data.Id);
        currentPlaying = data.Id;
    }
}