// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : IHealthHolder.cs
//
// All Rights Reserved

using System;

public interface IHealthHolder
{
    event Action OnDeath;

    void Damage(float amount);

    void Heal(float amount);

    float GetHealthPercentage();
}