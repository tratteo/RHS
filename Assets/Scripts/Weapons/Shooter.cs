// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Shooter.cs
//
// All Rights Reserved

using GibFrame.ObjectPooling;
using UnityEngine;

public class Shooter : Weapon
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject[] projectilePrefabs;
    private int prefabIndex = 0;

    public void SetPrefabIndex(int index)
    {
        if (index > 0 && index < projectilePrefabs.Length)
        {
            prefabIndex = index;
        }
    }

    public Projectile TriggerShoot(float param = 1F)
    {
        // Debug.Log(firePoint == null);
        GameObject obj = PoolManager.Instance.Spawn(Layers.PROJECTILES, projectilePrefabs[prefabIndex].name, firePoint.position, firePoint.rotation);
        Projectile projectile = obj.GetComponent<Projectile>();
        if (!projectile)
        {
            Debug.LogError("Instantiated a non projectile from a shooter!");
        }
        else
        {
            projectile.DelegateLaunch(param);
            projectile.DamageMultiplier(GeneralDamageMultiplier);
            projectile.Setup(Owner, Target);
        }
        return projectile;
    }

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < projectilePrefabs.Length; i++)
        {
            PoolDispatcher.Instance.RequestPool(Layers.PROJECTILES, projectilePrefabs[i], 25);
        }
    }
}