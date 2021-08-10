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
    [SerializeField, Guarded] private StringEventBus loadSceneChannel;
    [SerializeField] private Level[] levels;
    [SerializeField, Guarded] private Transform content;
    [SerializeField, Guarded] private GameObject levelObjectPrefab;

    private void Start()
    {
        GameDaemon.Instance.RemovePersistentResource(GameDaemon.LOADED_LEVEL);

        levels.ForEach(l =>
        {
            GameObject obj = Instantiate(levelObjectPrefab, content);
            if (l.Boss)
            {
                obj.transform.GetFirstComponentInChildrenWithName<Text>("Name", true).text = l.Boss.GetName();
                obj.transform.GetFirstComponentInChildrenWithName<Text>("Description", true).text = l.Boss.GetDescription();
            }
            obj.GetComponentInChildren<GButton>().AddOnReleasedCallback(new Callback(() =>
            {
                GameDaemon.Instance.AddPersistentResource(GameDaemon.LOADED_LEVEL, l);
                loadSceneChannel.Broadcast(l.Scene);
            }));
        });
    }
}