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
    [SerializeField, Guarded] private CinemachineVirtualCamera cinemachineCamera;
    private Camera mainCamera;

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

    protected override void Awake()
    {
        base.Awake();
        mainCamera = Camera.main;
        noise = cinemachineCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
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
        if (!Active) return; noise.m_AmplitudeGain = shake.Params.Amplitude;
        noise.m_FrequencyGain = shake.Params.Frequency;
        shakeDuration = shake.Params.Duration;
        //Vector2 screenPos = mainCamera.WorldToViewportPoint(shake.WorldPos);
        //if (screenPos.x > 0F && screenPos.x < 1F && screenPos.y > 0F && screenPos.y < 1F)
        //{
        //    noise.m_AmplitudeGain = shake.Params.Amplitude;
        //    noise.m_FrequencyGain = shake.Params.Frequency;
        //    shakeDuration = shake.Params.Duration;
        //}
    }
}