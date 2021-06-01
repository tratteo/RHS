// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : StringChannelEvent.cs
//
// All Rights Reserved

using UnityEngine;

[CreateAssetMenu(fileName = "StringChannel", menuName = "Scriptable Objects/Channels/StringChannel", order = 0)]
public class StringChannelEvent : SingleParamChannelEvent<string>
{
    public const string MENU = "Menu";
    public const string MAP = "Map";
}