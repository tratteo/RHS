// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : SessionStatisticsEventBus.cs
//
// All Rights Reserved

using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatisticsChannel", menuName = "Scriptable Objects/Channels/StatisticsChannel", order = 0)]
public class SessionStatisticsEventBus : SingleParamEventBus<List<Statistic>>
{
}