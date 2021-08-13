// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : TutorialBossPhaseII.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

public class TutorialBossPhaseII : BossPhaseStateMachine
{
    [SerializeField] private Sword.Attack.Builder slash;
    private Sword sword;
    [SerializeField] private TutorialEventBus tutorialBus;
    [Header("Bomber")]
    [SerializeField] private RandomizedFloat attackUpdate;
    private float attackTimer;
    private bool abilitiesUnlocked = false;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        if (Owner.Weapon is Sword weapon)
        {
            sword = weapon;
        }
        else
        {
            Debug.Log("Unable to retrieve required sword");
        }
        tutorialBus.BroadcastNextFocus();
        attackTimer = 2F;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if (!IsPerformingAbility && CanExecute())
        {
            if (attackTimer > 0)
            {
                attackTimer -= Time.deltaTime;
            }
            else if (GetDistanceToTarget() <= slash.Build().Range)
            {
                sword.TriggerAttack(slash);
                attackTimer = attackUpdate;
            }
        }
        float healthPercentage = Owner.GetHealthPercentage();
        Debug.Log(healthPercentage);
        if (!abilitiesUnlocked && healthPercentage <= 0.5F)
        {
            tutorialBus.BroadcastNextFocus();
            abilitiesUnlocked = true;
        }
    }

    protected override bool CanExecuteAbilities() => abilitiesUnlocked;

    protected override float GetRange() => slash.Build().Range * 0.75F;

    private void Awake()
    {
        slash.OnStart(() => Owner.SetInteraction(Assets.Sprites.Exclamation));
        slash.OnComplete(() => Owner.SetInteraction());
    }
}