// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : InputEventBus.cs
//
// All Rights Reserved

using UnityEngine;

[CreateAssetMenu(fileName = "InputBus", menuName = "Scriptable Objects/Bus/InputBus", order = 0)]
public class InputEventBus : SingleParamEventBus<Inputs.InputData>
{
}