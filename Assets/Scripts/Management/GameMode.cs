// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : GameMode.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    [SerializeField, Guarded] private GameObject characterPrefab;

    private void Start()
    {
        if (!GameDaemon.Instance.TryGetResource(GameDaemon.LOADED_LEVEL, out Level loadedLevel, true))
        {
            Debug.LogError("Unable to load the level from the game mode!");
        }
        else
        {
            Instantiate(characterPrefab, Vector2.zero, Quaternion.identity);
            Instantiate(loadedLevel.Boss, Vector2.right * 15F, Quaternion.identity);
        }
    }
}