// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : CharacterManager.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.Performance;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public CharacterKinematic Kinematic { get; private set; }

    public CharacterCamera Camera { get; private set; }

    public CharacterCombat Combat { get; private set; }

    public CharacterGUI GUI { get; private set; }

    private void Awake()
    {
        (Kinematic = GetComponent<CharacterKinematic>()).SetManager(this);
        (Camera = GetComponent<CharacterCamera>()).SetManager(this);
        (Combat = GetComponent<CharacterCombat>()).SetManager(this);
        (GUI = GetComponent<CharacterGUI>()).SetManager(this);
    }
}