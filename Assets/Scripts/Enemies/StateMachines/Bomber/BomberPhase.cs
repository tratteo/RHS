// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : BomberPhase.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

public class BomberPhase : BossPhaseStateMachine
{
    public const int CLUSTER_GRENADES_INDEX = 1;
    public const int GRENADES_INDEX = 0;
    [Header("Bomber")]
    [SerializeField] private RandomizedFloat shootUpdate;
    [SerializeField, Guarded] private GameObject gapBossAbilityPrefab;
    private float shootTimer;
    private GapBossAbility gapBossAbility;

    protected Shooter Launcher { get; private set; } = null;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Weapon weapon = Owner.GetWeapon();
        if (weapon is Shooter)
        {
            Launcher = weapon as Shooter;
        }
        else
        {
            Debug.LogError("Bomber has no shooter weapon");
        }
        shootTimer = shootUpdate;
        gapBossAbility = (GapBossAbility)Ability<BossEnemy>.AttachTo(gapBossAbilityPrefab, Owner);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (!IsPerformingAbility && CanExecute())
        {
            if (shootTimer > 0)
            {
                shootTimer -= Time.deltaTime;
            }
            else if (GetDistanceToTarget() <= GetAttackRange() * 1.75F)
            {
                Launcher.TriggerShoot(Vector2.Distance(Owner.TargetContext.Transform.position, Owner.transform.position) * 0.825F);
                shootTimer = shootUpdate;
            }
        }
        if (CanExecute())
        {
            if (gapBossAbility.CanPerform())
            {
                gapBossAbility.Perform();
            }
        }
    }

    protected override float GetAttackRange() => 10F;

    protected override Vector3 GetMovementDirection()
    {
        Vector2 dir = (Owner.transform.position - Owner.TargetContext.Transform.position).normalized;
        float distance = Vector2.Distance(Owner.TargetContext.Transform.position, Owner.transform.position);
        if (distance > GetAttackRange())
        {
            return -dir.Perturbate();
        }
        else if (distance < GetAttackRange() * 0.75F)
        {
            return dir.Perturbate();
        }
        else
        {
            dir = UnityEngine.Random.value < 0.5F ? Quaternion.AngleAxis(90F, Vector3.forward) * dir : Quaternion.AngleAxis(-90F, Vector3.forward) * dir;
            return dir;
        }
    }
}