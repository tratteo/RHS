// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : YasukePhaseII.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

public class YasukePhaseII : BossPhaseStateMachine
{
    [Header("Yasuke II")]
    [SerializeField, Guarded] private GameObject gapCloserPrefab;
    private Sword sword;
    private GapBossAbility gapCloser;

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
        gapCloser = (GapBossAbility)Ability<BossEnemy>.AttachTo(gapCloserPrefab, Owner);
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

    protected override float GetRange() => 5F;
}