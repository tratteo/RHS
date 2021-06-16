// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : ChargedSlashCharacterAbility.cs
//
// All Rights Reserved

using System;
using System.Collections;
using UnityEngine;

public class ThrustCharacterAbility : Ability<CharacterManager>
{
    [SerializeField] private float chargeTime;
    [SerializeField] private Sword.Attack.Builder attack;

    protected override IEnumerator Execute_C()
    {
        Weapon weapon = Parent.Combat.GetWeapon();
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
                Parent.GUI.SetInteraction(Assets.Sprites.Exclamation, Color.yellow);
                sword.OverrideRotation(transform.up);
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
                Parent.GUI.SetInteraction();
                if (wasBlocking)
                {
                    sword.ToggleBlock(true);
                }
            }
            else
            {
                Complete();
                Parent.GUI.SetInteraction();
            }
        }
        else
        {
            Complete();
            Parent.GUI.SetInteraction();
        }
    }
}