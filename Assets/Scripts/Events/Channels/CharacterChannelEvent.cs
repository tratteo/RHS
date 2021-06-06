// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : CharacterChannelEvent.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterChannel", menuName = "Scriptable Objects/Channels/CharacterChannel", order = 0)]
public class CharacterChannelEvent : ScriptableObject
{
    public Event<bool> GameEndedEvent { get; private set; } = new Event<bool>();

    public Event<bool> OnStunEvent { get; private set; } = new Event<bool>();
}