// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : ButtonAnimAdapter.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.UI;
using UnityEngine;

public class ButtonAnimAdapter : MonoBehaviour
{
    [Range(0F, 1F)] [SerializeField] private float animRatio = 2F / 38F;
    private RectTransform subImage;
    private GButton parent;
    private float stride;
    private bool pressed = false;

    private Vector3 animatedPos;
    private Vector3 defaultPos;
    private RectTransform parentRectTransform;

    private void Start()
    {
        new Timer(this, 0.01F, true, true, new Callback(Initialize));
    }

    private void Initialize()
    {
        subImage = GetComponent<RectTransform>();
        parent = GetComponentInParent<GButton>();
        parentRectTransform = parent.gameObject.GetComponent<RectTransform>();
        defaultPos = subImage.anchoredPosition;
        if (parent != null)
        {
            parent.AddOnPressedCallback(new Callback(() =>
            {
                stride = parentRectTransform.sizeDelta.y * animRatio;
                animatedPos = Vector3.down * stride;
                pressed = true;
                subImage.anchoredPosition = animatedPos;
            }));

            parent.AddOnPointerExitCallback(new Callback(() =>
            {
                subImage.anchoredPosition = defaultPos;
            }));

            parent.AddOnCancelCallback(new Callback(() =>
            {
                pressed = false;
                subImage.anchoredPosition = defaultPos;
            }));
            parent.AddOnReleasedCallback(new Callback(() =>
            {
                pressed = false;
                subImage.anchoredPosition = defaultPos;
            }));
            parent.AddOnPointerEnterCallback(new Callback(() =>
            {
                if (pressed)
                {
                    stride = parentRectTransform.sizeDelta.y * animRatio;
                    animatedPos = Vector3.down * stride;
                    subImage.anchoredPosition = animatedPos;
                }
            }));
        }
    }
}