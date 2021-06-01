// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : IElementOfInterest.cs
//
// All Rights Reserved

using UnityEngine;

public interface IElementOfInterest
{
    public enum InterestPriority { LOW = 0, AVERAGE = 1, MAJOR = 2, MANDATORY = 3 }

    float ThresholdDistance { get; }

    Vector3 GetSightPoint();

    InterestPriority GetInterestPriority();
}