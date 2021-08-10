// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : BossPhaseTransitionStateMachine.cs
//
// All Rights Reserved

using UnityEngine;

public class BossPhaseTransitionStateMachine : BossStateMachine
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Owner.SetInteraction();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        Owner.Move(Vector2.zero);
        Owner.Rigidbody.velocity = Vector2.zero;
    }
}