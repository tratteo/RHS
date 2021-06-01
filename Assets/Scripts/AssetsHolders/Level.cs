// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Level.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Levels/Level")]
public class Level : ScriptableObject, IDescribable
{
    [SerializeField, Guarded] private SerializedDescribable describable;
    [SerializeField, Guarded] private GameObject boss;

    public GameObject Boss => boss;

    public string GetDescription() => describable.GetDescription();

    public Sprite GetIcon() => describable.GetIcon();

    public string GetId() => describable.GetId();

    public string GetName() => describable.GetName();
}