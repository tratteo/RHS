// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Destructible.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;
using UnityEngine.Events;
using static IHealthHolder;

public class Destructible : MonoBehaviour, IHealthHolder
{
    [SerializeField] private float resistance = 5F;

    [SerializeField] private UnityEvent OnDestroyed = null;
    private ValueContainerSystem resistanceSystem;

    public void Damage(Data amount)
    {
        resistanceSystem.Decrease(amount.Amount);
    }

    public float GetHealthPercentage() => resistanceSystem.GetPercentage();

    public void Heal(Data amount)
    {
        //resistanceSystem.Increase(amount);
    }

    private void OnExhaust()
    {
        if (OnDestroyed != null)
        {
            OnDestroyed?.Invoke();
        }
    }

    private void OnEnable()
    {
        resistanceSystem.OnExhaust += OnExhaust;
    }

    private void Awake()
    {
        resistanceSystem = new ValueContainerSystem(resistance);
    }

    private void OnDisable()
    {
        resistanceSystem.OnExhaust -= OnExhaust;
    }
}