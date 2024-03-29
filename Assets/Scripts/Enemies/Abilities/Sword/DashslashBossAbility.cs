﻿// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : DashslashBossAbility.cs
//
// All Rights Reserved

using GibFrame;
using System.Collections;
using UnityEngine;

public class DashslashBossAbility : Ability<BossEnemy>
{
    [SerializeField] private RandomizedFloat vulnerabilityTime;
    [SerializeField] private float waitTime = 0.75F;
    [SerializeField] private Sword.Attack.Builder slash;
    [SerializeField] private ParticleSystem chargeEffect;

    protected override IEnumerator Execute_C()
    {
        if (Parent.Weapon is Sword sword)
        {
            bool wasBlocking = sword.IsBlocking;
            if (wasBlocking)
            {
                sword.ToggleBlock(false);
            }
            slash.OnComplete(() => Parent.SetInteraction());
            Parent.Move(Vector2.zero);
            Vector3 axis = (Parent.transform.position - Parent.BattleContext.Transform.position).normalized;
            Vector3 pos = Parent.BattleContext.Transform.position + (axis * 12F);
            Parent.Dash(pos - Parent.transform.position, 4F);
            yield return new WaitForSeconds(waitTime);
            Parent.SetInteraction(Assets.Sprites.Exclamation, Color.red);
            float duration = Parent.Dash(Parent.BattleContext.Transform.position - Parent.transform.position, 2.75F);
            yield return new WaitForSeconds(duration);
            chargeEffect?.Play();
            sword.TriggerAttack(slash);
            Parent.Move(Vector2.zero);
            yield return new WaitForSeconds(slash.Build().Duration + vulnerabilityTime);
            if (wasBlocking)
            {
                sword.ToggleBlock(true);
            }
        }
        Complete();
    }
}