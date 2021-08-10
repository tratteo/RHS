// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : YasukePhaseI.cs
//
// All Rights Reserved

using UnityEngine;

public class YasukePhaseI : BossPhaseStateMachine
{
    private Sword sword;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Owner.Move(Vector2.zero);
        sword = Owner.Weapon as Sword;
        sword.ToggleBlock(true, 1F);
        if (!sword)
        {
            Debug.LogError("Slayer is not equipped with a sword!");
        }
    }

    protected override float GetRange() => 5F;
}