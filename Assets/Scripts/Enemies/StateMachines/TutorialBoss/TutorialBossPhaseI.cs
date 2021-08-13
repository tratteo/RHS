// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : TutorialBossPhaseI.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

public class TutorialBossPhaseI : BossPhaseStateMachine
{
    private const int LINEAR = 0;
    private const int DESTRUCTIBLE = 1;
    private const int DEFLECTABLE = 2;
    [SerializeField] private TutorialEventBus tutorialBus;
    [Header("Bomber")]
    [SerializeField] private RandomizedFloat shootUpdate;
    private float shootTimer;
    private Shooter shooter;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Weapon weapon = Owner.Weapon;
        if (weapon is Shooter)
        {
            shooter = weapon as Shooter;
        }
        else
        {
            Debug.LogError("TUtorial boss has no shooter weapon");
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
            else if (GetDistanceToTarget() <= GetRange() * 1.75F)
            {
                shooter.TriggerShoot();
                shootTimer = shootUpdate;
            }
        }
        float healthPercentage = Owner.GetHealthPercentage();
        if (shooter.ProjectileIndex != DESTRUCTIBLE && healthPercentage > 0.35F && healthPercentage <= 0.7F)
        {
            shooter.SetPrefabIndex(DESTRUCTIBLE);
            tutorialBus.BroadcastNextFocus();
            shootTimer = 3F;
        }
        else if (shooter.ProjectileIndex != DEFLECTABLE && Owner.GetHealthPercentage() <= 0.35F)
        {
            shooter.SetPrefabIndex(DEFLECTABLE);
            tutorialBus.BroadcastNextFocus();
            shootTimer = 3F;
        }
    }

    protected override Vector3 GetMovementDirection()
    {
        Vector2 dir = (Owner.transform.position - Owner.BattleContext.Transform.position).normalized;
        float distance = Vector2.Distance(Owner.BattleContext.Transform.position, Owner.transform.position);
        if (distance > GetRange())
        {
            return -dir.Perturbate();
        }
        else if (distance < GetRange() * 0.75F)
        {
            return dir.Perturbate();
        }
        else
        {
            dir = UnityEngine.Random.value < 0.5F ? Quaternion.AngleAxis(90F, Vector3.forward) * dir : Quaternion.AngleAxis(-90F, Vector3.forward) * dir;
            return dir;
        }
    }

    protected override float GetRange() => 10F;
}