// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Destructible.cs
//
// All Rights Reserved

using GibFrame;
using System;
using UnityEngine;

public class Destructible : MonoBehaviour, IHealthHolder
{
    [SerializeField] private float resistance = 5F;
    private ValueContainerSystem resistanceSystem;

    public event Action OnDeath;

    public void Damage(float amount)
    {
        resistanceSystem.Decrease(amount);
    }

    public float GetHealthPercentage() => resistanceSystem.GetPercentage();

    public void Heal(float amount)
    {
        //resistanceSystem.Increase(amount);
    }

    private void Awake()
    {
        resistanceSystem = new ValueContainerSystem(resistance);
        resistanceSystem.OnExhaust += () => Destroy(gameObject);
    }
}