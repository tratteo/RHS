// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : ResistanceMinion.cs
//
// All Rights Reserved

using GibFrame.ObjectPooling;
using UnityEngine;

public class ResistanceMinion : EnemyMinion
{
    [SerializeField] private float damage = 100F;
    [SerializeField] private GameObject spawnOnDestroyPrefab;

    public override void OnDie()
    {
        Owner.HealthSystem.Decrease(damage);
        if (spawnOnDestroyPrefab)
        {
            _ = PoolManager.Instance.Spawn(Categories.INTERACTABLES, spawnOnDestroyPrefab.name, transform.position, transform.rotation);
        }
        base.OnDie();
    }

    private void Awake()
    {
        PoolDispatcher.Instance.RequestPool(Categories.INTERACTABLES, spawnOnDestroyPrefab, 1);
    }
}