// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : IMovable.cs
//
// All Rights Reserved

using UnityEngine;

public interface IMovable
{
    void Move(Vector2 direction, float speedMultiplier = 1F);
}