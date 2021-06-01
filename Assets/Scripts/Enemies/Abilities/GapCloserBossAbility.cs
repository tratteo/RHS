// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : GapCloserBossAbility.cs
//
// All Rights Reserved

using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "GapCloserBossAbility", menuName = "Scriptable Objects/Bosses/Abilities/Gap Closer", order = 0)]
public class GapCloserBossAbility : Ability<BossEnemy>
{
    public override bool CanPerform(BossEnemy parent)
    {
        return base.CanPerform(parent) && Vector2.Distance(parent.transform.position, parent.TargetContext.Transform.position) > 4F;
    }

    protected override IEnumerator Execute_C(BossEnemy parent)
    {
        float duration = parent.Dash(parent.TargetContext.Transform.position - parent.transform.position, 2F);
        yield return new WaitForSeconds(duration);
        Complete();
    }
}