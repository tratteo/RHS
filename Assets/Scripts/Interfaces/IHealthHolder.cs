// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : IHealthHolder.cs
//
// All Rights Reserved

using UnityEngine;

public interface IHealthHolder
{
    void Damage(Data amount);

    void Heal(Data amount);

    float GetHealthPercentage();

    public class Data
    {
        public GameObject Dealer { get; private set; }

        public float Amount { get; private set; }

        public Data(GameObject dealer, float amount)
        {
            Dealer = dealer;
            Amount = amount;
        }
    }
}