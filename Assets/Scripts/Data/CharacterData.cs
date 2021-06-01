// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : CharacterData.cs
//
// All Rights Reserved

using GibFrame.SaveSystem;
using UnityEngine;

[System.Serializable]
public class CharacterData : EncryptedData
{
    public static readonly string PATH = Application.persistentDataPath + "/character.data";

    public string EquippedAbility { get; set; }

    public CharacterData()
    {
    }
}