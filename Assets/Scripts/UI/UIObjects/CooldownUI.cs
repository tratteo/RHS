// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : CooldownUI.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [SerializeField, Guarded] private Image image;
    [SerializeField, Guarded] private Text amountText;
    [SerializeField, Guarded] private ICooldownOwner cooldownHolder;

    public void Bind(ICooldownOwner cooldownHolder)
    {
        this.cooldownHolder = cooldownHolder;
    }

    private void Awake()
    {
        if (!image)
        {
            image = GetComponent<Image>();
        }
        if (!amountText)
        {
            amountText = GetComponentInChildren<Text>();
        }
    }

    private void Update()
    {
        if (cooldownHolder != null)
        {
            image.fillAmount = 1F - cooldownHolder.GetCooldownPercentage();
            if (cooldownHolder is IMultiCooldownOwner multiCooldown && amountText)
            {
                amountText.text = multiCooldown.GetResourcesAmount().ToString();
            }
            else
            {
                amountText.text = "";
            }
        }
    }
}