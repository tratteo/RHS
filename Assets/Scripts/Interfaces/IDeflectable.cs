// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : IDeflectable.cs
//
// All Rights Reserved

public interface IDeflectable
{
    bool CanBeDeflected { get; }

    void Deflect(IAgent agent);
}