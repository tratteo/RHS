// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : ValueContainerBar.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.Performance;
using GibFrame.UI;
using UnityEngine;
using UnityEngine.UI;

public class ValueContainerBar : MonoBehaviour, ICommonUpdate
{
    public enum ValueToastBindType { INCREASE, DECREASE, CHANGE }

    [SerializeField] private Image fillImage;
    [SerializeField] private Image secondFill;
    [SerializeField] private Text amountText;
    [SerializeField] private ToastScript valueToast;

    private bool toastBinded = false;
    private bool enqueueToast = true;
    private Vector2 durations = new Vector2(0.1F, 0.5F);

    private float delayedValue = 0;
    private ValueToastBindType toastBindType;

    private ValueContainerSystem bindedSystem;

    public bool Binded { get; private set; } = false;

    public void Bind(ValueContainerSystem system)
    {
        Unbind();
        bindedSystem = system;
        bindedSystem.OnDecrease += OnIncrease;
        bindedSystem.OnIncrease += OnIncrease;
        delayedValue = bindedSystem.CurrentValue;
        UpdateUI();
        Binded = true;
    }

    public void EnableValueToast(ValueToastBindType toastBindType, Vector2 durations, bool enqueueToast = true)
    {
        this.toastBindType = toastBindType;
        this.durations = durations;
        toastBinded = true;
        this.enqueueToast = enqueueToast;
    }

    public void UnbindToast()
    {
        toastBinded = false;
    }

    public void Unbind()
    {
        if (bindedSystem != null)
        {
            bindedSystem.OnDecrease -= OnDecrease;
            bindedSystem.OnIncrease -= OnIncrease;
        }
        UnbindToast();
        bindedSystem = null;
        Binded = false;
    }

    public void CommonUpdate(float deltaTime)
    {
        if (Binded && secondFill)
        {
            delayedValue = Mathf.Lerp(delayedValue, bindedSystem.CurrentValue, 0.02F);
            secondFill.fillAmount = delayedValue / bindedSystem.MaxValue;
        }
    }

    private void OnEnable()
    {
        CommonUpdateManager.Register(this);
    }

    private void OnDisable()
    {
        CommonUpdateManager.Unregister(this);
    }

    private void OnDecrease(float amount)
    {
        UpdateUI();
        if (toastBinded)
        {
            if (toastBindType == ValueToastBindType.CHANGE || toastBindType == ValueToastBindType.DECREASE)
            {
                if (enqueueToast && valueToast != null)
                {
                    valueToast.EnqueueToast(amount.ToString(), null, durations.y, new Vector2(durations.x, durations.x));
                }
                else
                {
                    valueToast.ShowToast(amount.ToString(), null, durations.y, new Vector2(durations.x, durations.x));
                }
            }
        }
    }

    private void UpdateUI()
    {
        if (bindedSystem == null) return;
        if (fillImage != null)
        {
            fillImage.fillAmount = bindedSystem.GetPercentage();
        }
        if (amountText != null)
        {
            if (bindedSystem.TextFormat.Equals(""))
            {
                amountText.text = bindedSystem.CurrentValue.ToString();
            }
            else
            {
                amountText.text = string.Format(bindedSystem.TextFormat, bindedSystem.CurrentValue);
            }
        }
    }

    private void OnIncrease(float amount)
    {
        UpdateUI();
        if (toastBinded)
        {
            if (toastBindType == ValueToastBindType.CHANGE || toastBindType == ValueToastBindType.INCREASE)
            {
                if (enqueueToast && valueToast != null)
                {
                    valueToast.EnqueueToast(amount.ToString(), null, durations.y, new Vector2(durations.x, durations.x));
                }
                else
                {
                    valueToast.ShowToast(amount.ToString(), null, durations.y, new Vector2(durations.x, durations.x));
                }
            }
        }
    }

    private void OnDestroy()
    {
        Unbind();
    }
}