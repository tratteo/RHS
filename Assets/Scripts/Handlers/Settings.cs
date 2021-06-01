// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Settings.cs
//
// All Rights Reserved

using GibFrame.SaveSystem;
using System;

public static class Settings
{
    private static SettingsData settings;

    public static event Action<string, object> OnSettingChanged = delegate { };

    public static void Modify(string settingId, object value)
    {
        settings.AddOrModify(settingId, value);
        SaveManager.Async.MarkDirty((settings, SettingsData.PATH));
        OnSettingChanged?.Invoke(settingId, value);
    }

    public static T GetSetting<T>(string id)
    {
        if (settings != null)
        {
            return settings.Get<T>(id);
        }
        return default(T);
    }

    public static void Initialize()
    {
        settings = SaveManager.LoadOrInitialize(new SettingsData(), SettingsData.PATH);
        settings.Check((SettingsData.CAMERA_HARDNESS, 6F),
                       (SettingsData.VIBRATION_ACTIVE, true),
                       (SettingsData.MUSIC_ACTIVE, true),
                       (SettingsData.SOUND_ACTIVE, true),
                       (SettingsData.MUSIC_VOLUME, 0.8F),
                       (SettingsData.SOUND_VOLUME, 0.6F));

        SaveManager.Async.MarkDirty((settings, SettingsData.PATH));
    }
}