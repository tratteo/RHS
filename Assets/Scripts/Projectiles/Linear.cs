// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Linear.cs
//
// All Rights Reserved

using UnityEngine;

public class Linear : Projectile, IDeflectable
{
    public bool canBeDeflected = true;
    [SerializeField] private float speed = 10F;

    public void Deflect(IAgent agent)
    {
        if (!canBeDeflected) return;
        if (agent.GetFactionRelation() == IAgent.FactionRelation.HOSTILE)
        {
            gameObject.layer = LayerMask.NameToLayer(Layers.ENEMY_PROJECTILES);
        }
        else if (agent.GetFactionRelation() == IAgent.FactionRelation.FRIENDLY)
        {
            gameObject.layer = LayerMask.NameToLayer(Layers.PROJECTILES);
        }

        Rigidbody.velocity = -Rigidbody.velocity;
        canBeDeflected = false;
    }

    public override void OnObjectSpawn()
    {
        base.OnObjectSpawn();
        Rigidbody.velocity = Vector3.zero;
        Rigidbody.AddForce(transform.right * speed, ForceMode2D.Impulse);
    }
}