// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : DamageOverTimeStatusEffect.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.Performance;
using UnityEngine;

public class DamageOverTimeStatusEffect : StatusEffect
{
    [Header("Damage over time")]
    [SerializeField] private float damage = 1F;
    [SerializeField] private float interval = 0.5F;
    private UpdateJob damageJob;
    private IHealthHolder healthHolder;

    public override void CommonUpdate(float deltaTime)
    {
        base.CommonUpdate(deltaTime);
        damageJob.Step(deltaTime);
    }

    protected override void OnSpawn()
    {
        if (!GetStatusEffectFunctionalInterface(out healthHolder))
        {
            Destroy();
        }
        else
        {
            damageJob = new UpdateJob(new Callback(() => healthHolder.Damage(new IHealthHolder.Data(gameObject, damage))), interval);
        }
    }
}