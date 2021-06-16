// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : ThrustBossAbility.cs
//
// All Rights Reserved

using System;
using System.Collections;
using UnityEngine;

public class ThrustBossAbility : Ability<BossEnemy>
{
    [SerializeField] private float chargeTime;
    [SerializeField] private Sword.Attack.Builder attack;

    protected override IEnumerator Execute_C()
    {
        Weapon weapon = Parent.GetWeapon();
        if (weapon is Sword sword)
        {
            AnimationClip clip = Array.Find(sword.Animator.runtimeAnimatorController.animationClips, (c) => c.name.Equals("thrust"));
            if (clip)
            {
                bool wasBlocking = sword.IsBlocking;
                if (wasBlocking)
                {
                    sword.ToggleBlock(false);
                }
                Parent.SetInteraction(Assets.Sprites.Exclamation, Color.red);
                sword.OverrideRotation(transform.up);
                Parent.Move(Vector2.zero);
                yield return new WaitForSeconds(chargeTime);
                if (sword.HasTarget())
                {
                    sword.OverrideRotation(sword.Target.GetSightPoint() - Parent.transform.position);
                }
                else
                {
                    sword.OverrideRotation(transform.right * Mathf.Sign(Parent.transform.localScale.x));
                }
                sword.Animator.Play("thrust");
                sword.TriggerAttack(attack);
                yield return new WaitForSeconds(clip.length);
                sword.ResetOverrideRotation();
                Complete();
                Parent.SetInteraction();
                if (wasBlocking)
                {
                    sword.ToggleBlock(true);
                }
            }
            else
            {
                Complete();
                Parent.SetInteraction();
            }
        }
        else
        {
            Complete();
            Parent.SetInteraction();
        }
    }
}