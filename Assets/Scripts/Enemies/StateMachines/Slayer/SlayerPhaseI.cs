// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : SlayerPhaseI.cs
//
// All Rights Reserved

using UnityEngine;

public class SlayerPhaseI : BossPhaseStateMachine
{
    private Sword sword;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Owner.Move(Vector2.zero);
        sword = Owner.GetWeapon() as Sword;
        sword.ToggleBlock(true, 1F);
        if (!sword)
        {
            Debug.LogError("Slayer is not equipped with a sword!");
        }
    }

    protected override float GetAttackRange() => 5F;
}