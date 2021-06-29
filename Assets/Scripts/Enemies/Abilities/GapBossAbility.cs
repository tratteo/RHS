﻿// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : GapCloserBossAbility.cs
//
// All Rights Reserved

using System.Collections;
using UnityEngine;

public class GapBossAbility : Ability<BossEnemy>
{
    [SerializeField] private bool towards = true;
    [SerializeField] private float triggerDistance = 10F;

    public override bool CanPerform()
    {
        float distance = Vector2.Distance(Parent.transform.position, Parent.TargetContext.Transform.position);
        return base.CanPerform() && (towards && distance > triggerDistance || !towards && distance < triggerDistance);
    }

    protected override IEnumerator Execute_C()
    {
        Vector2 axis = Parent.TargetContext.Transform.position - Parent.transform.position;
        if (!towards) axis *= -1F;
        float duration = Parent.Dash(axis, 2.65F);
        yield return new WaitForSeconds(duration);
        Complete();
    }
}