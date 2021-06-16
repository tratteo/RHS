// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Projectile.cs
//
// All Rights Reserved

using GibFrame.ObjectPooling;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour, IPooledObject, IEffectBearer
{
    protected List<StatusEffect> onHitEffects;
    protected List<string> collideTagExceptions;
    [Header("Projectile")]
    [SerializeField] private float baseDamage = 1F;
    private float counterAttackDamageMultiplier = 1F;
    private bool critical = false;
    private float criticalMultiplier;
    private float damage;

    public bool Grounded { get; private set; }

    protected SpriteRenderer Renderer { get; private set; }

    protected Collider2D Collider { get; private set; } = null;

    protected Rigidbody2D Rigidbody { get; private set; } = null;

    public void AddEffects(params StatusEffect[] effects)
    {
        onHitEffects.AddRange(effects);
    }

    public string GetActionLayer()
    {
        if (gameObject.layer.Equals(LayerMask.NameToLayer(Layers.ENEMY_PROJECTILES)))
        {
            return Layers.FRIENDLIES;
        }
        else if (gameObject.layer.Equals(LayerMask.NameToLayer(Layers.PROJECTILES)))
        {
            return Layers.HOSTILES;
        }
        return "";
    }

    public void SetCounterAttack(float counterAttackDamageMultiplier = 2F)
    {
        this.counterAttackDamageMultiplier = counterAttackDamageMultiplier;
    }

    public void SetupLayer(IAgent spawner)
    {
        if (spawner.GetFactionRelation() == IAgent.FactionRelation.HOSTILE)
        {
            gameObject.layer = LayerMask.NameToLayer(Layers.ENEMY_PROJECTILES);
        }
        else if (spawner.GetFactionRelation() == IAgent.FactionRelation.FRIENDLY)
        {
            gameObject.layer = LayerMask.NameToLayer(Layers.PROJECTILES);
        }
        //Collider.enabled = true;
    }

    public void ClearTagExceptions()
    {
        collideTagExceptions.Clear();
    }

    public void DamageMultiplier(float multiplier)
    {
        damage *= multiplier;
    }

    public virtual void OnObjectSpawn()
    {
        Renderer.enabled = true;
        Rigidbody.velocity = Vector3.zero;
        collideTagExceptions.Clear();
        onHitEffects.Clear();
        damage = baseDamage;
        critical = false;
        criticalMultiplier = 1F;
    }

    public void AddTagException(string tag)
    {
        if (!collideTagExceptions.Contains(tag))
        {
            collideTagExceptions.Add(tag);
        }
    }

    protected virtual void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        Renderer = GetComponent<SpriteRenderer>();
        onHitEffects = new List<StatusEffect>();
        collideTagExceptions = new List<string>();
        damage = baseDamage;
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collideTagExceptions.Contains(collision.collider.tag))
        {
            IHealthHolder healthHolder;
            if ((healthHolder = collision.gameObject.GetComponent<IHealthHolder>()) != null)
            {
                healthHolder.Damage(GetDamage());
            }
            IEffectsReceiverHolder effectReceiver;
            if ((effectReceiver = collision.gameObject.GetComponent<IEffectsReceiverHolder>()) != null)
            {
                effectReceiver.GetEffectsReceiver().AddEffects(onHitEffects.ToArray());
            }
            gameObject.SetActive(false);
        }
    }

    protected float GetDamage()
    {
        float damage = critical ? this.damage * criticalMultiplier * counterAttackDamageMultiplier : this.damage * counterAttackDamageMultiplier;
        counterAttackDamageMultiplier = 1F;
        return damage;
    }
}