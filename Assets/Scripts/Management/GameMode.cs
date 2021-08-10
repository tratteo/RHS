// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : GameMode.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.Performance;
using System;
using UnityEngine;

public class GameMode : MonoSingleton<GameMode>, ICommonUpdate, IStatisticsProvider
{
    [SerializeField, Guarded] private BoolEventBus gameEndedBus;
    [SerializeField, Guarded] private GameObject characterPrefab;
    [SerializeField] private bool instantiateBossOnStart = true;
    private float time = 0;

    public event Action<BossEnemy> OnBossSpawned = delegate { };

    public void CommonUpdate(float deltaTime)
    {
        time += deltaTime;
    }

    public Statistic[] GetStats()
    {
        return new Statistic[] { new Statistic(Statistic.TIME, time) };
    }

    public void InstantiateLoadedBoss()
    {
        if (GameDaemon.Instance.TryGetResource(GameDaemon.LOADED_LEVEL, out Level loadedLevel, false) && loadedLevel.Boss)
        {
            GameObject obj = Instantiate(loadedLevel.Boss.gameObject, Vector2.up * 12F, Quaternion.identity);
            BossEnemy boss = obj.GetComponent<BossEnemy>();
            boss.OnDeath += () => gameEndedBus.Broadcast(true);
            OnBossSpawned?.Invoke(boss);
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

    private void Start()
    {
        if (characterPrefab)
        {
            Instantiate(characterPrefab, Vector2.zero, Quaternion.identity);
        }
        if (instantiateBossOnStart)
        {
            InstantiateLoadedBoss();
        }
    }
}