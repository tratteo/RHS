// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : StunExplosionBossAbility.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.Debug;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaExplosionBossAbility : Ability<BossEnemy>
{
    private readonly Collider2D[] buf = new Collider2D[8];
    [Header("Channels")]
    [SerializeField, Guarded] private CameraShakeEventBus cameraShakeChannel;
    [SerializeField] private float baseDamage = 350F;
    [SerializeField] private float channelTime = 1.25F;
    [SerializeField] private float explosionRadius = 8F;
    [SerializeField] private List<GameObject> statusEffectsPrefabs;
    [SerializeField] private RandomizedFloat recoverDuration;
    [SerializeField, Guarded] private GameObject rangeIndicator;
    [SerializeField, Guarded] private ParticleSystem explosionEffect;

    private List<StatusEffect> effects;
    private IHealthHolder healthHolder;

    public override bool CanPerform()
    {
        return base.CanPerform() && Vector2.Distance(Parent.BattleContext.Transform.position, Parent.transform.position) < explosionRadius * 0.8F;
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
        rangeIndicator.SetActive(false);
        int amount = Physics2D.OverlapCircleNonAlloc(transform.position, explosionRadius, buf, ~LayerMask.GetMask(Layers.HOSTILES));
        explosionEffect.Play();
        explosionEffect.transform.localScale = Vector3.one * explosionRadius;
        GDebug.DrawWireSphere(transform.position, explosionRadius, Color.green, 1F, 3);
        for (int i = 0; i < amount; i++)
        {
            effects.ForEach(e => e.ApplyTo(buf[i].transform));

            if ((healthHolder = buf[i].gameObject.GetComponent<IHealthHolder>()) != null)
            {
                healthHolder.Damage(new IHealthHolder.Data(gameObject, baseDamage));
            }
        }
        Parent.SetInteraction(Assets.Sprites.Stun);
        yield return new WaitForSeconds(recoverDuration);
        Parent.SetInteraction();
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
        effects = new List<StatusEffect>();
        foreach (GameObject obj in statusEffectsPrefabs)
        {
            StatusEffect effect = obj.GetComponent<StatusEffect>();
            if (effect)
            {
                effects.Add(effect);
                PoolDispatcher.Instance.RequestPool(Categories.STATUS_EFFECTS, obj, 1);
            }
        }
    }
}