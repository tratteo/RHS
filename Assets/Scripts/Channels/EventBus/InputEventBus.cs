// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : InputEventBus.cs
//
// All Rights Reserved

using UnityEngine;

[CreateAssetMenu(fileName = "InputChannel", menuName = "Scriptable Objects/Channels/InputChannel", order = 0)]
public class InputEventBus : SingleParamEventBus<Inputs.InputData>
{
}