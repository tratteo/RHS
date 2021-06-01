// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : VoidChannelEventListener.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;
using UnityEngine.Events;

public class VoidChannelEventListener : MonoBehaviour
{
    [SerializeField, Guarded] private VoidChannelEvent channel;
    [SerializeField] private UnityEvent OnEventRaised;

    private void OnEnable()
    {
        if (channel != null)
        {
            channel.OnEvent += Respond;
        }
    }

    private void OnDisable()
    {
        if (channel != null)
        {
            channel.OnEvent -= Respond;
        }
    }

    private void Respond()
    {
        OnEventRaised?.Invoke();
    }
}