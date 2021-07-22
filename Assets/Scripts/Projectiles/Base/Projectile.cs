// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Projectile.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.ObjectPooling;
using GibFrame.Performance;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour, IPooledObject, ICommonUpdate, IDeflectable, ICommonFixedUpdate
{
    [Header("Projectile")]
    [SerializeField] private float baseDamage = 1F;
    [Header("Kinematic")]
    [SerializeField] private float speed = 1F;
    [Range(0F, 0.5F)] [SerializeField] private float homingRatio = 0F;
    [SerializeField] private bool torqueOnLaunch = false;
    [SerializeField] private RandomizedFloat randomTorque;
    [Header("Deflect")]
    [SerializeField] private bool isDeflectable = false;
    [SerializeField] private float deflectForce = 1F;

    [Header("FX")]
    [SerializeField] private ParticleSystem onDestroyedEffect;

    [Header("Life span")]
    [SerializeField] private bool hasLifeSpan = false;
    [SerializeField] private RandomizedFloat lifeSpan;

    [Header("Status effects")]
    [SerializeField] private List<GameObject> statusEffectsPrefabs;
    private List<StatusEffect> statusEffects;
    private float counterAttackDamageMultiplier = 1F;
    private bool critical = false;
    private float criticalMultiplier;
    private float damage;
    private float currentLifeSpan = 0F;
    private bool destroyed = false;

    public IAgent Owner { get; private set; }

    public Transform Target { get; private set; }

    public Rigidbody2D Rigidbody { get; private set; } = null;

    protected SpriteRenderer Renderer { get; private set; }

    protected Collider2D Collider { get; private set; } = null;

    public event Action OnDestroy = delegate { };

    public static Projectile Create(string prefabName, Vector2 position, Quaternion rotation, IAgent owner, Transform target, float param = 1F)
    {
        GameObject obj = PoolManager.Instance.Spawn(Categories.PROJECTILES, prefabName, position, rotation);
        Projectile projectile = obj.GetComponent<Projectile>();
        if (!projectile)
        {
            Debug.LogError("Trying to instantiate a non projectile as a projectile");
        }
        else
        {
            projectile.Setup(owner, target);
            projectile.DelegateLaunch(param);
            return projectile;
        }
        return null;
    }

    public Projectile Generate(string childId, Vector2 position, Quaternion rotation)
    {
        GameObject obj = PoolManager.Instance.Spawn(Layers.PROJECTILES, childId, position, rotation);
        obj.layer = gameObject.layer;
        Projectile projectile = obj.GetComponent<Projectile>();
        projectile.Setup(Owner, Target);
        return projectile;
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

    public virtual void DelegateLaunch(float param = 1F)
    {
        Rigidbody.AddForce((Rigidbody.drag + 1F) * speed * param * transform.right, UnityEngine.ForceMode2D.Impulse);
        if (torqueOnLaunch)
        {
            Rigidbody.AddTorque(randomTorque * Mathf.Deg2Rad * Rigidbody.inertia, ForceMode2D.Impulse);
        }
    }

    public void Setup(IAgent owner, Transform target)
    {
        Owner = owner;
        Target = target;
        if (owner.GetFactionRelation() == IAgent.FactionRelation.HOSTILE)
        {
            gameObject.layer = LayerMask.NameToLayer(Layers.ENEMY_PROJECTILES);
        }
        else if (owner.GetFactionRelation() == IAgent.FactionRelation.FRIENDLY)
        {
            gameObject.layer = LayerMask.NameToLayer(Layers.PROJECTILES);
        }
        //Collider.enabled = true;
    }

    public void DamageMultiplier(float multiplier)
    {
        damage *= multiplier;
    }

    public virtual void OnObjectSpawn()
    {
        Renderer.enabled = true;
        Rigidbody.velocity = Vector3.zero;
        damage = baseDamage;
        critical = false;
        criticalMultiplier = 1F;
        if (hasLifeSpan)
        {
            currentLifeSpan = lifeSpan;
        }
        destroyed = false;
    }

    public void CommonUpdate(float deltaTime)
    {
        if (currentLifeSpan > 0)
        {
            currentLifeSpan -= deltaTime;
            if (currentLifeSpan <= 0)
            {
                currentLifeSpan = 0;
                Destroy();
            }
        }
    }

    public void Deflect(IAgent agent)
    {
        if (isDeflectable)
        {
            DeflectBehaviour(agent);
        }
    }

    public void CommonFixedUpdate(float fixedDeltaTime)
    {
        if (Target)
        {
            Rigidbody.velocity = Vector2.Lerp(Rigidbody.velocity, Rigidbody.velocity.magnitude * (Target.position - transform.position).normalized * speed, homingRatio);
        }
    }

    protected virtual void DeflectBehaviour(IAgent agent)
    {
        Owner = agent;
        Target = null;
        Rigidbody.velocity = -Rigidbody.velocity * deflectForce;
    }

    protected virtual void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
        Renderer = GetComponent<SpriteRenderer>();
        damage = baseDamage;

        statusEffects = new List<StatusEffect>();
        foreach (GameObject obj in statusEffectsPrefabs)
        {
            StatusEffect effect = obj.GetComponent<StatusEffect>();
            if (effect)
            {
                statusEffects.Add(effect);
            }
        }
        foreach (StatusEffect effect in statusEffects)
        {
            PoolDispatcher.Instance.RequestPool(Categories.STATUS_EFFECTS, effect.gameObject, 1);
        }
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        IHealthHolder healthHolder;
        if ((healthHolder = collision.gameObject.GetComponent<IHealthHolder>()) != null)
        {
            healthHolder.Damage(new IHealthHolder.Data(gameObject, GetDamage()));
        }
        ApplyStatusEffects(collision.gameObject);
        Destroy();
    }

    protected void ApplyStatusEffects(GameObject target)
    {
        foreach (StatusEffect effect in statusEffects)
        {
            effect.ApplyTo(target.transform);
        }
    }

    protected float GetDamage()
    {
        float damage = critical ? this.damage * criticalMultiplier * counterAttackDamageMultiplier : this.damage * counterAttackDamageMultiplier;
        counterAttackDamageMultiplier = 1F;
        return damage;
    }

    protected virtual void OnDestroyed()
    {
        OnDestroy?.Invoke();
        OnDestroy = null;
    }

    protected virtual void OnDestroyEffectPlay(ParticleSystem destroyEffect)
    {
        onDestroyedEffect.Play();
    }

    protected void Destroy()
    {
        if (destroyed) return;
        destroyed = true;
        OnDestroyed();
        if (onDestroyedEffect)
        {
            OnDestroyEffectPlay(onDestroyedEffect);
            new Timer(this, onDestroyedEffect.main.duration, false, true, new Callback(() => gameObject.SetActive(false)));
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        CommonUpdateManager.Register(this);
    }

    private void OnDisable()
    {
        CommonUpdateManager.Unregister(this);
    }
}