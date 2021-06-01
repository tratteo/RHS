// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : ICooldownOwner.cs
//
// All Rights Reserved

using UnityEngine;

public interface ICooldownOwner
{
    float GetCooldown();

    float GetCooldownPercentage();

    Sprite GetIcon();
}