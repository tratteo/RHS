// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : SettingsManager.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.Audio;
using GibFrame.UI;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ToggleButton vibrationToggle;
    [SerializeField] private Slider soundSlider;
    [SerializeField] private ToggleButton soundToggle;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private ToggleButton musicToggle;
    [SerializeField] private Slider cameraSmoothness;
    [SerializeField] private ToggleButton[] qualityToggles;

    public void SoundSliderChanged(float newValue)
    {
        AudioManager.Instance.ChangeSoundsVolume(newValue);
        if (newValue == 0)
        {
            Settings.Modify(SettingsData.SOUND_ACTIVE, false);
            soundToggle.SetState(false);
        }
        else
        {
            soundToggle.SetState(true);
        }
    }

    public void MusicSliderChanged(float newValue)
    {
        AudioManager.Instance.ChangeMusicVolume(newValue);
        if (newValue == 0)
        {
            Settings.Modify(SettingsData.MUSIC_ACTIVE, false);
            musicToggle.SetState(false);
        }
        else
        {
            musicToggle.SetState(true);
        }
    }

    public void SoundVolumeEndDrag()
    {
        Settings.Modify(SettingsData.SOUND_VOLUME, soundSlider.value);
    }

    public void SetQualityLevel(int level)
    {
        if (level == QualitySettings.GetQualityLevel()) return;
        QualitySettings.SetQualityLevel(level);
        for (int i = 0; i < qualityToggles.Length; i++)
        {
            qualityToggles[i].SetState(i == level);
        }
    }

    public void MusicVolumeEndDrag()
    {
        Settings.Modify(SettingsData.MUSIC_VOLUME, musicSlider.value);
    }

    public void CameraSmoothnessEndDrag()
    {
        float newVal = cameraSmoothness.value;
        Settings.Modify(SettingsData.CAMERA_HARDNESS, newVal);
    }

    public void OnSliderChanged(float newValue)
    {
        //!Needed in order to change the slider value runtime
    }

    public void Toggle()
    {
        SetActive(!gameObject.activeSelf);
    }

    public void ToggleMusic()
    {
        bool newState = !Settings.GetSetting<bool>(SettingsData.MUSIC_ACTIVE);
        Settings.Modify(SettingsData.MUSIC_ACTIVE, newState);
        musicToggle.SetState(newState);
        if (!newState)
        {
            AudioManager.Instance.ChangeMusicVolume(0F);
            musicSlider.value = 0F;
        }
        else
        {
            AudioManager.Instance.ChangeMusicVolume(Settings.GetSetting<float>(SettingsData.MUSIC_VOLUME));
            musicSlider.value = Settings.GetSetting<float>(SettingsData.MUSIC_VOLUME);
        }
    }

    public void ToggleSound()
    {
        bool newState = !Settings.GetSetting<bool>(SettingsData.SOUND_ACTIVE);
        Settings.Modify(SettingsData.SOUND_ACTIVE, newState);
        soundToggle.SetState(newState);
        if (!newState)
        {
            AudioManager.Instance.ChangeSoundsVolume(0F);
            soundSlider.value = 0F;
        }
        else
        {
            AudioManager.Instance.ChangeSoundsVolume(Settings.GetSetting<float>(SettingsData.SOUND_VOLUME));
            soundSlider.value = Settings.GetSetting<float>(SettingsData.SOUND_VOLUME);
        }
    }

    public void SetActive(bool active)
    {
        if (active != gameObject.activeSelf)
        {
            gameObject.SetActive(active);
        }
        if (gameObject.activeSelf)
        {
            vibrationToggle.SetState(Settings.GetSetting<bool>(SettingsData.VIBRATION_ACTIVE));
            musicToggle.SetState(Settings.GetSetting<bool>(SettingsData.MUSIC_ACTIVE));
            soundToggle.SetState(Settings.GetSetting<bool>(SettingsData.SOUND_ACTIVE));
            musicSlider.value = Settings.GetSetting<bool>(SettingsData.MUSIC_ACTIVE) ? Settings.GetSetting<float>(SettingsData.MUSIC_VOLUME) : 0F;
            soundSlider.value = Settings.GetSetting<bool>(SettingsData.SOUND_ACTIVE) ? Settings.GetSetting<float>(SettingsData.SOUND_VOLUME) : 0F;
            cameraSmoothness.value = Settings.GetSetting<float>(SettingsData.CAMERA_HARDNESS);
        }
        Time.timeScale = gameObject.activeSelf ? 0F : 1F;
    }

    public void ToggleVibration()
    {
        bool newState = !Settings.GetSetting<bool>(SettingsData.VIBRATION_ACTIVE);
        Settings.Modify(SettingsData.VIBRATION_ACTIVE, newState);
        vibrationToggle.SetState(newState);
        if (newState)
        {
            Vibration.OneShot(Vibration.LIGHT_BUZZ);
        }
    }

    private void Start()
    {
        for (int i = 0; i < qualityToggles.Length; i++)
        {
            qualityToggles[i].SetState(i == QualitySettings.GetQualityLevel());
        }
    }
}