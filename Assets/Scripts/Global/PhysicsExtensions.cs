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
        Rigidbody2D rigidbody2D;
        IManagedRigidbody managedRigidbody;
        if ((managedRigidbody = obj.GetComponent<IManagedRigidbody>()) != null)
        {
            managedRigidbody.AddExternalForce(force);
        }
        else if ((rigidbody2D = obj.GetComponent<Rigidbody2D>()) != null)
        {
            if (!Mathf.Approximately(rigidbody2D.mass, 0F))
            {
                force *= rigidbody2D.mass;
            }
            rigidbody2D.AddForce(force);
        }
    }
}