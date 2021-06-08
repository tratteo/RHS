// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Grenade.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.Performance;
using UnityEngine;

public class Grenade : Projectile, ICommonUpdate, IDeflectable, ICommonFixedUpdate
{
    [Header("Channels")]
    [SerializeField, Guarded] protected CameraShakeChannelEvent cameraShakeChannel;
    [Header("FX")]
    [SerializeField] protected ParticleSystem explosionEffect;
    private readonly Collider2D[] buf = new Collider2D[8];
    private bool canBeDeflected = true;
    [Header("Parameters")]
    [SerializeField] private RandomizedFloat detonationTime;
    [SerializeField] private float explosionRadius = 4F;
    [Header("Movement")]
    [SerializeField] private bool torqueOnLaunch = false;
    [SerializeField] private RandomizedFloat randomTorque;
    private float detonationTimer = 0F;
    private bool detonated = false;

    private IHealthHolder healthHolder;

    public override void OnObjectSpawn()
    {
        base.OnObjectSpawn();
        detonated = false;
        detonationTimer = 0F;
    }

    public void CommonUpdate(float deltaTime)
    {
        if (detonationTimer > 0)
        {
            detonationTimer -= Time.deltaTime;
            if (detonationTimer <= 0)
            {
                detonationTimer = 0;
                Detonate();
            }
        }
    }

    public void Launch(float force)
    {
        Rigidbody.AddForce((Rigidbody.drag + 1F) * force * transform.right, UnityEngine.ForceMode2D.Impulse);
        detonationTimer = detonationTime;
        if (torqueOnLaunch)
        {
            Rigidbody.AddTorque(randomTorque * Mathf.Deg2Rad * Rigidbody.inertia, ForceMode2D.Impulse);
        }
    }

    public virtual void Deflect(IAgent agent)
    {
        if (!canBeDeflected) return;
        //if (agent.GetFactionRelation() == IAgent.FactionRelation.HOSTILE)
        //{
        //    gameObject.layer = LayerMask.NameToLayer(Layers.ENEMY_PROJECTILES);
        //}
        //else if (agent.GetFactionRelation() == IAgent.FactionRelation.FRIENDLY)
        //{
        //    gameObject.layer = LayerMask.NameToLayer(Layers.PROJECTILES);
        //}

        Rigidbody.velocity = -Rigidbody.velocity;
        canBeDeflected = false;
    }

    public void CommonFixedUpdate(float fixedDeltaTime)
    {
        //Rigidbody.AddTorque(20F, ForceMode2D.Force);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collideTagExceptions.Contains(collision.collider.tag))
        {
            Detonate();
        }
    }

    protected virtual void Detonate()
    {
        if (detonated) return;
        detonated = true;
        cameraShakeChannel.Broadcast(new CameraShakeChannelEvent.Shake(CameraShakeChannelEvent.EXPLOSION, transform.position));
        int amount = Physics2D.OverlapCircleNonAlloc(transform.position, explosionRadius, buf, GetActionLayer());
        if (explosionEffect)
        {
            explosionEffect.Play();
            explosionEffect.transform.localScale = Vector3.one * explosionRadius;
        }

        for (int i = 0; i < amount; i++)
        {
            if ((healthHolder = buf[i].gameObject.GetComponent<IHealthHolder>()) != null)
            {
                healthHolder.Damage(GetDamage());
            }
            IEffectsReceiverHolder effectReceiver;
            if ((effectReceiver = buf[i].gameObject.GetComponent<IEffectsReceiverHolder>()) != null)
            {
                effectReceiver.GetEffectsReceiver().AddEffects(onHitEffects.ToArray());
            }
        }
        Collider.enabled = false;
        Renderer.enabled = false;
        new Timer(this, explosionEffect.main.duration, false, true, new Callback(() => gameObject.SetActive(false)));
    }

    private void OnEnable()
    {
        CommonUpdateManager.Register(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

    private void OnDisable()
    {
        CommonUpdateManager.Unregister(this);
    }
}