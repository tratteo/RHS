// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Ability.cs
//
// All Rights Reserved

using GibFrame.Performance;
using System;
using System.Collections;
using UnityEngine;

public abstract class Ability<TParent> : MonoBehaviour, ICooldownOwner, IDescribable, ICommonUpdate where TParent : MonoBehaviour
{
    [SerializeField] private SerializedDescribable describable;
    [SerializeField] private float cooldown;
    private float cooldownTimer;
    private Coroutine coroutine;

    public bool IsPerforming { get; private set; } = false;

    public TParent Parent { get; private set; }

    public event Action OnComplete = delegate { };

    public event Action OnPerform = delegate { };

    public static Ability<TParent> Attach(GameObject abilityPrefab, TParent parent)
    {
        Ability<TParent> ability;
        GameObject obj = Instantiate(abilityPrefab);
        if ((ability = obj.GetComponent<Ability<TParent>>()) != null)
        {
            ability.Parent = parent;
            Transform holder;
            if (!(holder = parent.transform.Find("AbilitiesHolder")))
            {
                GameObject holderObj = new GameObject
                {
                    name = "AbilitiesHolder"
                };
                holderObj.transform.SetParent(parent.transform);
                holder = holderObj.transform;
            }
            obj.transform.SetParent(holder);
            return ability;
        }
        else
        {
            Debug.LogError("Unable to get the right type of TParent from ability prefab");
            return null;
        }
    }

    public bool IsInCooldown() => cooldownTimer > 0F;

    public virtual bool CanPerform()
    {
        return !IsInCooldown() && !IsPerforming;
    }

    public float GetCooldown() => cooldown;

    public float GetCooldownPercentage() => cooldownTimer / cooldown;

    public void HardStop()
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
            OnStopped();
        }
    }

    public bool Perform()
    {
        if (CanPerform())
        {
            coroutine = StartCoroutine(Execute_C());
            IsPerforming = true;
            OnPerform?.Invoke();
            OnPerform = null;
            return true;
        }
        return false;
    }

    #region Describable

    public string GetId() => describable.GetId();

    public Sprite GetIcon() => describable.GetIcon();

    public string GetName() => describable.GetName();

    public string GetDescription() => describable.GetDescription();

    #endregion Describable

    public void ResetCooldown()
    {
        cooldownTimer = 0F;
    }

    public virtual void CommonUpdate(float deltaTime)
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

    protected virtual void OnStopped()
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

    protected abstract IEnumerator Execute_C();

    private void OnEnable()
    {
        CommonUpdateManager.Register(this);
    }

    private void OnDisable()
    {
        CommonUpdateManager.Unregister(this);
    }
}