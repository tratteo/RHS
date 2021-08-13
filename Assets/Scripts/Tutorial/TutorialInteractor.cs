// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : TutorialInteractor.cs
//
// All Rights Reserved

using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialInteractor : MonoBehaviour
{
    [SerializeField] private bool oneshot = true;
    [SerializeField] private EventTrigger.Entry actionTrigger;

    private void Awake()
    {
        EventTrigger eventTrigger = gameObject.AddComponent<EventTrigger>();
        eventTrigger.triggers.Add(actionTrigger);
        if (oneshot)
        {
            actionTrigger.callback.AddListener((b) =>
            {
                Destroy(eventTrigger);
                Destroy(this);
            });
        }
    }
}