// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : TutorialEventBus.cs
//
// All Rights Reserved

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialEventBus", menuName = "Scriptable Objects/Bus/TutorialBus", order = 0)]
public class TutorialEventBus : ScriptableObject
{
    public event Action<GameObject> OnFocus = delegate { };

    public event Action<GameObject> OnReleaseFocus = delegate { };

    public event Action OnNextFocus = delegate { };

    public void BroadcastNextFocus()
    {
        OnNextFocus?.Invoke();
    }

    public void BroadcastFocus(GameObject sender)
    {
        OnFocus?.Invoke(sender);
    }

    public void BroadcastReleaseFocus(GameObject sender)
    {
        OnReleaseFocus?.Invoke(sender);
    }
}