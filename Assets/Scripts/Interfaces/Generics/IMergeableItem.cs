// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : IMergeableItem.cs
//
// All Rights Reserved

public interface IMergeableItem<E> : IMergeableItem where E : Item
{
    E Merge(params E[] others);
}