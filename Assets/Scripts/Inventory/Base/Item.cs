// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Item.cs
//
// All Rights Reserved

using GibFrame;
using System;
using System.Text;
using UnityEngine;

[Serializable]
public abstract class Item
{
    public enum RarityType { COMMON, RARE, EPIC, LEGENDARY }

    protected RarityType rarity;
    private static readonly (RarityType rarity, float probability)[] Rarities = new (RarityType, float)[]
    {
        (RarityType.COMMON,0.5F),
        (RarityType.RARE, 0.3F),
        (RarityType.EPIC, 0.15F),
        (RarityType.LEGENDARY, 0.05F)
    };

    public RarityType Rarity { get => rarity; protected set { rarity = value; } }

    protected Item(RarityType rarity)
    {
        this.rarity = rarity;
    }

    public static RarityType GetRandomRarity()
    {
        Rarities.NormalizeProbabilities((e) => e.probability, (e, p) => e.probability = p);
        return Rarities.SelectWithProbability((e) => e.probability).rarity;
    }

    public static bool CanEventuallyMerge(Item item, Predicate<Item> AddOn, params Item[] other)
    {
        if (!IsItemEventuallyMergeable(item)) return false;
        for (int i = 0; i < other.Length; i++)
        {
            if (!IsItemEventuallyMergeable(other[i])) return false;
            if (AddOn != null)
            {
                if (!AddOn(other[i])) return false;
            }
            Debug.Log("Checking: " + item.rarity + " with " + other[i].rarity);
            if (!item.GetType().Equals(other[i].GetType()) || !item.Rarity.Equals(other[i].Rarity))
            {
                return false;
            }
        }
        return true;
    }

    public virtual string GetName()
    {
        return GetType().ToString();
    }

    public override string ToString()
    {
        StringBuilder res = new StringBuilder();
        res.AppendFormat("Rarity: <color=green>{0}</color>", Enum.GetName(typeof(RarityType), Rarity));
        res.AppendFormat("\nSell price: <color=orange>{0}</color>", GetSellPrice());
        return res.ToString();
    }

    public virtual int GetSellPrice()
    {
        return Rarity switch
        {
            RarityType.COMMON => 1,
            RarityType.RARE => 2,
            RarityType.EPIC => 5,
            RarityType.LEGENDARY => 10,
            _ => 0
        };
    }

    protected RarityType NextRarity(RarityType current)
    {
        int index = (int)current;
        int length = System.Enum.GetValues(typeof(RarityType)).Length;
        index = index + 1 > length - 1 ? length - 1 : index + 1;
        return (RarityType)index;
    }

    private static bool IsItemEventuallyMergeable(Item item)
    {
        return !(item == null) && item is IMergeableItem && !item.Rarity.Equals(RarityType.LEGENDARY);
    }

    public class Factory
    {
        private readonly Func<Item> Crafter;

        public Factory(Func<Item> Crafter)
        {
            this.Crafter = Crafter;
        }

        public static implicit operator Item(Factory factory) => factory.Craft();

        public Item Craft() => Crafter();
    }
}