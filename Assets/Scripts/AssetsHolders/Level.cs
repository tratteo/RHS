// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Level.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Scriptable Objects/Levels/Level")]
public class Level : ScriptableObject
{
    [SerializeField, SceneString] public string scene;
    [SerializeField, Guarded] private BossEnemy boss;

    public string Scene => scene.ToString();

    public BossEnemy Boss => boss;
}