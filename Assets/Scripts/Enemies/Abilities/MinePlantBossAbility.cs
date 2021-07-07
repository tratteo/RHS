// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : MinePlantBossAbility.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.ObjectPooling;
using System.Collections;
using UnityEngine;

public class MinePlantBossAbility : Ability<BossEnemy>
{
    [SerializeField, Guarded] private GameObject minePrefab;

    protected override IEnumerator Execute_C()
    {
        GameObject obj = PoolManager.Instance.Spawn(Categories.PROJECTILES, minePrefab.name, Parent.transform.position, Quaternion.identity);
        Mine mine = obj.GetComponent<Mine>();
        if (mine)
        {
            mine.Setup(Parent, Parent.TargetContext.Transform);
        }
        Complete();
        yield break;
    }

    private void Awake()
    {
        PoolDispatcher.Instance.RequestPool(Categories.PROJECTILES, minePrefab, 50);
    }
}