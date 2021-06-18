// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : CameraShakeEventBus.cs
//
// All Rights Reserved

using UnityEngine;

[CreateAssetMenu(fileName = "CameraShakeChannel", menuName = "Scriptable Objects/Channels/CameraShakeChannel", order = 0)]
public class CameraShakeEventBus : SingleParamEventBus<CameraShakeEventBus.Shake>
{
    public static readonly Shake.Parameters LIGHT_EXPLOSION = new Shake.Parameters(0.1F, 5F, 0.7F);
    public static readonly Shake.Parameters EXPLOSION = new Shake.Parameters(0.15F, 8F, 0.5F);
    public static readonly Shake.Parameters HEAVY_EXPLOSION = new Shake.Parameters(0.2F, 12F, 0.5F);
    public static readonly Shake.Parameters HUGE_EXPLOSION = new Shake.Parameters(0.35F, 16F, 0.75F);
    public static readonly Shake.Parameters HIT = new Shake.Parameters(0.1F, 12F, 2F);

    public class Shake
    {
        public Parameters Params { get; private set; }

        public Vector3 WorldPos { get; private set; }

        public Shake(Parameters parameters, Vector3 worldPos)
        {
            Params = parameters;
            WorldPos = worldPos;
        }

        public class Parameters
        {
            public float Duration { get; private set; }

            public float Amplitude { get; private set; }

            public float Frequency { get; private set; }

            public Parameters(float duration, float amplitude, float frequency)
            {
                Duration = duration;
                Amplitude = amplitude;
                Frequency = frequency;
            }
        }
    }
}