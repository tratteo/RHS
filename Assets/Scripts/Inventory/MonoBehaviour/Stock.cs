// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Stock.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

public class Stock : MonoBehaviour
{
    private Transform content = null;

    public void StockIn(ItemUI equipment)
    {
        content = content ?? transform.GetFirstComponentInChildrenWithName<Transform>("Content", true);
        equipment.MoveTo(content);
    }
}