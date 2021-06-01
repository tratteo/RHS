// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : ChargedslashBossAbility.cs
//
// All Rights Reserved

using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "ChargedSlashBossAbility", menuName = "Scriptable Objects/Bosses/Abilities/Charged Slash", order = 0)]
public class ChargedslashBossAbility : Ability<BossEnemy>
{
    [SerializeField] private float chargeTime;
    [SerializeField] private Sword.Slash.Builder slash;

    public override bool CanPerform(BossEnemy parent)
    {
        return base.CanPerform(parent) && Vector3.Distance(parent.transform.position, parent.TargetContext.Transform.position) < slash.Build().Range;
    }

    protected override IEnumerator Execute_C(BossEnemy parent)
    {
        Weapon weapon = parent.GetWeapon();
        if (weapon is Sword sword)
        {
            bool wasBlocking = sword.IsBlocking;
            if (wasBlocking)
            {
                sword.ToggleBlock(false);
            }
            parent.Move(Vector2.zero);
            parent.Rigidbody.velocity = Vector2.zero;
            parent.SetInteraction(Assets.Sprites.Exclamation, Color.red);
            yield return new WaitForSeconds(chargeTime);
            sword.TriggerSlash(slash.OnComplete(() =>
            {
                parent.SetInteraction();
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