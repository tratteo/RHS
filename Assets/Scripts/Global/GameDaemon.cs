// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : GameDaemon.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.ObjectPooling;
using GibFrame.Performance;
using System.Collections.Generic;
using UnityEngine;

public class GameDaemon : MonoSingleton<GameDaemon>, ICommonUpdate
{
    public const string LOADED_LEVEL = "loaded_level";
    private readonly Queue<PoolRequest> poolRequests = new Queue<PoolRequest>();
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

    public void RequestPool(string categoryId, GameObject prefab, int size)
    {
        poolRequests.Enqueue(new PoolRequest(categoryId, prefab, size));
    }

    public void CommonUpdate(float deltaTime)
    {
        while (poolRequests.Count > 0)
        {
            PoolRequest request = poolRequests.Dequeue();
            ExecutePoolRequest(request);
        }
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

    private void OnEnable()
    {
        CommonUpdateManager.Register(this);
    }

    private void OnDisable()
    {
        CommonUpdateManager.Unregister(this);
    }

    private bool ExecutePoolRequest(PoolRequest request)
    {
        if (!PoolManager.Instance)
        {
            Debug.LogError("Unable to find the singleton PoolManager");
            return false;
        }
        else
        {
            PoolCategory category = PoolManager.Instance.GetCategory(request.Category);
            if (category == null)
            {
                category = new PoolCategory(Layers.PROJECTILES);
            }
            Pool pool = category.GetPool(request.Prefab.name);
            if (pool == null)
            {
                pool = new Pool(request.Prefab.name, request.Prefab, request.Size);
                category.AddPool(pool);
                PoolManager.Instance.AddCategory(category);
                return true;
            }
            return false;
        }
    }

    private void Start()
    {
        if (loadMenuOnStart)
        {
            loadSceneChannel.Broadcast(StringChannelEvent.MENU);
        }
    }

    private class PoolRequest
    {
        public string Category { get; private set; }

        public GameObject Prefab { get; private set; }

        public int Size { get; private set; }

        public PoolRequest(string category, GameObject prefab, int size)
        {
            Category = category;
            Prefab = prefab;
            Size = size;
        }
    }
}