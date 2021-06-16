// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : MultislashBossAbility.cs
//
// All Rights Reserved

using GibFrame;
using System.Collections;
using UnityEngine;

public class MultislashBossAbility : Ability<BossEnemy>
{
    [SerializeField] private float channelTime = 1F;
    [SerializeField] private RandomizedFloat vulnerabilityTime;
    [SerializeField] private Multislash[] slashes;
    private int parried = 0;

    public override bool CanPerform()
    {
        return base.CanPerform() && (slashes.Length > 0);
    }

    protected override IEnumerator Execute_C()
    {
        parried = 0;
        Weapon weapon = Parent.GetWeapon();
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
                Parent.Move(Quaternion.AngleAxis(UnityEngine.Random.value > 0.5F ? 90F : -90F, Vector3.forward) * (Parent.TargetContext.Transform.position - Parent.transform.position).normalized);
                multislash.Slash.OnStart(() => Parent.SetInteraction(Assets.Sprites.Exclamation));
                multislash.Slash.OnComplete(() => Parent.SetInteraction());
                yield return new WaitForSeconds(multislash.Delay);
                Parent.Rigidbody.velocity = Vector2.zero;
                yield return new WaitForSeconds(AdjustPosition(Parent, multislash.Slash));
                sword.TriggerAttack(multislash.Slash);
                yield return new WaitForSeconds(multislash.Slash.Build().Duration);
            }
            Parent.Rigidbody.velocity = Vector2.zero;
            Parent.Move(Vector2.zero);
            sword.OnBlocked -= OnBlock;
            if (parried >= slashes.Length)
            {
                Parent.SetInteraction(Assets.Sprites.Stun);
                yield return new WaitForSeconds(vulnerabilityTime);
                Parent.SetInteraction();
            }
            if (wasBlocking)
            {
                sword.ToggleBlock(true);
            }
        }
        Complete();
    }

    protected override void OnStopped()
    {
        base.OnStopped();
        Weapon weapon = Parent.GetWeapon();
        if (weapon is Sword sword)
        {
            sword.OnBlocked -= OnBlock;
        }
    }

    private void OnBlock(Sword.Clash clash)
    {
        parried++;
    }

    private float AdjustPosition(BossEnemy parent, Sword.Attack currentSlash)
    {
        Vector2 directionToTarget = parent.TargetContext.Transform.position - parent.transform.position;
        if (parent.CurrentStatus == Enemy.Status.ATTACKING && parent.TargetContext != null)
        {
            float distance = Vector2.Distance(parent.transform.position, parent.TargetContext.Transform.position);
            if (distance > currentSlash.Range)
            {
                return parent.Dash(directionToTarget.Perturbate((Quaternion.AngleAxis(90F, Vector3.forward) * directionToTarget).normalized, currentSlash.Range * 0.75F).normalized * directionToTarget.magnitude, 2.75F);
            }
            else
            {
                return parent.Dash((-directionToTarget).Perturbate((Quaternion.AngleAxis(90F, Vector3.forward) * (-directionToTarget)).normalized, currentSlash.Range * 0.75F).normalized, (currentSlash.Range - distance) * 2.75F);
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
        [SerializeField] private Sword.Attack.Builder slash;

        public RandomizedFloat Delay => delay;

        public Sword.Attack.Builder Slash => slash;
    }
}