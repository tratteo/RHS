// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : IMergeableItem.cs
//
// All Rights Reserved

public interface IMergeableItem
{
    Item Merge(params Item[] others);

    bool CanMerge(params Item[] other);
}