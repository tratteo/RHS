// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : SessionStatisticsEventBus.cs
//
// All Rights Reserved

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SessionStatisticsBus", menuName = "Scriptable Objects/Bus/SessionStatisticsBus", order = 0)]
public class SessionStatisticsEventBus : SingleParamEventBus<List<Statistic>>
{
}