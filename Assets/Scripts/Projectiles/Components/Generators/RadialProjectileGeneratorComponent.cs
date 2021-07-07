// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : RadialProjectileGeneratorComponent.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

public class RadialProjectileGeneratorComponent : ProjectileGeneratorComponent
{
    [Header("Radial")]
    [SerializeField] private bool randomizeStartAxis = false;
    [SerializeField] private RandomizedInt amount;
    [SerializeField] private float force = 1F;

    protected override void Generate()
    {
        int amount = this.amount;
        Vector2 axis = randomizeStartAxis ? new Vector2(Random.Range(-1F, 1F), Random.Range(-1F, 1F)).normalized : (Vector2)Parent.transform.right;
        float angleStride = 360F / amount;
        for (int i = 0; i < amount; i++)
        {
            Projectile projectile = Parent.Generate(Prefab.name, Parent.transform.position, Parent.transform.rotation);
            projectile.DelegateLaunch(force);
            projectile.transform.right = axis;
            axis = Quaternion.AngleAxis(angleStride, Vector3.forward) * axis;
        }
    }
}