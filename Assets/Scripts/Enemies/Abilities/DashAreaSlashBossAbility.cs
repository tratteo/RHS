// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : DashAreaSlashBossAbility.cs
//
// All Rights Reserved

using GibFrame;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "DashAreaSlashBossAbility", menuName = "Scriptable Objects/Bosses/Abilities/Dash Slash", order = 0)]
public class DashAreaSlashBossAbility : Ability<BossEnemy>
{
    [SerializeField] private RandomizedFloat vulnerabilityTime;
    [SerializeField] private float waitTime = 0.75F;
    [SerializeField] private Sword.Slash.Builder slash;

    protected override IEnumerator Execute_C(BossEnemy parent)
    {
        if (parent.GetWeapon() is Sword sword)
        {
            bool wasBlocking = sword.IsBlocking;
            if (wasBlocking)
            {
                sword.ToggleBlock(false);
            }
            slash.OnComplete(() => parent.SetInteraction());
            parent.Move(Vector2.zero);
            Vector3 axis = (parent.transform.position - parent.TargetContext.Transform.position).normalized;
            Vector3 pos = parent.TargetContext.Transform.position + (axis * 10F);
            parent.Dash(pos - parent.transform.position, 4F);
            yield return new WaitForSeconds(waitTime);
            parent.SetInteraction(Assets.Sprites.Exclamation, Color.red);
            float duration = parent.Dash(parent.TargetContext.Transform.position - parent.transform.position, 2.75F);
            yield return new WaitForSeconds(duration);
            sword.TriggerSlash(slash);
            parent.Move(Vector2.zero);
            yield return new WaitForSeconds(slash.Build().Duration + vulnerabilityTime);
            if (wasBlocking)
            {
                sword.ToggleBlock(true);
            }
        }
        Complete();
    }
}