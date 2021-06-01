// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : StatusEffect.cs
//
// All Rights Reserved

public abstract class StatusEffect
{
    public float Duration { get; private set; }

    public float Probability { get; private set; }

    public StatusEffect(float duration, float probability = 1F)
    {
        Duration = duration;
        Probability = probability;
    }

    public abstract void ApplyTo(object target);

    public abstract void RemoveFrom(object target);

    public void EditDuration(float newDuration)
    {
        Duration = newDuration;
    }
}