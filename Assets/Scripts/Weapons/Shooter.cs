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
    [SerializeField] private GameObject projectilePrefab;

    public void TriggerShoot()
    {
        // Debug.Log(firePoint == null);
        GameObject obj = PoolManager.Instance.Spawn(Layers.PROJECTILES, projectilePrefab.name, firePoint.position, firePoint.rotation);
        Projectile projectile = obj.GetComponent<Projectile>();
        if (!projectile)
        {
            Debug.LogError("Instantiated a non projectile from a shooter!");
        }
        else
        {
            projectile.SetDamage(GetDamage());
            if (Owner.GetFactionRelation() == IAgent.FactionRelation.HOSTILE)
            {
                projectile.gameObject.layer = LayerMask.NameToLayer(Layers.ENEMY_PROJECTILES);
            }
            else if (Owner.GetFactionRelation() == IAgent.FactionRelation.FRIENDLY)
            {
                projectile.gameObject.layer = LayerMask.NameToLayer(Layers.PROJECTILES);
            }
        }
    }

    protected override void Start()
    {
        base.Start();
        InitializePool();
    }

    private void InitializePool()
    {
        PoolCategory category = PoolManager.Instance.GetCategory(Layers.PROJECTILES);
        if (category == null)
        {
            category = new PoolCategory(Layers.PROJECTILES);
        }
        Pool pool = category.GetPool(projectilePrefab.name);
        if (pool == null)
        {
            pool = new Pool(projectilePrefab.name, projectilePrefab, 50);
            category.AddPool(pool);
            PoolManager.Instance.AddCategory(category);
        }
    }
}