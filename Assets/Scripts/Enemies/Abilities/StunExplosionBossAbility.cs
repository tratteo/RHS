// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : AreaStunBossAbility.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.Debug;
using System.Collections;
using UnityEngine;

public class StunExplosionBossAbility : Ability<BossEnemy>
{
    private readonly Collider2D[] buf = new Collider2D[8];
    [Header("Channels")]
    [SerializeField, Guarded] private CameraShakeEventBus cameraShakeChannel;
    [SerializeField] private float baseDamage = 350F;
    [SerializeField] private float channelTime = 1.25F;
    [SerializeField] private float explosionRadius = 8F;
    [SerializeField] private float stunDuration = 2F;
    [SerializeField, Guarded] private GameObject rangeIndicator;
    [SerializeField, Guarded] private ParticleSystem explosionEffect;

    private IHealthHolder healthHolder;

    public override bool CanPerform()
    {
        return base.CanPerform() && Vector2.Distance(Parent.TargetContext.Transform.position, Parent.transform.position) < explosionRadius * 0.8F;
    }

    protected override IEnumerator Execute_C()
    {
        rangeIndicator.transform.position = Parent.transform.position;
        rangeIndicator.transform.localScale = Vector3.one * explosionRadius;
        rangeIndicator.SetActive(true);
        Parent.Move(Vector2.zero);
        Parent.SetInteraction(Assets.Sprites.Exclamation, Color.red);
        yield return new WaitForSeconds(channelTime);
        Parent.SetInteraction();
        cameraShakeChannel.Broadcast(new CameraShakeEventBus.Shake(CameraShakeEventBus.HUGE_EXPLOSION, Parent.transform.position));
        int amount = Physics2D.OverlapCircleNonAlloc(transform.position, explosionRadius, buf, ~LayerMask.GetMask(Layers.HOSTILES));
        explosionEffect.Play();
        explosionEffect.transform.localScale = Vector3.one * explosionRadius;
        GDebug.DrawWireSphere(transform.position, explosionRadius, Color.green, 1F, 3);
        for (int i = 0; i < amount; i++)
        {
            IStunnable stunnable;
            if ((stunnable = buf[i].gameObject.GetComponent<IStunnable>()) != null)
            {
                stunnable.Stun(stunDuration);
            }

            if ((healthHolder = buf[i].gameObject.GetComponent<IHealthHolder>()) != null)
            {
                healthHolder.Damage(baseDamage);
            }
        }
        rangeIndicator.SetActive(false);
        Complete();
    }

    protected override void OnStopped()
    {
        base.OnStopped();
        rangeIndicator.SetActive(false);
        Parent.SetInteraction();
    }

    private void Awake()
    {
        rangeIndicator.GetComponent<SpriteRenderer>().color = Color.red;
        rangeIndicator.SetActive(false);
    }
}