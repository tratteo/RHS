﻿// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : BombBarrageBossAbility.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.ObjectPooling;
using System.Collections;
using UnityEngine;

public class BombBarrageBossAbility : Ability<BossEnemy>
{
    [SerializeField] private RandomizedFloat explosionInterval;
    [SerializeField] private RandomizedFloat duration;
    [SerializeField] private GameObject delayedExplosionPrefab;

    protected override IEnumerator Execute_C()
    {
        Parent.EnableSelfMovement = true;
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        float currentTime = 0F;
        float explosionTimer = explosionInterval;
        float duration = this.duration;
        while ((currentTime += Time.fixedDeltaTime) < duration && Parent.CurrentStatus == Enemy.Status.ATTACKING)
        {
            explosionTimer -= Time.fixedDeltaTime;
            if (explosionTimer <= 0F)
            {
                GameObject obj = PoolManager.Instance.Spawn(Categories.PROJECTILES, delayedExplosionPrefab.name, Parent.TargetContext.Transform.position.Perturbate(3F), Quaternion.identity);
                Grenade grenade = obj.GetComponent<Grenade>();
                grenade.Setup(Parent, Parent.TargetContext.Transform);
                grenade.DelegateLaunch(0F);
                explosionTimer = explosionInterval;
            }
            yield return wait;
        }
        Complete();
    }

    private void Awake()
    {
        PoolDispatcher.Instance.RequestPool(Categories.PROJECTILES, delayedExplosionPrefab, 50);
    }
}