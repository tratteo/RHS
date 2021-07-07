// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : ProjectileComponent.cs
//
// All Rights Reserved

using GibFrame.Performance;
using UnityEngine;

[RequireComponent(typeof(Projectile))]
public abstract class ProjectileComponent : MonoBehaviour, ICommonUpdate
{
    protected Projectile Parent { get; private set; }

    public virtual void CommonUpdate(float deltaTime)
    {
    }

    protected virtual void Awake()
    {
        Parent = GetComponent<Projectile>();
        if (!Parent)
        {
            Debug.LogError(gameObject + "[Projectile component]: unable to locate parent");
        }
    }

    protected virtual void OnEnable()
    {
        CommonUpdateManager.Register(this);
    }

    protected virtual void OnDisable()
    {
        CommonUpdateManager.Unregister(this);
    }
}