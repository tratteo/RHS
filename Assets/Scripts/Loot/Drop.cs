// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Drop.cs
//
// All Rights Reserved

using GibFrame;

public class Drop : IProbSelectable
{
    private float probability;

    public Item.Factory Factory { get; private set; }

    public Drop(Item.Factory factory, float rawProb = 1F)
    {
        Factory = factory;
        probability = rawProb;
    }

    public static implicit operator Item(Drop drop) => drop.Factory;

    public float ProvideSelectProbability() => probability;

    public void SetSelectProbability(float prob)
    {
        probability = prob;
    }
}