// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : ChargedSlashCharacterAbility.cs
//
// All Rights Reserved

using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ChargedSlashCharacterAbility", menuName = "Scriptable Objects/Character/Abilities/Charged Slash", order = 0)]
public class ChargedSlashCharacterAbility : Ability<CharacterManager>
{
    [SerializeField] private float chargeTime;
    [SerializeField] private Sword.Slash.Builder slash;

    protected override IEnumerator Execute_C(CharacterManager parent)
    {
        Weapon weapon = parent.Combat.GetWeapon();
        if (weapon is Sword sword)
        {
            bool wasBlocking = sword.IsBlocking;
            if (wasBlocking)
            {
                sword.ToggleBlock(false);
            }
            parent.GUI.SetInteraction(Assets.Sprites.Exclamation, Color.yellow);
            yield return new WaitForSeconds(chargeTime);
            sword.TriggerSlash(slash.OnComplete(() =>
            {
                Complete();
                parent.GUI.SetInteraction();
                if (wasBlocking)
                {
                    sword.ToggleBlock(true);
                }
            }));
        }
        else
        {
            Complete();
            parent.GUI.SetInteraction();
        }
    }
}