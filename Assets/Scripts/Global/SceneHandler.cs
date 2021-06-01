// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : SceneHandler.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    private string currentLoaded = "";
    [SerializeField, Guarded] private StringChannelEvent loadSceneChannel;
    [SerializeField, Guarded] private GameObject backgroundPanel;

    public void LoadSceneAsync(string name)
    {
        if (currentLoaded != "")
        {
            SceneManager.UnloadSceneAsync(currentLoaded);
        }
        backgroundPanel.SetActive(true);
        SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive).completed += (op) =>
        {
            OnSceneLoaded(op, name);
            Time.timeScale = 1F;
            backgroundPanel.SetActive(false);
        };
    }

    private void OnSceneLoaded(AsyncOperation op, string name)
    {
        currentLoaded = name;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(currentLoaded));
    }

    private void OnEnable()
    {
        loadSceneChannel.OnEvent += LoadSceneAsync;
    }

    private void OnDisable()
    {
        loadSceneChannel.OnEvent -= LoadSceneAsync;
    }
}