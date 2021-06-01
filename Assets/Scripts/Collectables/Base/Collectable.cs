// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Collectable.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

public abstract class Collectable<Target> : MonoBehaviour, ICollectable<Target> where Target : class
{
    public const int COLLECT_TIMEOUT = 20;
    private new Rigidbody rigidbody;
    private float randomizerSeed;

    public bool Grounded { get; private set; }

    public virtual void CollectAndApply(Target target)
    {
        Destroy(gameObject);
    }

    public void CollectAndApply(object target)
    {
        if (target is Target)
        {
            CollectAndApply(target as Target);
        }
    }

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        new Timer(this, COLLECT_TIMEOUT, false, true, new Callback(Despawn));
        randomizerSeed = UnityEngine.Random.Range(-Mathf.PI, Mathf.PI);
    }

    private void FixedUpdate()
    {
        if (!Grounded && Physics.Raycast(transform.position, Vector3.down, 0.85F/*, LayerMask.GetMask(Global.Layers.ENVIROMENT_LAYER)*/))
        {
            Grounded = true;
            rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0F, rigidbody.velocity.z);
        }
        if (Grounded)
        {
            transform.Translate(Vector3.up * 0.0175F * Mathf.Sin(4.5F * (Time.timeSinceLevelLoad + randomizerSeed)));
            transform.Rotate(Vector3.up, 2F);
        }
        else
        {
            rigidbody.AddForce(Physics.gravity * 4F, ForceMode.Acceleration);
        }
    }

    private void Despawn()
    {
        Destroy(gameObject);
    }
}