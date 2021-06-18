// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : StatisticsDisplay.cs
//
// All Rights Reserved

using GibFrame;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsDisplay : MonoBehaviour
{
    [SerializeField, Guarded] private Text mainText;

    public void Display(List<Statistic> stats)
    {
        mainText.text = string.Empty;
        string text = string.Empty;
        foreach (Statistic stat in stats)
        {
            text += stat.ToString() + "\n";
        }
        mainText.text = text;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        mainText.text = string.Empty;
    }
}