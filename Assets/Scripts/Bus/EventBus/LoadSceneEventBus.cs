// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : LoadSceneEventBus.cs
//
// All Rights Reserved

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "LoadSceneBus", menuName = "Scriptable Objects/Bus/LoadSceneBus", order = 0)]
public class LoadSceneEventBus : StringEventBus
{
    public const string MENU = "Menu";

    public event Action OnReloadScene = delegate { };

    public void ReloadCurrentScene()
    {
        OnReloadScene?.Invoke();
    }
}