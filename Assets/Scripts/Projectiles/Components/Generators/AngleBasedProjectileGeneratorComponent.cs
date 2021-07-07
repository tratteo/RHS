// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : AngleBasedProjectileGeneratorComponent.cs
//
// All Rights Reserved

using System.Collections.Generic;
using UnityEngine;

public class AngleBasedProjectileGeneratorComponent : ProjectileGeneratorComponent
{
    [SerializeField] private float force = 1F;
    [SerializeField] private List<float> angles;

    protected override void Generate()
    {
        for (int i = 0; i < angles.Count; i++)
        {
            Vector2 axis = Parent.transform.right;
            axis = Quaternion.AngleAxis(angles[i], Vector3.forward) * axis;
            Projectile projectile = Parent.Generate(Prefab.name, Parent.transform.position, Parent.transform.rotation);
            projectile.transform.right = axis;
            projectile.DelegateLaunch(force);
        }
    }
}