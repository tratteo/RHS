// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : OnHitEffectOf.cs
//
// All Rights Reserved

using UnityEngine;

public abstract class OnHitEffectOf<Target> : StatusEffectOf<Target> where Target : Component
{
    public OnHitEffectOf(float probability) : base(0F, probability)
    {
    }

    public override void RemoveFrom(Target target)
    {
    }
}