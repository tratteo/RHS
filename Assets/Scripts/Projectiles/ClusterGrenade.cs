// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : ClusterGrenade.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

public class ClusterGrenade : Grenade
{
    [Header("Cluster")]
    [SerializeField] private RandomizedFloat detonationForce;

    protected override void OnChildSplit(GameObject obj)
    {
        base.OnChildSplit(obj);
        obj.transform.SetPositionAndRotation(transform.position.Perturbate(0.5F), Quaternion.Euler(0F, 0F, Random.value * 360));
        obj.GetComponent<Projectile>()?.DelegateLaunch(detonationForce);
    }
}