// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : MildewerPhase.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

public abstract class MildewerPhase : BossPhaseStateMachine
{
    [SerializeField] private RandomizedFloat shootUpdate;
    [SerializeField] private GameObject rootsSpawnAbilityPrefab;
    private Ability<BossEnemy> rootsSpawnAbility;
    private float shootTimer = 0F;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Owner.Invulnerable = true;
        rootsSpawnAbility = Ability<BossEnemy>.AttachTo(rootsSpawnAbilityPrefab, Owner);
        shootTimer = shootUpdate;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        Destroy(rootsSpawnAbility.gameObject);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (rootsSpawnAbility.CanPerform())
        {
            rootsSpawnAbility.Perform();
        }
        if (!IsPerformingAbility && CanExecute())
        {
            if (shootTimer > 0)
            {
                shootTimer -= Time.deltaTime;
                if (shootTimer < 0F)
                {
                    (Owner.Weapon as Shooter).TriggerShoot(Vector2.Distance(Owner.BattleContext.Transform.position, Owner.transform.position) * 0.825F);
                    shootTimer = shootUpdate;
                }
            }
        }
    }

    protected override float GetRange() => 0F;
}