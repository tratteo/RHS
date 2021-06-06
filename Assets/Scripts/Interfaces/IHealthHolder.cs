// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : IHealthHolder.cs
//
// All Rights Reserved

public interface IHealthHolder
{
    void Damage(float amount);

    void Heal(float amount);

    float GetHealthPercentage();
}