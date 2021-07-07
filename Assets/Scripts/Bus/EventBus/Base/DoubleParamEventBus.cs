// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : DoubleParamEventBus.cs
//
// All Rights Reserved

using System;
using UnityEngine;

public abstract class DoubleParamEventBus<Type1, Type2> : ScriptableObject
{
    public event Action<Type1, Type2> OnEvent = delegate { };

    public void Broadcast(Type1 arg1, Type2 arg2)
    {
        OnEvent?.Invoke(arg1, arg2);
    }
}