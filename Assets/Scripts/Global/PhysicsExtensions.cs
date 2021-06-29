// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : PhysicsExtensions.cs
//
// All Rights Reserved

using UnityEngine;

public static class PhysicsExtensions
{
    public static void AddForce2D(this GameObject obj, Vector3 force)
    {
        Rigidbody2D rigidbody2D = obj.GetComponent<Rigidbody2D>();
        if (!rigidbody2D) return;
        IManagedRigidbody managedRigidbody;
        if ((managedRigidbody = obj.GetComponent<IManagedRigidbody>()) != null)
        {
            if (!Mathf.Approximately(rigidbody2D.mass, 0F))
            {
                force /= rigidbody2D.mass;
            }
            managedRigidbody.AddExternalForce(force);
        }
        else
        {
            if (!Mathf.Approximately(rigidbody2D.mass, 0F))
            {
                force *= rigidbody2D.mass;
            }
            rigidbody2D.AddForce(force);
        }
    }
}