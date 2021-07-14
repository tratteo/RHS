// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : ProjectileGeneratorComponent.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.Performance;
using UnityEngine;

public abstract class ProjectileGeneratorComponent : ProjectileComponent
{
    [Header("Options")]
    [SerializeField] private bool generateOnDestroy = true;
    [SerializeField] private bool continuousGeneration = false;
    [Min(0.2F)] [SerializeField] private float generationTime = 0.5F;
    [Space(10)]
    [SerializeField] private GameObject prefab;
    private UpdateJob generateJob;

    protected GameObject Prefab => prefab;

    public override void CommonUpdate(float deltaTime)
    {
        base.CommonUpdate(deltaTime);
        generateJob?.CommonUpdate(deltaTime);
    }

    protected override void Awake()
    {
        base.Awake();
        if (continuousGeneration)
        {
            generateJob = new UpdateJob(new Callback(Generate), generationTime);
        }
        if (prefab)
        {
            PoolDispatcher.Instance.RequestPool(Categories.PROJECTILES, Prefab, 10);
        }
    }

    protected abstract void Generate();

    protected override void OnEnable()
    {
        base.OnEnable();
        if (generateOnDestroy)
        {
            Parent.OnDestroy += Generate;
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Parent.OnDestroy -= Generate;
    }
}