// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : ClusterGrenade.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.ObjectPooling;
using UnityEngine;

public class ClusterGrenade : Grenade
{
    [Header("Cluster")]
    [SerializeField] private int clusterDimension = 3;
    [SerializeField] private RandomizedFloat detonationForce;
    [SerializeField, Guarded] private GameObject clusterGrenadePrefab = null;

    public override void Deflect(IAgent agent)
    {
        return;
    }

    protected override void Detonate()
    {
        cameraShakeChannel.Broadcast(new CameraShakeEventBus.Shake(CameraShakeEventBus.EXPLOSION, transform.position));
        for (int i = 0; i < clusterDimension; i++)
        {
            GameObject obj = PoolManager.Instance.Spawn(Layers.PROJECTILES, clusterGrenadePrefab.name, transform.position.Perturbate(0.5F), Quaternion.Euler(0F, 0F, Random.value * 360));
            obj.layer = gameObject.layer;
            obj.GetComponent<Grenade>().Launch(detonationForce);
        }
        gameObject.SetActive(false);
    }

    protected override void Awake()
    {
        base.Awake();
        PoolDispatcher.Instance.RequestPool(Layers.PROJECTILES, clusterGrenadePrefab, 50);
    }
}