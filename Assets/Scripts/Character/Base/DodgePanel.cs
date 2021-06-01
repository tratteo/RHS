// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : DodgePanel.cs
//
// All Rights Reserved

using GibFrame;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DodgePanel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    private SwipeInfo swipe;

    public event Action<Vector2, float> OnDodgePerformed;

    public void OnPointerDown(PointerEventData eventData)
    {
        swipe.Start(new Vector2(eventData.position.x, eventData.position.y));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        swipe.End(new Vector2(eventData.position.x, eventData.position.y));
        BroadcastDodge();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        swipe.End(new Vector2(eventData.position.x, eventData.position.y));
        BroadcastDodge();
    }

    private void BroadcastDodge()
    {
        if (!swipe.IsValid) return;
        if (swipe.Duration <= 1F && swipe.Distance > 70F)
        {
            float force = GMath.Map(swipe.Distance, (0F, Screen.width / 6F), (20F, 50F));
            OnDodgePerformed?.Invoke(swipe.Direction, force);
        }
        swipe.Reset();
    }

    private void Awake()
    {
        swipe = new SwipeInfo();
    }

    private class SwipeInfo
    {
        private float startTime;

        private float endTime;

        private bool startSet = false;

        private bool endSet = false;

        public Vector2 StartPos { get; private set; }

        public Vector2 EndPos { get; private set; }

        public float Duration => endTime - startTime;

        public float Distance => (EndPos - StartPos).magnitude;

        public float AverageVelocity => Duration == 0 ? 0 : Distance / Duration;

        public Vector2 Direction => (EndPos - StartPos).normalized;

        public bool IsValid => startSet && endSet;

        public void Reset()
        {
            StartPos = Vector2.zero;
            EndPos = Vector2.zero;
            startTime = 0;
            endTime = 0;
            startSet = false;
            endSet = false;
        }

        public void Start(Vector2 pos)
        {
            StartPos = pos;
            startTime = Time.timeSinceLevelLoad;
            startSet = true;
        }

        public void End(Vector2 pos)
        {
            EndPos = pos;
            endTime = Time.timeSinceLevelLoad;
            endSet = true;
        }
    }
}