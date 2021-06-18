// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Statistic.cs
//
// All Rights Reserved

using System;
using System.Text;

public class Statistic : IEquatable<Statistic>
{
    public const int HEALTH_PERCENTAGE = 0x0;
    public const int TIME = 0x1;
    public const int PARRIES = 0x2;
    public const int DISTANCE_TRAVELED = 0x3;

    public int Hash { get; private set; }

    public object Value { get; set; }

    public Statistic(int hash, object value)
    {
        Hash = hash;
        Value = value;
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder();
        switch (Hash)
        {
            case HEALTH_PERCENTAGE:
                Format("Health", builder, "%", ValueAs<float>() * 100F);
                break;

            case TIME:
                Format("Time", builder, "seconds", ValueAs<float>());
                break;

            case PARRIES:
                Format("Perfect parries", builder, "", ValueAs<int>());
                break;

            case DISTANCE_TRAVELED:
                Format("Distance traveled", builder, " meters", ValueAs<float>());
                break;
        };
        return builder.ToString();
    }

    public T ValueAs<T>() => (T)Value;

    public bool Equals(Statistic other)
    {
        return Hash.Equals(other.Hash);
    }

    private void Format(string name, StringBuilder builder, string suffix, object value)
    {
        builder.Append(name).Append(": ").Append("<color=green>");
        if (value is float)
        {
            builder.AppendFormat("{0:F2}", value);
        }
        else if (value is int)
        {
            builder.AppendFormat("{0:0}", value);
        }
        builder.Append(" ").Append(suffix).Append("</color>");
    }
}