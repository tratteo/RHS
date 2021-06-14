// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Mine.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

public class Mine : Projectile
{
    [Header("FX")]
    [SerializeField] protected ParticleSystem explosionEffect;
    private readonly Collider2D[] buf = new Collider2D[8];
    [Header("Channels")]
    [SerializeField, Guarded] private CameraShakeEventBus cameraShakeChannel;
    [Header("Parameters")]
    [SerializeField] private float explosionRadius = 6F;
    [SerializeField] private float explosionForce = 2500F;

    public void Detonate()
    {
        cameraShakeChannel.Broadcast(new CameraShakeEventBus.Shake(CameraShakeEventBus.HEAVY_EXPLOSION, transform.position));

        int amount = Physics2D.OverlapCircleNonAlloc(transform.position, explosionRadius, buf, LayerMask.GetMask(GetActionLayer()));
        for (int i = 0; i < amount; i++)
        {
            buf[i].gameObject.AddForce2D((buf[i].transform.position - transform.position).normalized * explosionForce);
            IHealthHolder healthHolder;
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
        if (explosionEffect)
        {
            explosionEffect.Play();
            explosionEffect.transform.localScale = Vector3.one * explosionRadius;
        }
        Renderer.enabled = false;
        new Timer(this, explosionEffect.main.duration, false, true, new Callback(() => gameObject.SetActive(false)));
        //gameObject.SetActive(false);
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collideTagExceptions.Contains(collision.collider.tag))
        {
            Detonate();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}