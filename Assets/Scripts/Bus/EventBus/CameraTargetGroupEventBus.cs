// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : CameraTargetGroupEventBus.cs
//
// All Rights Reserved

using UnityEngine;

[CreateAssetMenu(fileName = "CameraTargetGroupBus", menuName = "Scriptable Objects/Bus/CameraTargetGroupBus", order = 0)]
public class CameraTargetGroupEventBus : DoubleParamEventBus<Transform, bool>
{
}