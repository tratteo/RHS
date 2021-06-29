// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : VoidEventBus.cs
//
// All Rights Reserved

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "VoidBus", menuName = "Scriptable Objects/Bus/VoidBus", order = 0)]
public class VoidEventBus : ScriptableObject
{
    public event Action OnEvent = delegate { };

    public void Broadcast()
    {
        OnEvent?.Invoke();
    }
}