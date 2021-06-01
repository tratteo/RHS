// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : ElementOfInterest.cs
//
// All Rights Reserved

using UnityEngine;

public class ElementOfInterest : MonoBehaviour, IElementOfInterest
{
    [SerializeField] private float thresholdDistance = 5F;
    [SerializeField] private IElementOfInterest.InterestPriority priority;
    [SerializeField] private Vector3 sightPointOffset;

    public float ThresholdDistance => thresholdDistance;

    public IElementOfInterest.InterestPriority GetInterestPriority() => priority;

    public Vector3 GetSightPoint()
    {
        return sightPointOffset + transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(GetSightPoint(), ThresholdDistance);
    }
}