// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : EnemyMinion.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.ObjectPooling;
using GibFrame.Performance;
using System;
using UnityEngine;

public abstract class EnemyMinion : MonoBehaviour, ICommonUpdate, ICommonFixedUpdate, IElementOfInterest, IAgent, IPooledObject
{
    [Header("Bus")]
    [SerializeField, Guarded] private IndicatorEventBus indicatorEventBus;

    public float ThresholdDistance => 10F;

    protected BossEnemy Owner { get; private set; }

    public event Action OnDeath = delegate { };

    public void CommonFixedUpdate(float fixedDeltaTime)

    {
    }

    public void CommonUpdate(float deltaTime)
    {
    }

    public void SetOwner(BossEnemy enemy)
    {
        Owner = enemy;
    }

    public IAgent.FactionRelation GetFactionRelation() => IAgent.FactionRelation.HOSTILE;

    public IElementOfInterest.InterestPriority GetInterestPriority() => IElementOfInterest.InterestPriority.MANDATORY;

    public Vector3 GetSightPoint() => transform.position;

    public virtual void OnDie()
    {
        Owner = null;
        OnDeath?.Invoke();
        indicatorEventBus.Broadcast(new IndicatorEventBus.IndicatorData(transform, false));
        gameObject.SetActive(false);
    }

    public void OnObjectSpawn()
    {
        indicatorEventBus.Broadcast(new IndicatorEventBus.IndicatorData(transform, true, IndicatorsGUI.IndicatorType.MINION));
    }

    private void OnDisable()
    {
        CommonUpdateManager.Unregister(this);
    }

    private void OnEnable()
    {
        CommonUpdateManager.Register(this);
    }
}