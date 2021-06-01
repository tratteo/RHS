// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : SerializedDescribable.cs
//
// All Rights Reserved

using System;
using UnityEngine;

[Serializable]
public class SerializedDescribable : IDescribable
{
    [SerializeField] private string id;
    [SerializeField] private string name;
    [SerializeField] private Sprite icon;
    [TextArea] [SerializeField] private string description;

    public string GetDescription() => description;

    public Sprite GetIcon() => icon;

    public string GetId() => id;

    public string GetName() => name;
}