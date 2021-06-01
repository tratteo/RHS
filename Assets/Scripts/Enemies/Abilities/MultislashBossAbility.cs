// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : MultislashBossAbility.cs
//
// All Rights Reserved

using GibFrame;
using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "MultislashBossAbility", menuName = "Scriptable Objects/Bosses/Abilities/Multi Slash", order = 0)]
public class MultislashBossAbility : Ability<BossEnemy>
{
    [SerializeField] private float channelTime = 1F;
    [SerializeField] private RandomizedFloat vulnerabilityTime;
    [SerializeField] private Multislash[] slashes;
    private int parried = 0;

    public override bool CanPerform(BossEnemy parent)
    {
        return base.CanPerform(parent) && (slashes.Length > 0);
    }

    protected override IEnumerator Execute_C(BossEnemy parent)
    {
        parried = 0;
        Weapon weapon = parent.GetWeapon();
        if (weapon is Sword sword)
        {
            bool wasBlocking = sword.IsBlocking;
            if (wasBlocking)
            {
                sword.ToggleBlock(false);
            }
            sword.OnBlocked += OnBlock;
            yield return new WaitForSeconds(channelTime);
            foreach (Multislash multislash in slashes)
            {
                parent.Move(Vector2.zero);
                parent.Rigidbody.velocity = Vector2.zero;
                multislash.Slash.OnStart(() => parent.SetInteraction(Assets.Sprites.Exclamation));
                multislash.Slash.OnComplete(() => parent.SetInteraction());
                yield return new WaitForSeconds(multislash.Delay);
                yield return new WaitForSeconds(AdjustPosition(parent, multislash.Slash));
                sword.TriggerSlash(multislash.Slash);
                yield return new WaitForSeconds(multislash.Slash.Build().Duration);
            }
            parent.Rigidbody.velocity = Vector2.zero;
            parent.Move(Vector2.zero);
            sword.OnBlocked -= OnBlock;
            if (parried >= slashes.Length)
            {
                parent.SetInteraction(Assets.Sprites.Stun);
                yield return new WaitForSeconds(vulnerabilityTime);
                parent.SetInteraction();
            }
            if (wasBlocking)
            {
                sword.ToggleBlock(true);
            }
        }
        Complete();
    }

    protected override void OnStopped(BossEnemy parent)
    {
        base.OnStopped(parent);
        Weapon weapon = parent.GetWeapon();
        if (weapon is Sword sword)
        {
            sword.OnBlocked -= OnBlock;
        }
    }

    private void OnBlock(Sword.Clash clash)
    {
        parried++;
    }

    private float AdjustPosition(BossEnemy parent, Sword.Slash currentSlash)
    {
        if (parent.CurrentStatus == Enemy.Status.ATTACKING && parent.TargetContext != null)
        {
            float distance = Vector2.Distance(parent.transform.position, parent.TargetContext.Transform.position);
            if (distance > currentSlash.Range)
            {
                return parent.Dash(parent.TargetContext.Transform.position - parent.transform.position, 2.25F);
            }
            else
            {
                Vector2 axis = parent.transform.position - parent.TargetContext.Transform.position;
                return parent.Dash(axis.Perturbate(Quaternion.AngleAxis(90F, Vector3.forward) * axis, 1F).normalized, (currentSlash.Range - distance) * 2.5F);
            }
        }
        else
        {
            parent.Move(Vector2.zero);
            return 0F;
        }
    }

    [System.Serializable]
    private class Multislash
    {
        [SerializeField] private RandomizedFloat delay;
        [SerializeField] private Sword.Slash.Builder slash;

        public RandomizedFloat Delay => delay;

        public Sword.Slash.Builder Slash => slash;
    }
}