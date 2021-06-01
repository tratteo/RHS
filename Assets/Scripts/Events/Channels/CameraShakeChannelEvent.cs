// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : CameraShakeChannelEvent.cs
//
// All Rights Reserved

using UnityEngine;

[CreateAssetMenu(fileName = "CameraShakeChannel", menuName = "Scriptable Objects/Channels/CameraShakeChannel", order = 0)]
public class CameraShakeChannelEvent : SingleParamChannelEvent<CameraShakeChannelEvent.Shake>
{
    public class Shake
    {
        public float Duration { get; private set; }

        public float Amplitude { get; private set; }

        public Shake(float duration, float amplitude)
        {
            Duration = duration;
            Amplitude = amplitude;
        }
    }
}