// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : BossEnemy.cs
//
// All Rights Reserved

using GibFrame;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossEnemy : Enemy, IWeaponOwner, IStatisticsProvider
{
    [Header("Boss")]
    [SerializeField, Guarded] private Weapon weapon;
    [SerializeField, Guarded] private Animator stateMachine;
    [SerializeField, Guarded] private Transform phaseIndicatorsParent;
    [SerializeField, Guarded] private string idleTransitionId = "idle";
    [SerializeField] private int phasesAmount;

    [Header("Animation")]
    [SerializeField] private bool hasMirror = false;
    [SerializeField, Guarded] private GameObject phaseIndicatorPrefab;
    [Header("Bus")]
    [SerializeField] private CameraTargetGroupEventBus targetGroupEventBus;
    [SerializeField] private TransformEventBus indicatorEventBus;
    private List<Statistic> intermediateStatistics;
    private int currentPhase = 0;

    private Image[] phaseIndicators;

    protected AnimatorDriver AnimatorDriver { get; private set; }

    public override Weapon GetWeapon() => weapon;

    public void NotifyStatistic(int hash, Func<object, object> UpdateAction)
    {
        Statistic stat = intermediateStatistics.Find(s => s.Hash.Equals(hash));
        if (stat != null)
        {
            stat.Value = UpdateAction(stat.Value);
        }
        else
        {
            stat = new Statistic(hash, UpdateAction(default));
            intermediateStatistics.Add(stat);
        }
    }

    public override void CommonFixedUpdate(float fixedDeltaTime)
    {
        base.CommonFixedUpdate(fixedDeltaTime);
        if (IsDashing)
        {
        }
        else
        {
            if (Mathf.Approximately(TargetVelocity.magnitude, 0F))
            {
                Animate(AnimatorDriver.IDLE);
            }
            else
            {
                Animate(AnimatorDriver.RUN, GetCombinedSign(Rigidbody.velocity));
            }
        }
    }

    public float GetCombinedSign(Vector3 vector)
    {
        return Mathf.Sign(transform.localScale.x * vector.x);
    }

    public Statistic[] GetStats()
    {
        return intermediateStatistics.ToArray();
    }

    public override void CommonUpdate(float deltaTime)
    {
        base.CommonUpdate(deltaTime);
        Vector2 viewPos = Camera.main.WorldToViewportPoint(transform.position);
        if (viewPos.x > 0F && viewPos.x < 1F && viewPos.y > 0F && viewPos.y < 1F)
        {
            targetGroupEventBus.Broadcast(transform, true);
        }
        else
        {
            targetGroupEventBus.Broadcast(transform, false);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        AnimatorDriver = GetComponent<AnimatorDriver>();
        intermediateStatistics = new List<Statistic>();
        weapon.SetOwner(this);
        phaseIndicators = new Image[phasesAmount];
        for (int i = 0; i < phasesAmount; i++)
        {
            GameObject obj = Instantiate(phaseIndicatorPrefab);
            obj.name = "Phase_" + i;
            obj.transform.SetParent(phaseIndicatorsParent);
            phaseIndicators[i] = obj.GetComponent<Image>();
        }
    }

    protected override void EngageBattle(Transform target)
    {
        base.EngageBattle(target);
        if (TargetContext != null)
        {
            stateMachine.SetBool(idleTransitionId, false);
            stateMachine.SetBool((++currentPhase).ToString(), true);
        }
    }

    protected override void SetStatus(Status status)
    {
        base.SetStatus(status);
        if (status == Status.IDLING)
        {
            Collider.enabled = true;
            Move(Vector2.zero);
            stateMachine.SetBool(idleTransitionId, true);
            for (int i = 1; i < phasesAmount + 1; i++)
            {
                stateMachine.SetBool(i.ToString(), false);
            }
        }
    }

    protected override void Die()
    {
        if (currentPhase < phasesAmount)
        {
            for (int i = 0; i < phasesAmount; i++)
            {
                if (i < phasesAmount - currentPhase)
                {
                    phaseIndicators[i].color = new Color(137F / 255F, 4 / 255F, 0, 1F);
                }
                else
                {
                    phaseIndicators[i].color = new Color(100F / 255F, 100F / 255F, 100F / 255F, 1F);
                }
            }
            stateMachine.SetBool((++currentPhase).ToString(), true);
        }
        else
        {
            base.Die();
        }
    }

    private void Start()
    {
        indicatorEventBus?.Broadcast(transform);
    }

    private bool ShouldAnimBeMirrored() => hasMirror && transform.localScale.x < 1F;

    private void Animate(string animation, object args = null)
    {
        AnimatorDriver.DriveAnimation(new AnimatorDriver.AnimationData(animation, ShouldAnimBeMirrored(), args));
    }
}