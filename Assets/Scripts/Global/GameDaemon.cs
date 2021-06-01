// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : GameDaemon.cs
//
// All Rights Reserved

using GibFrame;
using System.Collections.Generic;
using UnityEngine;

public class GameDaemon : MonoSingleton<GameDaemon>
{
    public const string LOADED_LEVEL = "loaded_level";
    [SerializeField, Guarded] private StringChannelEvent loadSceneChannel;
    [Header("Debug")]
    [SerializeField] private bool loadMenuOnStart = true;
    private Dictionary<string, object> persistentResources;

    public bool AddPersistentResource(string key, object value)
    {
        if (!persistentResources.ContainsKey(key))
        {
            persistentResources.Add(key, value);
            return true;
        }
        return false;
    }

    public bool HasPersistentResource(string key) => persistentResources.ContainsKey(key);

    public bool TryGetResource<T>(string key, out T value, bool consume = false)
    {
        if (persistentResources.TryGetValue(key, out object obj))
        {
            value = (T)obj;
            if (consume)
            {
                persistentResources.Remove(key);
            }
            return true;
        }
        value = default;
        return false;
    }

    protected override void Awake()
    {
        base.Awake();
        persistentResources = new Dictionary<string, object>();
        Inventory.Initialize();
        Settings.Initialize();
        Vibration.Initialize();
        Character.Initialize();
    }

    private void Start()
    {
        if (loadMenuOnStart)
        {
            loadSceneChannel.Broadcast(StringChannelEvent.MENU);
        }
    }
}