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
        if (!GameDaemon.Instance.TryGetResource(GameDaemon.LOADED_LEVEL, out Level loadedLevel, false))
        {
            Debug.LogError("Unable to load the level from the game mode!");
        }
        else
        {
            Instantiate(characterPrefab, Vector2.zero, Quaternion.identity);
            GameObject obj = Instantiate(loadedLevel.Boss.gameObject, Vector2.up * 12F, Quaternion.identity);
            BossEnemy boss = obj.GetComponent<BossEnemy>();
            boss.OnDeath += () => gameEndedBus.Broadcast(true);
            OnBossSpawned?.Invoke(boss);
        }
    }
}