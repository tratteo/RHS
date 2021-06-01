// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : BossStateMachine.cs
//
// All Rights Reserved

using UnityEngine;

public class BossStateMachine : StateMachineBehaviour
{
    protected BossEnemy Owner { get; private set; }

    protected Vector3 TargetDirection => Owner.TargetContext.Transform.position - Owner.transform.position;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Owner = animator.gameObject.GetComponentInChildren<BossEnemy>();
    }

    protected float GetDistanceToTarget() => Vector2.Distance(Owner.transform.position, Owner.TargetContext.Transform.position);
}