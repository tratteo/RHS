// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Vibration.cs
//
// All Rights Reserved

using GibFrame.Plugins;

public static class Vibration
{
    public static readonly int LIGHT_CLICK = 30;
    public static readonly int CLICK = 45;
    public static readonly int HEAVY_CLICK = 60;
    public static readonly int LIGHT_BUZZ = 75;

    private static Vibrator vibrator;

    public static void Initialize()
    {
        vibrator = new Vibrator();
    }

    public static void OneShot(long millis)
    {
        if (Settings.GetSetting<bool>(SettingsData.VIBRATION_ACTIVE))
        {
            vibrator.OneShot(millis);
        }
    }
}