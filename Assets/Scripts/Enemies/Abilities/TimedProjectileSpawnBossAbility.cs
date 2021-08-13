// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : TimedProjectileSpawnBossAbility.cs
//
// All Rights Reserved

using GibFrame;
using System.Collections;
using UnityEngine;

public class TimedProjectileSpawnBossAbility : Ability<BossEnemy>
{
    [SerializeField, Guarded] private GameObject projectilePrefab;

    protected override IEnumerator Execute_C()
    {
        Projectile.Create(projectilePrefab.name, Parent.transform.position, Quaternion.identity, Parent, Parent.BattleContext.Transform);
        Complete();
        yield break;
    }

    private void Awake()
    {
        PoolDispatcher.Instance.RequestPool(Categories.PROJECTILES, projectilePrefab, 50);
    }
}