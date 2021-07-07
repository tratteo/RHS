// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Mine.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

public class Mine : Projectile
{
    private readonly Collider2D[] buf = new Collider2D[8];
    [Header("Channels")]
    [SerializeField, Guarded] private CameraShakeEventBus cameraShakeChannel;
    [Header("Parameters")]
    [SerializeField] private float explosionRadius = 6F;
    [SerializeField] private float explosionForce = 2500F;

    protected override void OnDestroyEffectPlay(ParticleSystem destroyEffect)
    {
        base.OnDestroyEffectPlay(destroyEffect);
        destroyEffect.transform.localScale = Vector3.one * explosionRadius;
    }

    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        cameraShakeChannel.Broadcast(new CameraShakeEventBus.Shake(CameraShakeEventBus.HEAVY_EXPLOSION, transform.position));

        int amount = Physics2D.OverlapCircleNonAlloc(transform.position, explosionRadius, buf, LayerMask.GetMask(GetActionLayer()));
        for (int i = 0; i < amount; i++)
        {
            buf[i].gameObject.AddForce2D((buf[i].transform.position - transform.position).normalized * explosionForce);
            IHealthHolder healthHolder;
            if ((healthHolder = buf[i].gameObject.GetComponent<IHealthHolder>()) != null)
            {
                healthHolder.Damage(new IHealthHolder.Data(gameObject, GetDamage()));
            }
            IEffectsReceiverHolder effectReceiver;
            if ((effectReceiver = buf[i].gameObject.GetComponent<IEffectsReceiverHolder>()) != null)
            {
                effectReceiver.GetEffectsReceiver().AddEffects(onHitEffects.ToArray());
            }
        }

        Renderer.enabled = false;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}