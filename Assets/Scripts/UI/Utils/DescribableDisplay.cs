// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : DescribableDisplay.cs
//
// All Rights Reserved

using UnityEngine;
using UnityEngine.UI;

public class DescribableDisplay : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Text nameTxt;
    [SerializeField] private Text descriptionTxt;
    [SerializeField] private Text idTxt;

    public void Describe(IDescribable describable)
    {
        if (icon)
        {
            icon.sprite = describable.GetIcon();
        }
        if (nameTxt)
        {
            nameTxt.text = describable.GetName();
        }
        if (descriptionTxt)
        {
            descriptionTxt.text = describable.GetDescription();
        }
        if (idTxt)
        {
            idTxt.text = describable.GetId();
        }
    }

    public void SetActive(bool state)
    {
        gameObject.SetActive(state);
    }
}