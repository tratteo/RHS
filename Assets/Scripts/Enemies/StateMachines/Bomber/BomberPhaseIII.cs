// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : BomberPhaseIII.cs
//
// All Rights Reserved

using UnityEngine;

public class BomberPhaseIII : BomberPhase
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Launcher.SetPrefabIndex(CLUSTER_GRENADES_INDEX);
    }
}