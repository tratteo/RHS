// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : TutorialDriver.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

public class TutorialDriver : MonoBehaviour
{
    [SerializeField, Guarded] private TutorialEventBus tutorialBus;
    [SerializeField] private bool initializeFirstElement = true;
    [SerializeField] private TutorialItem[] tutorialElements;
    private int activeIndex = -1;

    public void ClearAllProjectiles()
    {
        Projectile[] projectiles = FindObjectsOfType<Projectile>(false);
        foreach (Projectile projectile in projectiles)
        {
            projectile.Destroy();
        }
    }

    private void NextFocus()
    {
        ReleaseFocus();
        RequestFocus();
    }

    private void ReleaseFocus(GameObject sender = null)
    {
        if (activeIndex >= 0)
        {
            tutorialElements[activeIndex].ReleaseFocus();
        }
    }

    private void Start()
    {
        if (initializeFirstElement)
        {
            NextFocus();
        }
    }

    private void Awake()
    {
        tutorialElements.ForEach(e => e.ReleaseFocus(false));
    }

    private void OnEnable()
    {
        tutorialBus.OnReleaseFocus += ReleaseFocus;
        tutorialBus.OnFocus += RequestFocus;
        tutorialBus.OnNextFocus += NextFocus;
    }

    private void OnDisable()
    {
        tutorialBus.OnReleaseFocus -= ReleaseFocus;
        tutorialBus.OnFocus -= RequestFocus;
        tutorialBus.OnNextFocus -= NextFocus;
    }

    private void RequestFocus(GameObject sender = null)
    {
        if (activeIndex + 1 < tutorialElements.Length)
        {
            tutorialElements[++activeIndex].Focus();
        }
    }
}