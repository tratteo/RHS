// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : IndicatorEventBus.cs
//
// All Rights Reserved

using UnityEngine;

[CreateAssetMenu(fileName = "IndicatorEventBus", menuName = "Scriptable Objects/Bus/IndicatorBus", order = 0)]
public class IndicatorEventBus : SingleParamEventBus<IndicatorEventBus.IndicatorData>
{
    public class IndicatorData
    {
        public Transform Target { get; private set; }

        public IndicatorsGUI.IndicatorType Type { get; private set; }

        public bool Add { get; private set; }

        public IndicatorData(Transform target, bool add, IndicatorsGUI.IndicatorType type = IndicatorsGUI.IndicatorType.INFO)
        {
            Target = target;
            Type = type;
            Add = add;
        }
    }
}