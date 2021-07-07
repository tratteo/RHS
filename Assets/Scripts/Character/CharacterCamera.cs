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
    [SerializeField, Guarded] private CameraTargetGroupEventBus targetGroupEventBus;
    [SerializeField, Guarded] private CameraShakeEventBus cameraShakeChannel;
    [Header("References")]
    [SerializeField, Guarded] private CinemachineVirtualCamera cinemachineCamera;
    [SerializeField, Guarded] private CinemachineTargetGroup targetGroup;
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
        targetGroup.AddMember(Manager.transform, 2F, 1F);
        AdjustCamera(Settings.GetSetting<float>(SettingsData.CAMERA_HARDNESS));
        targetGroupEventBus.OnEvent += OnTargetGroupRequest;
        cameraShakeChannel.OnEvent += ShakeCamera;
        Settings.OnSettingChanged += OnSettingChanged;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        targetGroupEventBus.OnEvent -= OnTargetGroupRequest;
        cameraShakeChannel.OnEvent -= ShakeCamera;
        Settings.OnSettingChanged -= OnSettingChanged;
    }

    private void OnTargetGroupRequest(Transform transform, bool add)
    {
        if (add)
        {
            if (targetGroup.FindMember(transform) < 0)
            {
                targetGroup.AddMember(transform, 1.5F, 1F);//cinemachineCamera.m_Lens.OrthographicSize);
            }
        }
        else
        {
            targetGroup.RemoveMember(transform);
        }
    }

    private void AdjustCamera(float hardness)
    {
        CinemachineFramingTransposer transposer = cinemachineCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        if (transposer)
        {
            transposer.m_XDamping = hardness;
            transposer.m_YDamping = hardness;
            transposer.m_ZDamping = hardness;
        }
    }

    private void OnSettingChanged(string key, object newVal)
    {
        if (key.Equals(SettingsData.CAMERA_HARDNESS))
        {
            float newHardness = (float)newVal;
            AdjustCamera(newHardness);
        }
    }

    private void ShakeCamera(CameraShakeEventBus.Shake shake)
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