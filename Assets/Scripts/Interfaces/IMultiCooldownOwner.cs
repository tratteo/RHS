// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : IMultiCooldownOwner.cs
//
// All Rights Reserved

public interface IMultiCooldownOwner : ICooldownOwner
{
    int GetResourcesAmount();
}