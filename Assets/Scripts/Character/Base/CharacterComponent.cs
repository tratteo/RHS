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
    [Header("Channels")]
    [SerializeField, Guarded] private BoolEventBus gameEndedBus;

    [SerializeField, Guarded] private InputEventBus inputBus;

    public Rigidbody2D Rigidbody { get; private set; }

    protected AnimatorDriver AnimatorDriver { get; private set; }

    protected BoolEventBus GameEndedBus => gameEndedBus;

    protected InputEventBus InputBus => inputBus;

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

    protected virtual void OnInput(Inputs.InputData data)
    {
    }

    protected virtual void OnGameEnded(bool win)
    {
        Active = false;
    }

    protected virtual void OnEnable()
    {
        inputBus.OnEvent += OnInput;
        gameEndedBus.OnEvent += OnGameEnded;
        CommonUpdateManager.Register(this);
    }

    protected virtual void OnDisable()
    {
        inputBus.OnEvent -= OnInput;
        gameEndedBus.OnEvent -= OnGameEnded;
        CommonUpdateManager.Unregister(this);
    }

    protected virtual void Awake()
    {
        Collider = GetComponent<Collider2D>();
        Rigidbody = GetComponent<Rigidbody2D>();
        AnimatorDriver = GetComponent<AnimatorDriver>();
    }

    protected virtual void Start()
    {
        if (!Manager)
        {
            Debug.LogError(ToString() + " is not linked to any character manager!");
        }
    }
}