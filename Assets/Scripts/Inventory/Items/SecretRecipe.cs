// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : SecretRecipe.cs
//
// All Rights Reserved

using System.Text;

[System.Serializable]
public class SecretRecipe : ResearchItem
{
    public SecretRecipe(RarityType rarity) : base(rarity)
    {
    }

    public override string GetName()
    {
        return "Secret Recipe";
    }

    public float GetResearchTimeMultiplier()
    {
        return Rarity switch
        {
            RarityType.COMMON => 0.8F,
            RarityType.RARE => 0.6F,
            RarityType.EPIC => 0.5F,
            RarityType.LEGENDARY => 0.25F,
            _ => 1F
        };
    }

    public override string ToString()
    {
        StringBuilder res = new StringBuilder(base.ToString());
        res.Append("\n\n");
        res.AppendFormat("Decrease by <color=green>{0}%</color> the time required for the nutrition researches\n", (1 - GetResearchTimeMultiplier()) * 100);
        return res.ToString();
    }
}