// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : AnimatorDriver.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

public class AnimatorDriver : MonoBehaviour
{
    public const string RUN = "run";
    public const string IDLE = "idle";
    public const string DASH = "dash";
    private Animator animator;
    [Header("Channels")]
    [SerializeField, Guarded] private AnimationChannelEvent animationChannelEvent;
    [SerializeField] private int animationLayerIndex = 0;

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
        animator.Play(data.Id, animationLayerIndex);
        currentPlaying = data.Id;
    }
}