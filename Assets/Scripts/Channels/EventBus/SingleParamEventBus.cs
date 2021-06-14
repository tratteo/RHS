// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : SingleParamEventBus.cs
//
// All Rights Reserved

using System;
using UnityEngine;

public abstract class SingleParamEventBus<Type> : ScriptableObject
{
    public event Action<Type> OnEvent = delegate { };

    public void Broadcast(Type arg)
    {
        OnEvent?.Invoke(arg);
    }
}