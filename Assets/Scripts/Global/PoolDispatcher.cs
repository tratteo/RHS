// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : PoolDispatcher.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.ObjectPooling;
using GibFrame.Performance;
using System.Collections.Generic;
using UnityEngine;

public class PoolDispatcher : MonoSingleton<PoolDispatcher>, ICommonUpdate
{
    private readonly Queue<PoolRequest> poolRequests = new Queue<PoolRequest>();

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