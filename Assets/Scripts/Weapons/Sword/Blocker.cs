// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Blocker.cs
//
// All Rights Reserved

using UnityEngine;

public class Blocker : MonoBehaviour
{
    private new Collider2D collider;
    private Sword swordParent;
    private float blockProbability;

    public bool Active { get; private set; }

    public void EnableBlock(float probability = -1F)
    {
        collider.enabled = true;
        Active = true;
        blockProbability = probability > 0F ? probability : blockProbability;
    }

    public void DisableBlock()
    {
        collider.enabled = false;
        Active = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.Equals(gameObject) || swordParent.IsSlashing) return;
        Sword clasher;
        if (clasher = collision.GetComponent<Sword>())
        {
            if (UnityEngine.Random.value < blockProbability)
            {
                if (clasher.LastSlash.IsParriable)
                {
                    clasher.BlockedBy(swordParent);
                    swordParent.HitRotate();
                }
            }
        }
    }

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        swordParent = GetComponentInParent<Sword>();
        if (!swordParent)
        {
            Debug.LogError("Blocker has not found the parent sword");
        }
    }
}