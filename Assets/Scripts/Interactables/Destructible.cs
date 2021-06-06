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
    [SerializeField] private bool deactivate = false;
    private ValueContainerSystem resistanceSystem;

    public void Damage(float amount)
    {
        resistanceSystem.Decrease(amount);
    }

    public float GetHealthPercentage() => resistanceSystem.GetPercentage();

    public void Heal(float amount)
    {
        //resistanceSystem.Increase(amount);
    }

    private void OnExhaust()
    {
        if (deactivate)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
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