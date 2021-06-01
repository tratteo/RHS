// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Table.cs
//
// All Rights Reserved

using GibFrame;
using System;
using System.Collections.Generic;

public class Table
{
    private readonly List<Drop> drops;

    private readonly Amounts amounts;

    public Table(Amounts amounts, params Drop[] drops)
    {
        this.drops = new List<Drop>();
        this.drops.AddRange(drops);
        this.amounts = amounts;
    }

    public Item[] GetDrops()
    {
        int amount = amounts.GetAmount();
        if (amount <= 0 || drops == null || drops.Count <= 0) return new Item[0];

        Item[] items = new Item[amount];
        drops.NormalizeProbabilities();
        for (int i = 0; i < amount; i++)
        {
            items[i] = drops.SelectWithProbability();
        }
        return items;
    }

    public class Amounts
    {
        private readonly (int amount, float prob)[] amounts;

        public Amounts(params (int amount, float prob)[] values)
        {
            amounts = new (int amount, float prob)[values.Length];
            Array.Copy(values, amounts, values.Length);
            amounts.NormalizeProbabilities((e) => e.prob, (e, p) => e.prob = p);
        }

        public int GetAmount() => amounts.SelectWithProbability((e) => e.prob).amount;
    }
}