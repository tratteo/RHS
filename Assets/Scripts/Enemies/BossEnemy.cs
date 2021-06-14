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
    [Header("Boss")]
    [SerializeField, Guarded] private Weapon weapon;
    [SerializeField, Guarded] private Animator stateMachine;
    [SerializeField, Guarded] private Transform phaseIndicatorsParent;
    [SerializeField, Guarded] private string idleTransitionId = "idle";
    [SerializeField] private int phasesAmount;
    [Header("Animation")]
    [SerializeField] private bool hasMirror = false;
    [SerializeField, Guarded] private GameObject phaseIndicatorPrefab;

    private int currentPhase = 0;

    private Image[] phaseIndicators;

    protected AnimatorDriver AnimatorDriver { get; private set; }

    public override Weapon GetWeapon() => weapon;

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

    protected override void Awake()
    {
        base.Awake();
        AnimatorDriver = GetComponent<AnimatorDriver>();
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
                    phaseIndicators[i].color = new Color(125F / 255F, 125F / 255F, 125F / 255F, 1F);
                }
            }
            stateMachine.SetBool((++currentPhase).ToString(), true);
        }
        else
        {
            base.Die();
        }
    }

    private bool ShouldAnimBeMirrored() => hasMirror && transform.localScale.x < 1F;

    private void Animate(string animation, object args = null)
    {
        AnimatorDriver.DriveAnimation(new AnimatorDriver.AnimationData(animation, ShouldAnimBeMirrored(), args));
    }
}