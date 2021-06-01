// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : EquipmentItem.cs
//
// All Rights Reserved

using GibFrame;
using System.Text;

[System.Serializable]
public abstract class EquipmentItem : Item
{
    public int EquipCost
    {
        get
        {
            return Rarity switch
            {
                RarityType.COMMON => 1,
                RarityType.RARE => 2,
                RarityType.EPIC => 3,
                RarityType.LEGENDARY => 5,
                _ => 0
            };
        }
    }

    protected EquipmentItem(RarityType rarity) : base(rarity)
    {
    }

    public override string ToString()
    {
        StringBuilder res = new StringBuilder(base.ToString());
        res.Append("\n");
        res.Append("Equip cost: <color=green>" + EquipCost + "</color>\n");
        return res.ToString();
    }

    public abstract void EquipTo(object target);

    public abstract void UnequipFrom(object target);

    private class RarityProbability : IProbSelectable
    {
        private float probability;

        public RarityType Rarity { get; private set; }

        public RarityProbability(RarityType rarity, float rawProb)
        {
            Rarity = rarity;
            probability = rawProb;
        }

        public float ProvideSelectProbability() => probability;

        public void SetSelectProbability(float prob)
        {
            probability = prob;
        }
    }
}