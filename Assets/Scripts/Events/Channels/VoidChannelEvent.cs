// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : VoidChannelEvent.cs
//
// All Rights Reserved

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "VoidChannelEvent", menuName = "Scriptable Objects/Channels/VoidChannelEvent", order = 0)]
public class VoidChannelEvent : ScriptableObject
{
    public event Action OnEvent = delegate { };

    public void Broadcast()
    {
        OnEvent?.Invoke();
    }
}