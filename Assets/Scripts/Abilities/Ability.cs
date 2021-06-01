// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Ability.cs
//
// All Rights Reserved

using System;
using System.Collections;
using UnityEngine;

public abstract class Ability<TParent> : ScriptableObject, ICooldownOwner, IDescribable where TParent : MonoBehaviour
{
    [SerializeField] private SerializedDescribable describable;
    [SerializeField] private float cooldown;
    private float cooldownTimer;
    private Coroutine coroutine;

    public bool IsPerforming { get; private set; } = false;

    public event Action OnComplete = delegate { };

    public event Action OnPerform = delegate { };

    public bool IsInCooldown() => cooldownTimer > 0F;

    public virtual bool CanPerform(TParent parent)
    {
        return !IsInCooldown() && !IsPerforming;
    }

    public float GetCooldown() => cooldown;

    public float GetCooldownPercentage() => cooldownTimer / cooldown;

    public Sprite GetIcon() => describable.GetIcon();

    public void HardStop(TParent parent)
    {
        if (coroutine != null)
        {
            parent.StopCoroutine(coroutine);
            OnStopped(parent);
        }
    }

    public bool Perform(TParent parent)
    {
        if (CanPerform(parent))
        {
            coroutine = parent.StartCoroutine(Execute_C(parent));
            IsPerforming = true;
            OnPerform?.Invoke();
            OnPerform = null;
            return true;
        }
        return false;
    }

    public void Step(float deltaTime)
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= deltaTime;
        }
        else
        {
            cooldownTimer = 0F;
        }
    }

    public string GetId() => describable.GetId();

    public string GetName() => describable.GetName();

    public void Reset()
    {
        ResetCooldown();
        OnPerform = null;
        OnComplete = null;
        IsPerforming = false;
    }

    public void ResetCooldown()
    {
        cooldownTimer = 0F;
    }

    public string GetDescription() => describable.GetDescription();

    protected virtual void OnStopped(TParent parent)
    {
        Complete();
    }

    protected virtual void Complete()
    {
        coroutine = null;
        IsPerforming = false;
        OnComplete?.Invoke();
        OnComplete = null;
        cooldownTimer = cooldown;
    }

    protected abstract IEnumerator Execute_C(TParent parent);

    private void OnEnable()
    {
        IsPerforming = false;
        ResetCooldown();
    }
}