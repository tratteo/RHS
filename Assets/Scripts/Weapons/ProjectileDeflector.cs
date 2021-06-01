// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : ProjectileDeflector.cs
//
// All Rights Reserved

using UnityEngine;

public class ProjectileDeflector : MonoBehaviour
{
    [SerializeField] private float deflectThresholdTime = 0.3F;
    private int deflectTicks;
    private PolygonCollider2D deflectCollider;

    public IAgent Owner { get; private set; }

    public void StartDeflecting(Vector2[] polygonPoints)
    {
        deflectCollider.SetPath(0, polygonPoints);
        deflectCollider.enabled = true;
        deflectTicks = Mathf.RoundToInt(deflectThresholdTime / Time.fixedDeltaTime);
    }

    public void Setup(IAgent owner)
    {
        Owner = owner;
    }

    private void Awake()
    {
        deflectCollider = GetComponent<PolygonCollider2D>();
    }

    private void EndDeflect()
    {
        deflectCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (deflectTicks > 0 && Owner != null)
        {
            IDeflectable deflectable;
            if ((deflectable = other.gameObject.GetComponent<IDeflectable>()) != null)
            {
                if (deflectable.CanBeDeflected)
                {
                    deflectable.Deflect(Owner);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (deflectTicks > 0)
        {
            deflectTicks--;
        }
        else
        {
            EndDeflect();
        }
    }
}