// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : SwingDirectionalProjectileBossAbility.cs
//
// All Rights Reserved

using GibFrame;
using System.Collections;
using UnityEngine;

public class SwingDirectionalProjectileBossAbility : Ability<BossEnemy>
{
    [SerializeField, Guarded] private GameObject projectilePrefab;
    [SerializeField] private float duration = 2F;
    [SerializeField] private float delay = 1F;
    [SerializeField] private float stride = 40F;
    [SerializeField] private float swingSpeed = 1F;

    protected override IEnumerator Execute_C()
    {
        Vector3 initialAxis = Parent.BattleContext.Transform.position - Parent.transform.position;
        WaitForFixedUpdate wait = new WaitForFixedUpdate();
        float currentTime = 0F;
        float shootDelay = 0F;

        while ((currentTime += Time.fixedDeltaTime) < duration)
        {
            shootDelay += Time.fixedDeltaTime;
            if (shootDelay >= delay)
            {
                shootDelay = 0F;
                float angle = (Mathf.PingPong(swingSpeed * Time.time, 1F) - 0.5F) * stride * 4F;
                initialAxis = Parent.BattleContext.Transform.position - Parent.transform.position;
                Vector3 axis = Quaternion.AngleAxis(angle, Vector3.forward) * initialAxis;
                Projectile.Create(projectilePrefab.name, Parent.transform.position, Quaternion.LookRotation(Vector3.forward, Quaternion.AngleAxis(90F, Vector3.forward) * axis), Parent, Parent.BattleContext.Transform);
                Debug.DrawRay(Parent.transform.position, axis, Color.red, delay);
            }
            yield return wait;
        }
        Complete();
    }

    private void Awake()
    {
        PoolDispatcher.Instance.RequestPool(Categories.PROJECTILES, projectilePrefab, (int)(duration * (1F / delay) * 2));
    }
}