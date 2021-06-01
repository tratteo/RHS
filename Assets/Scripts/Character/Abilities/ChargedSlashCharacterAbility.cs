// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : ChargedSlashCharacterAbility.cs
//
// All Rights Reserved

using System.Collections;
using UnityEngine;

public class ChargedSlashCharacterAbility : Ability<CharacterManager>
{
    [SerializeField] private float chargeTime;
    [SerializeField] private Sword.Slash.Builder slash;

    protected override IEnumerator Execute_C()
    {
        Weapon weapon = Parent.Combat.GetWeapon();
        if (weapon is Sword sword)
        {
            bool wasBlocking = sword.IsBlocking;
            if (wasBlocking)
            {
                sword.ToggleBlock(false);
            }
            Parent.GUI.SetInteraction(Assets.Sprites.Exclamation, Color.yellow);
            yield return new WaitForSeconds(chargeTime);
            sword.TriggerSlash(slash.OnComplete(() =>
            {
                Complete();
                Parent.GUI.SetInteraction();
                if (wasBlocking)
                {
                    sword.ToggleBlock(true);
                }
            }));
        }
        else
        {
            Complete();
            Parent.GUI.SetInteraction();
        }
    }
}