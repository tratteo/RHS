// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : LevelSelectorUI.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.UI;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectorUI : MonoBehaviour
{
    [SerializeField, Guarded] private StringChannelEvent loadSceneChannel;
    [SerializeField] private Level[] levels;
    [SerializeField, Guarded] private Transform content;
    [SerializeField, Guarded] private GameObject levelObjectPrefab;

    private void Start()
    {
        levels.ForEach(l =>
        {
            GameObject obj = Instantiate(levelObjectPrefab, content);
            obj.GetComponentInChildren<Text>().text = l.GetName();
            obj.GetComponentInChildren<GButton>().AddOnReleasedCallback(new Callback(() =>
            {
                GameDaemon.Instance.AddPersistentResource(GameDaemon.LOADED_LEVEL, l);
                loadSceneChannel.Broadcast(l.GetName());
            }));
        });
    }
}