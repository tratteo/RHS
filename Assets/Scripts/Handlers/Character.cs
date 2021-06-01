// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Character.cs
//
// All Rights Reserved

using GibFrame.SaveSystem;

public class Character
{
    private static CharacterData data;

    public static void Initialize()
    {
        data = SaveManager.LoadOrInitialize(new CharacterData(), CharacterData.PATH);
    }

    public static void MarkDirty()
    {
        SaveManager.Async.MarkDirty((data, CharacterData.PATH));
    }

    public static string GetEquippedAbility() => data.EquippedAbility;
}