// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : CharacterCamera.cs
//
// All Rights Reserved

using Cinemachine;
using GibFrame;
using UnityEngine;

public class CharacterCamera : CharacterComponent
{
    [Header("Channels")]
    [SerializeField, Guarded] private CameraShakeChannelEvent cameraShakeChannel;
    [SerializeField, Guarded] private new CinemachineVirtualCamera camera;

    private CinemachineBasicMultiChannelPerlin noise;
    private float shakeDuration = 0F;

    public override void CommonUpdate(float deltaTime)
    {
        if (shakeDuration > 0F)
        {
            shakeDuration -= deltaTime;
            if (shakeDuration <= 0)
            {
                noise.m_AmplitudeGain = 0F;
            }
        }
    }

    protected override void OnGameEnded(bool win)
    {
        base.OnGameEnded(win);
        CinemachineBasicMultiChannelPerlin basicMultiChannelPerlin = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        basicMultiChannelPerlin.m_AmplitudeGain = 0;
    }

    protected override void Awake()
    {
        base.Awake();
        noise = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        cameraShakeChannel.OnEvent += ShakeCamera;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        cameraShakeChannel.OnEvent -= ShakeCamera;
    }

    private void ShakeCamera(CameraShakeChannelEvent.Shake shake)
    {
        CinemachineBasicMultiChannelPerlin basicMultiChannelPerlin = camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        basicMultiChannelPerlin.m_AmplitudeGain = shake.Amplitude;
        shakeDuration = shake.Duration;
    }
}