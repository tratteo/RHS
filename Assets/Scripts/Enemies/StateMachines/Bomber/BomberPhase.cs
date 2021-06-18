﻿// Copyright (c) Matteo Beltrame
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
    private float shootTimer;

    protected GrenadeLauncher Launcher { get; private set; } = null;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Weapon weapon = Owner.GetWeapon();
        if (weapon is GrenadeLauncher)
        {
            Launcher = weapon as GrenadeLauncher;
        }
        else
        {
            Debug.LogError("Bomber has no shooter weapon");
        }
        shootTimer = shootUpdate;
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
            else
            {
                Launcher.TriggerShoot(Vector2.Distance(Owner.TargetContext.Transform.position, Owner.transform.position) * 0.825F);
                shootTimer = shootUpdate;
            }
        }
    }

    protected override float GetAttackRange() => 8.5F;

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