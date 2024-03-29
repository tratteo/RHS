﻿// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : TransformEventBus.cs
//
// All Rights Reserved

using UnityEngine;

[CreateAssetMenu(fileName = "TransformBus", menuName = "Scriptable Objects/Bus/TransformBus", order = 0)]
public class TransformEventBus : SingleParamEventBus<Transform>
{
}