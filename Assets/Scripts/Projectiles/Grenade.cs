// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Grenade.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

public class Grenade : Projectile
{
    [Header("Grenade")]
    [SerializeField, Guarded] protected CameraShakeEventBus cameraShakeChannel;
    private readonly Collider2D[] buf = new Collider2D[8];
    [SerializeField] private float explosionRadius = 4F;

    protected override void OnDestroyEffectPlay(ParticleSystem destroyEffect)
    {
        base.OnDestroyEffectPlay(destroyEffect);
        destroyEffect.transform.localScale = Vector3.one * explosionRadius;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        Destroy();
    }

    protected override void OnDestroyed()
    {
        base.OnDestroyed();
        cameraShakeChannel.Broadcast(new CameraShakeEventBus.Shake(CameraShakeEventBus.LIGHT_EXPLOSION, transform.position));
        int amount = Physics2D.OverlapCircleNonAlloc(transform.position, explosionRadius, buf, LayerMask.GetMask(GetActionLayer()));

        for (int i = 0; i < amount; i++)
        {
            if (!buf[i].gameObject.Equals(gameObject))
            {
                IHealthHolder healthHolder;
                if ((healthHolder = buf[i].gameObject.GetComponent<IHealthHolder>()) != null)
                {
                    healthHolder.Damage(new IHealthHolder.Data(gameObject, GetDamage()));
                }
            }
        }
        Collider.enabled = false;
        Renderer.enabled = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}