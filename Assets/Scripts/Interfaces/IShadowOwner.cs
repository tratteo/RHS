// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : IShadowOwner.cs
//
// All Rights Reserved

using System;

public interface IShadowOwner
{
    event Action<bool> OnChangeGroundedState;
}