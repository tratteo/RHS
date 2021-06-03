// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : BossEnemy.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;
using UnityEngine.UI;

public class BossEnemy : Enemy
{
    [Header("Channels")]
    [SerializeField, Guarded] private AnimationChannelEvent animationChannel;
    [Header("Boss")]
    [SerializeField, Guarded] private Weapon weapon;
    [SerializeField, Guarded] private Animator stateMachine;
    [SerializeField, Guarded] private Transform phaseIndicatorsParent;
    [SerializeField, Guarded] private string idleTransitionId = "idle";
    [SerializeField] private int phasesAmount;
    private int currentPhase = 0;
    private Image[] phaseIndicators;

    public override Weapon GetWeapon() => weapon;

    public override void CommonFixedUpdate(float fixedDeltaTime)
    {
        base.CommonFixedUpdate(fixedDeltaTime);
        if (IsDashing)
        {
            animationChannel.Broadcast(new AnimationChannelEvent.AnimationData(AnimatorDriver.DASH, null));
        }
        else
        {
            if (Mathf.Approximately(TargetVelocity.magnitude, 0F))
            {
                animationChannel.Broadcast(new AnimationChannelEvent.AnimationData(AnimatorDriver.IDLE, null));
            }
            else
            {
                animationChannel.Broadcast(new AnimationChannelEvent.AnimationData(AnimatorDriver.RUN, Rigidbody.velocity.magnitude));
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        weapon.SetOwner(this);
        phaseIndicators = new Image[phasesAmount];
        for (int i = 0; i < phasesAmount; i++)
        {
            GameObject obj = new GameObject()
            {
                name = "Phase_" + i
            };
            Image image = obj.AddComponent<Image>();
            image.rectTransform.sizeDelta = new Vector2(50, 50);
            image.transform.SetParent(phaseIndicatorsParent);
            image.maskable = false;
            image.color = Color.red;
            phaseIndicators[i] = image;
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
                    phaseIndicators[i].color = Color.red;
                }
                else
                {
                    phaseIndicators[i].color = Color.white;
                }
            }
            stateMachine.SetBool((++currentPhase).ToString(), true);
        }
        else
        {
            base.Die();
        }
    }
}