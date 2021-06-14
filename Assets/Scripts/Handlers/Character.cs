// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Character.cs
//
// All Rights Reserved

using GibFrame.SaveSystem;

public class Character
{
    private static CharacterData data;
    private static StatisticsData statistics;

    public static void Initialize()
    {
        statistics = SaveManager.LoadOrInitialize(new StatisticsData(), StatisticsData.PATH);
        data = SaveManager.LoadOrInitialize(new CharacterData(), CharacterData.PATH);
    }

    public static void MarkDirty()
    {
        SaveManager.Async.MarkDirty((data, CharacterData.PATH), (statistics, StatisticsData.PATH));
    }

    public static string GetEquippedAbility() => data.EquippedAbility;
}