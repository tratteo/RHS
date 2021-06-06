// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : CharacterComponent.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.Performance;
using UnityEngine;

public class CharacterComponent : MonoBehaviour, ICommonUpdate, ICommonFixedUpdate
{
    [Header("Channel")]
    [SerializeField, Guarded] private CharacterChannelEvent eventChannel;

    public Rigidbody2D Rigidbody { get; private set; }

    protected CharacterChannelEvent EventBus => eventChannel;

    protected Collider2D Collider { get; private set; }

    protected CharacterManager Manager { get; private set; } = null;

    protected bool Active { get; private set; } = true;

    public void SetManager(CharacterManager manager)
    {
        Manager = manager;
    }

    public virtual void CommonFixedUpdate(float fixedDeltaTime)
    {
    }

    public virtual void CommonUpdate(float deltaTime)
    {
    }

    protected virtual void OnEnable()
    {
        EventBus.GameEndedEvent.Invocation += OnDeath;
        CommonUpdateManager.Register(this);
    }

    protected virtual void OnDeath(bool win)
    {
        Active = false;
    }

    protected virtual void OnDisable()
    {
        EventBus.GameEndedEvent.Invocation -= OnDeath;
        CommonUpdateManager.Unregister(this);
    }

    protected virtual void Awake()
    {
        Collider = GetComponent<Collider2D>();
        Rigidbody = GetComponent<Rigidbody2D>();
    }

    protected virtual void Start()
    {
        if (!Manager)
        {
            Debug.LogError(ToString() + " is not linked to any character manager!");
        }
    }
}