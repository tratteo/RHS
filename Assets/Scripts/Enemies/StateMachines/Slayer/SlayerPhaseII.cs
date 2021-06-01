// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : SlayerPhaseII.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

public class SlayerPhaseII : BossPhaseStateMachine
{
    [Header("Slayer II")]
    [SerializeField, Guarded] private GameObject gapCloserPrefab;
    private Sword sword;
    private GapCloserBossAbility gapCloser;

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
        gapCloser = (GapCloserBossAbility)Ability<BossEnemy>.Attach(gapCloserPrefab, Owner);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (CanExecute())
        {
            if (gapCloser.CanPerform())
            {
                gapCloser.Perform();
            }
        }
    }

    protected override float GetAttackRange() => 5F;
}