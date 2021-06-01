// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : InventoryComponent.cs
//
// All Rights Reserved

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryComponent : MonoBehaviour
{
    [SerializeField] private int maxItems = 1;
    [SerializeField] private bool canDragInItem = true;

    public ItemUI this[int index]
    {
        get
        {
            if (Items != null && Items.Count > 0)
            {
                return Items.ElementAt(index);
            }
            else
                return null;
        }
    }

    public int MaxItems { get => maxItems; }

    public List<ItemUI> Items { get; private set; }

    public bool CanDragItem { get => canDragInItem; }

    private event Action<ItemUI, InventoryComponent> OnExtract;

    private event Action<ItemUI, InventoryComponent> OnInsert;

    public bool CanAcceptItem() => Items.Count < MaxItems || MaxItems == -1;

    public void AddOnExtractCallback(Action<ItemUI, InventoryComponent> callback)
    {
        OnExtract += callback;
    }

    public void AddOnInsertCallback(Action<ItemUI, InventoryComponent> callback)
    {
        OnInsert += callback;
    }

    public void MoveAllTo(InventoryComponent other, bool callback)
    {
        if (Items != null)
        {
            List<ItemUI> equipment = new List<ItemUI>(Items);
            foreach (ItemUI eq in equipment)
            {
                if (Extract(eq, callback))
                {
                    other.Insert(eq, callback);
                }
            }
        }
    }

    public virtual bool Insert(ItemUI equipment, bool callback = true)
    {
        if (CanAcceptItem())
        {
            if (!Items.Contains(equipment))
            {
                Items.Add(equipment);
                equipment.CurrentInventory = this;
                PlaceEquipment(equipment);
                if (callback)
                {
                    OnInsert?.Invoke(equipment, this);
                }
                return true;
            }
            return false;
        }
        return false;
    }

    public ItemUI Head()
    {
        if (Items != null && Items.Count > 0)
        {
            return Items.ElementAt(0);
        }
        return null;
    }

    public bool CanExtract(ItemUI equipment)
    {
        return Items != null && Items.Contains(equipment);
    }

    public virtual bool CanInsert(ItemUI equipment)
    {
        return Items != null && !Items.Contains(equipment) && CanAcceptItem();
    }

    public ItemUI Poll()
    {
        if (Items != null && Items.Count > 0)
        {
            ItemUI res = Items.ElementAt(0);
            Items.RemoveAt(0);
            return res;
        }
        return null;
    }

    public bool Extract(ItemUI equipment, bool callback = true)
    {
        if (Items.Remove(equipment))
        {
            if (callback)
            {
                OnExtract?.Invoke(equipment, this);
            }
            return true;
        }
        return false;
    }

    public void Clear()
    {
        foreach (ItemUI item in Items)
        {
            DestroyImmediate(item.gameObject);
        }
        Items.Clear();
    }

    protected virtual void PlaceEquipment(ItemUI equipment)
    {
        equipment.MoveTo(transform);
        if (MaxItems == 1)
        {
            equipment.LocalPosition = Vector3.zero;
        }
    }

    protected virtual void Awake()
    {
        Items = new List<ItemUI>();
    }
}