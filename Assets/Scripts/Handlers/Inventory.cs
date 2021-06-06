// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Inventory.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.SaveSystem;
using System;
using System.Collections.Generic;

public static class Inventory
{
    private static InventoryData inventoryData;

    public static event Action<Item[]> OnAddItems = delegate { };

    public static event Action<Item[]> OnRemovedItems = delegate { };

    public static void MarkDirty()
    {
        SaveManager.Async.MarkDirty((inventoryData, InventoryData.PATH));
    }

    public static void AddToInventory(params Item[] items)
    {
        List<Item> added = new List<Item>();
        items.ForEach(i =>
        {
            if (inventoryData.AddToInventory(i) > 0)
            {
                added.Add(i);
            }
        });
        if (added.Count > 0)
        {
            MarkDirty();
            OnAddItems?.Invoke(added.ToArray());
        }
    }

    public static int RemoveFromInventory(params Item[] items)
    {
        List<Item> removed = new List<Item>();

        items.ForEach(i =>
        {
            if (inventoryData.RemoveFromInventory(i) > 0)
            {
                removed.Add(i);
            }
        });
        if (removed.Count > 0)
        {
            MarkDirty();
            OnRemovedItems?.Invoke(removed.ToArray());
        }
        return removed.Count;
    }

    public static List<Item> GetInventory(params Predicate<Item>[] predicate)
    {
        return inventoryData.GetInventory(predicate);
    }

    public static List<T> GetConvertedInventory<T>(Func<Item, T> converter, params Predicate<Item>[] predicates)
    {
        return inventoryData.GetConvertedInventory(converter, predicates);
    }

    public static Dictionary<T, List<E>> GetMappedInventory<T, E>(Func<E, T> mapper, params Predicate<Item>[] predicates) where E : Item
    {
        return inventoryData.GetMappedInventory<T, E>(mapper, predicates);
    }

    public static void Initialize()
    {
        InventoryData inv = new InventoryData();
        inventoryData = SaveManager.LoadOrInitialize(inv, InventoryData.PATH);
    }
}