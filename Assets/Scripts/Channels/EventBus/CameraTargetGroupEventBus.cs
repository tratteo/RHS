using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CameraTargetGroupBus", menuName = "Scriptable Objects/Bus/CameraTargetGroupBus", order = 0)]
public class CameraTargetGroupEventBus : DoubleParamEventBus<Transform, bool>
{
}