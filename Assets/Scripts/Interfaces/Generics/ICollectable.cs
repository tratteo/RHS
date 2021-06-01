// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : ICollectable.cs
//
// All Rights Reserved

public interface ICollectable<Target> : ICollectable where Target : class
{
    void CollectAndApply(Target target);
}