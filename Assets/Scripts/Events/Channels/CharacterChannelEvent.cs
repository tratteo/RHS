// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : CharacterChannelEvent.cs
//
// All Rights Reserved

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterChannel", menuName = "Scriptable Objects/Channels/CharacterChannel", order = 0)]
public class CharacterChannelEvent : ScriptableObject
{
    public event Action<bool> OnInvulnerabilityChanged = delegate { };

    public event Action<bool> OnGameEnded = delegate { };

    public void BroadcastInvulnerability(bool state)
    {
        OnInvulnerabilityChanged?.Invoke(state);
    }

    public void BroadcastOnGameEnded(bool win)
    {
        OnGameEnded?.Invoke(win);
    }
}