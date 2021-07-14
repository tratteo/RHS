// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : PoolDispatcher.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.ObjectPooling;
using GibFrame.Performance;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PoolDispatcher : MonoSingleton<PoolDispatcher>, ICommonUpdate
{
    private readonly List<PoolRequest> requests = new List<PoolRequest>();
    [SerializeField] private int maxPoolSize = 1000;

    public void RequestPool(string categoryId, GameObject prefab, int size)
    {
        PoolRequest find = requests.Find(r => r.Category.Equals(categoryId) && r.Prefab.Equals(prefab));
        if (find != null)
        {
            if (find.Size < maxPoolSize)
            {
                find.Size += size;
            }
        }
        else
        {
            find = new PoolRequest(categoryId, prefab, size);
            requests.Add(find);
        }
    }

    public void CommonUpdate(float deltaTime)
    {
        while (requests.Count > 0)
        {
            PoolRequest request = requests[requests.Count - 1];
            requests.RemoveAt(requests.Count - 1);
            ExecutePoolRequest(request);
        }
    }

    private void Start()
    {
        CommonUpdateManager.Register(this);
    }

    private void OnDestroy()
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
                category = new PoolCategory(request.Category);
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

    private class PoolRequest : IEquatable<PoolRequest>
    {
        public string Category { get; private set; }

        public GameObject Prefab { get; private set; }

        public int Size { get; set; }

        public PoolRequest(string category, GameObject prefab, int size)
        {
            Category = category;
            Prefab = prefab;
            Size = size;
        }

        public bool Equals(PoolRequest other)
        {
            return Category.Equals(other.Category) && Prefab.Equals(other.Prefab);
        }
    }
}