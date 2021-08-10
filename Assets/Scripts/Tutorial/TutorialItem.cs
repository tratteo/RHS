// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : TutorialItem.cs
//
// All Rights Reserved

using UnityEngine;
using UnityEngine.Events;

public class TutorialItem : MonoBehaviour
{
    [SerializeField] private bool stopTime = false;
    [SerializeField] private UnityEvent onFocus;
    [SerializeField] private UnityEvent onReleaseFocus;
    [Header("Animation")]
    [SerializeField] private bool animate = false;
    [SerializeField] private float animateSpeed = 2F;
    [SerializeField] private float animateAmplitude = 10F;
    private Vector3 startPos;

    public bool IsFocused { get; private set; } = false;

    public void Focus()
    {
        if (!IsFocused)
        {
            if (stopTime)
            {
                Time.timeScale = 0F;
            }
            onFocus?.Invoke();
            gameObject.SetActive(true);
            IsFocused = true;
        }
    }

    public void ReleaseFocus(bool callback = true)
    {
        if (IsFocused)
        {
            Time.timeScale = 1F;
            if (callback)
            {
                onReleaseFocus?.Invoke();
            }
            gameObject.SetActive(false);
            IsFocused = false;
        }
    }

    private void Awake()
    {
        startPos = transform.position;
    }

    private void FixedUpdate()
    {
        if (animate)
        {
            transform.position = startPos + animateAmplitude * Mathf.Sin(Time.timeSinceLevelLoad * animateSpeed) * transform.up;
        }
    }
}