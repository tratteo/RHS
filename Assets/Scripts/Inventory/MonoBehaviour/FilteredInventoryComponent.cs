// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : FilteredInventoryComponent.cs
//
// All Rights Reserved

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilteredInventoryComponent : InventoryComponent
{
    [SerializeField] private GraphicRaycaster raycaster;
    private Stock stock;
    private Predicate<Item> Filter;

    public void Populate(List<Item> equipment)
    {
        foreach (Item item in equipment)
        {
            //GameObject obj = Instantiate(AssetsProvider.UI.ItemUI_P);
            //obj.transform.localScale = Vector3.one;
            //ItemUI eq = obj.GetComponentInChildren<ItemUI>();
            //eq.Setup(item, raycaster, obj);
            //Insert(eq, false);
        }
    }

    public override bool CanInsert(ItemUI equipment)
    {
        return base.CanInsert(equipment) && Filter(equipment.Item);
    }

    public void SetFilter(Predicate<Item> Filter)
    {
        this.Filter = Filter;
    }

    protected override void PlaceEquipment(ItemUI equipment)
    {
        if (stock == null)
        {
            base.PlaceEquipment(equipment);
        }
        else
        {
            stock.StockIn(equipment);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        stock = GetComponentInChildren<Stock>();
    }

    private void Start()
    {
        List<Item> items;
        if (Filter != null)
        {
            items = Inventory.GetInventory(Filter);
        }
        else
        {
            items = Inventory.GetInventory();
        }
        Populate(items);
        AddOnExtractCallback((item, inventory) => Inventory.RemoveFromInventory(item.Item));
        AddOnInsertCallback((item, inventor) => Inventory.AddToInventory(item.Item));
    }
}