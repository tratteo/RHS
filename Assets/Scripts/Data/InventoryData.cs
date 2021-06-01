// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : InventoryData.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.SaveSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class InventoryData : EncryptedData
{
    public static readonly string PATH = Application.persistentDataPath + "/inventory.data";
    private readonly List<Item> inventory;

    public InventoryData()
    {
        inventory = new List<Item>();
    }

    public List<Item> GetInventory(params Predicate<Item>[] predicates)
    {
        if (predicates.Length > 0)
        {
            return inventory.GetPredicatesMatchingObjects(predicates).ToList();
        }
        else
        {
            return inventory;
        }
    }

    public bool Contains(Item item)
    {
        return GetInventory().FindAll((i) => i.Equals(item)).Count > 0;
    }

    public List<T> GetConvertedInventory<T>(Func<Item, T> converter, params Predicate<Item>[] predicates)
    {
        List<Item> items = GetInventory(predicates);
        return items.ConvertAll((i) => converter(i));
    }

    public Dictionary<T, List<E>> GetMappedInventory<T, E>(Func<E, T> mapper, params Predicate<Item>[] predicates) where E : Item
    {
        List<Item> res = GetInventory(predicates);
        Dictionary<T, List<E>> map = new Dictionary<T, List<E>>();

        foreach (Item item in res)
        {
            E elem;
            if (item is E)
            {
                elem = item as E;
                T mapped = mapper(elem);
                if (!map.ContainsKey(mapped))
                {
                    map.Add(mapped, new List<E>() { elem });
                }
                else
                {
                    map[mapped].Add(elem);
                }
            }
        }
        return map;
    }

    public int AddToInventory(params Item[] items)
    {
        int count = 0;
        foreach (Item item in items)
        {
            if (!inventory.Contains(item))
            {
                inventory.Add(item);
                count++;
            }
        }
        return count;
    }

    public int RemoveFromInventory(params Item[] items)
    {
        int count = 0;
        foreach (Item item in items)
        {
            if (inventory.Remove(item))
            {
                count++;
            }
        }
        return count;
    }
}