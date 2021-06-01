// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : StatusEffectOf.cs
//
// All Rights Reserved

using UnityEngine;

[System.Serializable]
public abstract class StatusEffectOf<A> : StatusEffect where A : Component
{
    protected StatusEffectOf(float duration, float probability = 1F) : base(duration, probability)
    {
    }

    public abstract void ApplyTo(A target);

    public override void ApplyTo(object target)
    {
        if (target is A)
        {
            ApplyTo(target as A);
        }
    }

    public abstract void RemoveFrom(A target);

    public override void RemoveFrom(object target)
    {
        if (target is A)
        {
            RemoveFrom(target as A);
        }
    }
}