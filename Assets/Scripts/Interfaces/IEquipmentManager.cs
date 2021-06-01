// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : IEquipmentManager.cs
//
// All Rights Reserved

public interface IEquipmentManager
{
    bool CanEquip(ItemUI item, InventoryComponent inventory);
}