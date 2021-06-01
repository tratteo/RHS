// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : SingleParamChannelEvent.cs
//
// All Rights Reserved

using System;
using UnityEngine;

public abstract class SingleParamChannelEvent<Type> : ScriptableObject
{
    public event Action<Type> OnEvent = delegate { };

    public void Broadcast(Type arg)
    {
        OnEvent?.Invoke(arg);
    }
}