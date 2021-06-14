// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : StatisticsData.cs
//
// All Rights Reserved

using GibFrame.SaveSystem;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StatisticsData : EncryptedData
{
    public static readonly string PATH = Application.persistentDataPath + "statistics.data";
    private readonly Dictionary<string, Statistics> stats;

    public StatisticsData()
    {
        stats = new Dictionary<string, Statistics>();
    }

    public bool ContainsId(string id) => stats.ContainsKey(id);

    public void AddById(string id)
    {
        stats.Add(id, new Statistics());
    }

    public bool ById(string id, out Statistics statistics)
    {
        return stats.TryGetValue(id, out statistics);
    }

    [System.Serializable]
    public class Statistics
    {
        private readonly Dictionary<int, object> values;

        public Statistics()
        {
            values = new Dictionary<int, object>();
        }

        public object ByHash(int hash) => values[hash];

        public bool Update(int hash, object newVal)
        {
            if (values.ContainsKey(hash))
            {
                values[hash] = newVal;
                return true;
            }
            return false;
        }

        public bool Add(int hash, object val)
        {
            if (!values.ContainsKey(hash))
            {
                values.Add(hash, val);
                return true;
            }
            return false;
        }
    }
}