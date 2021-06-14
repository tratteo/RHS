// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Statistic.cs
//
// All Rights Reserved

public class Statistic
{
    public int Hash { get; private set; }

    public object Value { get; private set; }

    public Statistic(int hash, object value)
    {
        Hash = hash;
        Value = value;
    }

    public T ValueAs<T>() => (T)Value;
}