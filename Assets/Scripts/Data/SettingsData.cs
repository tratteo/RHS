// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : SettingsData.cs
//
// All Rights Reserved

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SettingsData
{
    public const string MUSIC_ACTIVE = "music_active";
    public const string SOUND_ACTIVE = "sound_active";
    public const string MUSIC_VOLUME = "music_volume";
    public const string SOUND_VOLUME = "sounds_volume";
    public const string CAMERA_HARDNESS = "camera_hardness";
    public const string VIBRATION_ACTIVE = "vibration_active";

    public static readonly string PATH = Application.persistentDataPath + "/settings.data";
    private readonly Dictionary<string, object> settings;

    public SettingsData(params (string, object)[] settings)
    {
        this.settings = new Dictionary<string, object>();
        for (int i = 0; i < settings.Length; i++)
        {
            this.settings.Add(settings[i].Item1, settings[i].Item2);
        }
    }

    public bool CheckForMissingSettings(params (string, object)[] settings)
    {
        bool changed = false;
        for (int i = 0; i < settings.Length; i++)
        {
            if (!this.settings.ContainsKey(settings[i].Item1))
            {
                this.settings.Add(settings[i].Item1, settings[i].Item2);
                changed = true;
            }
        }
        return changed;
    }

    public void AddOrModify(string key, object value)
    {
        if (settings.ContainsKey(key))
        {
            settings[key] = value;
        }
        else
        {
            settings.Add(key, value);
        }
    }

    public int Check(params (string, object)[] values)
    {
        int count = 0;
        foreach ((string, object) val in values)
        {
            if (!settings.ContainsKey(val.Item1))
            {
                settings.Add(val.Item1, val.Item2);
                count++;
            }
        }
        return count;
    }

    public T Get<T>(string key)
    {
        if (settings.ContainsKey(key))
        {
            return (T)settings[key];
        }
        else return default(T);
    }
}