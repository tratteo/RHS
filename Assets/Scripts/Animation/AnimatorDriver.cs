// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : AnimatorDriver.cs
//
// All Rights Reserved

using UnityEngine;

public class AnimatorDriver : MonoBehaviour
{
    public const string RUN = "run";
    public const string IDLE = "idle";
    public const string DASH_F = "dash_f";
    public const string DASH_B = "dash_b";
    private Animator animator;

    [SerializeField] private int animationLayerIndex = 0;
    [SerializeField] private string rotatedSuffix = "_mirror";

    private int speedHash;
    private string currentPlaying;

    public void DriveAnimation(AnimationData data)
    {
        string animId = data.Id;
        switch (data.Id)
        {
            case RUN:
                float speed = data.GetArgAs<float>();
                animator.SetFloat(speedHash, speed);
                break;

            case IDLE:
                break;
        }
        if (data.HorizontalRotated)
        {
            animId += rotatedSuffix;
        }
        if (currentPlaying == animId) return;
        animator.Play(animId, animationLayerIndex);
        currentPlaying = animId;
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        speedHash = Animator.StringToHash("Speed");
    }

    public class AnimationData
    {
        public string Id { get; private set; }

        public object Arg { get; private set; }

        public bool HorizontalRotated { get; private set; }

        public AnimationData(string id, bool horizontalRotated = false, object args = null)
        {
            HorizontalRotated = horizontalRotated;
            Id = id;
            Arg = args;
        }

        public T GetArgAs<T>() => (T)Arg;
    }
}