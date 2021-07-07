// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : SlashBossAbility.cs
//
// All Rights Reserved

using System.Collections;
using UnityEngine;

public class SlashBossAbility : Ability<BossEnemy>
{
    [SerializeField] private float chargeTime;
    [SerializeField] private Sword.Attack.Builder slash;

    public override bool CanPerform()
    {
        return base.CanPerform() && Vector3.Distance(Parent.transform.position, Parent.TargetContext.Transform.position) < slash.Build().Range;
    }

    protected override IEnumerator Execute_C()
    {
        Weapon weapon = Parent.GetWeapon();
        if (weapon is Sword sword)
        {
            bool wasBlocking = sword.IsBlocking;
            if (wasBlocking)
            {
                sword.ToggleBlock(false);
            }
            Parent.Move(Vector2.zero);
            Parent.Rigidbody.velocity = Vector2.zero;
            Parent.SetInteraction(Assets.Sprites.Exclamation, Color.red);
            yield return new WaitForSeconds(chargeTime);
            sword.TriggerAttack(slash.OnComplete(() =>
            {
                Parent.SetInteraction();
                Complete();
                if (wasBlocking)
                {
                    sword.ToggleBlock(true);
                }
            }));
        }
        else
        {
            Complete();
        }
    }
}