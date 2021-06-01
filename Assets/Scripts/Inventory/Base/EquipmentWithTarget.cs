// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : EquipmentWithTarget.cs
//
// All Rights Reserved

[System.Serializable]
public abstract class EquipmentWithTarget<Target> : EquipmentItem where Target : class
{
    protected EquipmentWithTarget(RarityType rarity = RarityType.COMMON) : base(rarity)
    {
    }

    public override void EquipTo(object target)
    {
        if (target is Target)
        {
            EquipTo(target as Target);
        }
    }

    public override void UnequipFrom(object target)
    {
        if (target is Target)
        {
            UnequipFrom(target as Target);
        }
    }

    public abstract void EquipTo(Target character);

    public abstract void UnequipFrom(Target character);
}