// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Homing.cs
//
// All Rights Reserved

using UnityEngine;

public class Homing : Projectile
{
    [SerializeField] private float speed = 12F;
    private Transform target = null;
    private Vector3 offset;

    /// <summary>
    ///   Set the target to be homed
    /// </summary>
    /// <param name="target"> </param>
    public void SetTarget(Transform target, Vector3 offset = default)
    {
        this.target = target;
        this.offset = offset;
    }

    protected void FixedUpdate()
    {
        if (target != null)
        {
            transform.Translate((target.position + offset - transform.position) * speed * Time.fixedDeltaTime);
            if (Vector3.SqrMagnitude(target.position - transform.position) <= 5F)
            {
                Rigidbody.AddForce(Vector3.down * 80F * Time.fixedDeltaTime, ForceMode2D.Force);
            }
        }
    }
}