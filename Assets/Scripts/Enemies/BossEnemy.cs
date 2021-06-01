// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : BossEnemy.cs
//
// All Rights Reserved

using GibFrame;
using System.Collections;
using UnityEngine;

public class BossEnemy : Enemy
{
    [Header("Boss")]
    [SerializeField, Guarded] private Weapon weapon;
    [SerializeField, Guarded] private Animator stateMachine;
    [SerializeField, Guarded] private string idleTransitionId = "idle";
    [SerializeField] private int phasesAmount;
    private int currentPhase = 0;

    public override Weapon GetWeapon() => weapon;

    protected override void Awake()
    {
        base.Awake();
        weapon.SetOwner(this);
    }

    protected override void EngageBattle(Transform target)
    {
        base.EngageBattle(target);
        if (TargetContext != null)
        {
            stateMachine.SetBool(idleTransitionId, false);
            stateMachine.SetBool((++currentPhase).ToString(), true);
        }
    }

    protected override void SetStatus(Status status)
    {
        base.SetStatus(status);
        if (status == Status.IDLING)
        {
            stateMachine.SetBool(idleTransitionId, true);
            for (int i = 1; i < phasesAmount + 1; i++)
            {
                stateMachine.SetBool(i.ToString(), false);
            }
        }
    }

    protected override void Die()
    {
        if (currentPhase < phasesAmount)
        {
            StartCoroutine(AdvancePhase_C());
        }
        else
        {
            base.Die();
        }
    }

    private IEnumerator AdvancePhase_C()
    {
        stateMachine.SetBool((++currentPhase).ToString(), true);
        Rigidbody.velocity = Vector2.zero;
        Move(Vector2.zero);
        Collider.enabled = false;
        yield return new WaitForSeconds(2F);

        HealthSystem.Refull();
        Collider.enabled = true;
    }
}