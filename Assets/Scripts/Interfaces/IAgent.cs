// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : IAgent.cs
//
// All Rights Reserved

public interface IAgent
{
    public enum FactionRelation { HOSTILE, FRIENDLY }

    FactionRelation GetFactionRelation();
}