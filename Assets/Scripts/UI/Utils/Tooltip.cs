// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Tooltip.cs
//
// All Rights Reserved

using GibFrame;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    private Image panel;
    private Text description;
    private bool autoHide;

    private event Action<Tooltip> OnDestroyed;

    public static Tooltip Inflate(string description, TooltipPreferences preferences = null, GameObject prefab = null)
    {
        if (prefab == null)
        {
            //prefab = ResourcesProvider.UI.Tooltip_P;
        }
        GameObject obj = Instantiate(prefab);
        Tooltip tooltip = obj.GetComponentInChildren<Tooltip>();
        tooltip.Setup(description, preferences);
        return tooltip;
    }

    public void AddOnDestroyedCallback(Action<Tooltip> action)
    {
        OnDestroyed += action;
    }

    public void RemoveOnDestroyedCallback(Action<Tooltip> action)
    {
        OnDestroyed -= action;
    }

    public void Destroy()
    {
        OnDestroyed?.Invoke(this);
        Destroy(gameObject);
    }

    public void Setup(string description, TooltipPreferences preferences = null)
    {
        this.description.text = description;

        if (preferences == null)
        {
            preferences = TooltipPreferences.CreateTrivial();
        }
        panel.rectTransform.localPosition = preferences.ScreenPos;

        panel.rectTransform.sizeDelta = preferences.SizeDelta;
        autoHide = preferences.AutoHide;
        if (preferences.SmartPosition)
        {
            SmartPosition(preferences);
        }
    }

    private void SmartPosition(TooltipPreferences preferences)
    {
        float edgeX;
        float edgeY;
        if ((edgeY = preferences.ScreenPos.y + preferences.SizeDelta.y) > (Screen.height / 2) + preferences.ScreenPadding.y)
        {
            panel.rectTransform.localPosition += Vector3.down * (panel.rectTransform.sizeDelta.y + preferences.ElemPadding);
        }

        if ((edgeX = preferences.ScreenPos.x + (preferences.SizeDelta.x / 2)) > (Screen.width / 2) - preferences.ScreenPadding.x)
        {
            float stride = GMath.Abs((Screen.width / 2) - edgeX - preferences.ScreenPadding.x);
            panel.rectTransform.anchoredPosition = panel.rectTransform.anchoredPosition + Vector2.left * stride;
        }
        else if ((edgeX = preferences.ScreenPos.x - (preferences.SizeDelta.x / 2)) < (-Screen.width / 2) + preferences.ScreenPadding.x)
        {
            float stride = GMath.Abs((-Screen.width / 2) - edgeX + preferences.ScreenPadding.x);
            panel.rectTransform.anchoredPosition = panel.rectTransform.anchoredPosition + Vector2.right * stride;
        }
    }

    private void Awake()
    {
        description = transform.GetFirstComponentInChildrenWithName<Text>("Description", true);
        panel = transform.GetFirstComponentInChildrenWithName<Image>("Panel", true);
    }

    private void Update()
    {
        if (autoHide && (Input.touchCount > 0 || Input.GetMouseButtonDown(0)))
        {
            List<RaycastResult> results = new List<RaycastResult>();
            PointerEventData eventData = new PointerEventData(EventSystem.current);
            if (Input.GetMouseButtonDown(0))
            {
                eventData.position = Input.mousePosition;
            }
            if (Input.touchCount > 0)
            {
                eventData.position = Input.GetTouch(0).position;
            }

            EventSystem.current.RaycastAll(eventData, results);
            bool shouldDestroy = true;
            foreach (RaycastResult result in results)
            {
                if (result.gameObject == panel.gameObject)
                {
                    shouldDestroy = false;
                    break;
                }
            }
            if (shouldDestroy)
            {
                Destroy();
            }
        }
    }

    public class TooltipPreferences
    {
        public bool AutoHide { get; private set; }

        public Vector2 ScreenPadding { get; private set; }

        public Vector2 ScreenPos { get; private set; }

        public Vector2 SizeDelta { get; private set; }

        public float ElemPadding { get; private set; }

        public bool SmartPosition { get; private set; }

        private TooltipPreferences()
        {
            AutoHide = true;
            ScreenPos = Vector2.zero;
            SizeDelta = new Vector2(800, 400);
            SmartPosition = true;
            ScreenPadding = Vector2.zero;
        }

        public static Builder CreateTrivial() => new Builder();

        public class Builder
        {
            private readonly TooltipPreferences tooltip;

            public Builder()
            {
                tooltip = new TooltipPreferences();
            }

            public static implicit operator TooltipPreferences(Builder builder) => builder.tooltip;

            public Builder Size(float x, float y)
            {
                return Size(new Vector2(x, y));
            }

            public Builder ElementPadding(float padding)
            {
                tooltip.ElemPadding = padding;
                return this;
            }

            public Builder Size(Vector2 size)
            {
                tooltip.SizeDelta = size;
                return this;
            }

            public Builder Padding(Vector2 padding)
            {
                tooltip.ScreenPadding = padding;
                return this;
            }

            public Builder Padding(float x, float y)
            {
                return Padding(new Vector2(x, y));
            }

            public Builder At(Vector2 screenPos)
            {
                tooltip.ScreenPos = screenPos;
                return this;
            }

            public Builder At(float x, float y)
            {
                return At(new Vector2(x, y));
            }

            public Builder SmartPositioning(bool smartResizing)
            {
                tooltip.SmartPosition = smartResizing;
                return this;
            }

            public Builder AutoHide(bool autoHide)
            {
                tooltip.AutoHide = autoHide;
                return this;
            }
        }
    }
}