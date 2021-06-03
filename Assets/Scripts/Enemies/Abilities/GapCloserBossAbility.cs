// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : GapCloserBossAbility.cs
//
// All Rights Reserved

using System.Collections;
using UnityEngine;

public class GapCloserBossAbility : Ability<BossEnemy>
{
    public override bool CanPerform()
    {
        return base.CanPerform() && Vector2.Distance(Parent.transform.position, Parent.TargetContext.Transform.position) > 4F;
    }

    protected override IEnumerator Execute_C()
    {
        float duration = Parent.Dash(Parent.TargetContext.Transform.position - Parent.transform.position, 2.5F);
        yield return new WaitForSeconds(duration);
        Complete();
    }
}