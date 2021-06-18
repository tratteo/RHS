// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : StatisticsHub.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.SaveSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatisticsHub : MonoSingleton<StatisticsHub>
{
    [Header("Channels")]
    [SerializeField, Guarded] private BoolEventBus gameEndChannel;
    [SerializeField, Guarded] private SessionStatisticsEventBus statisticsChannel;
    private List<Statistic> sessionStats;
    private BossEnemy boss;

    //public Statistic GetStat(int hash)
    //{
    //    return sessionStats.Find(s => s.Hash.Equals(hash));
    //}

    //public void NotifyStatistic(int hash, object value)
    //{
    //    Statistic stat = sessionStats.Find(s => s.Hash.Equals(hash));
    //    if (stat != null)
    //    {
    //        stat.Value = value;
    //    }
    //    else
    //    {
    //        stat = new Statistic(hash, value);
    //        sessionStats.Add(stat);
    //    }
    //}

    private void OnEnable()
    {
        GameMode.Instance.OnBossSpawned += OnBossSpawned;
        gameEndChannel.OnEvent += OnGameEnd;
    }

    private void OnBossSpawned(BossEnemy boss)
    {
        this.boss = boss;
    }

    private void OnDisable()
    {
        GameMode.Instance.OnBossSpawned -= OnBossSpawned;
        gameEndChannel.OnEvent -= OnGameEnd;
    }

    private void OnGameEnd(bool win)
    {
        sessionStats.Clear();
        List<IStatisticsProvider> statisticsProviders = UnityUtils.GetInterfacesOfType<IStatisticsProvider>(true);
        foreach (IStatisticsProvider provider in statisticsProviders)
        {
            Statistic[] statistics = provider.GetStats();
            foreach (Statistic statistic in statistics)
            {
                if (!sessionStats.Contains(statistic))
                {
                    sessionStats.Add(statistic);
                }
            }
        }
        sessionStats = sessionStats.OrderBy(s => s.Hash).ToList();
        statisticsChannel.Broadcast(sessionStats);
        //if (win)
        //{
        //    UpdateStatistics();
        //}
    }

    private void UpdateStatistics()
    {
        StatisticsData data;
        SaveObject obj = SaveManager.LoadPersistentData(StatisticsData.PATH);
        if (obj != null)
        {
            data = obj.GetData<StatisticsData>();
            foreach (Statistic statistic in sessionStats)
            {
                BossStatisticsUpdater(data, statistic);
            }
        }
        else
        {
            data = new StatisticsData();
            foreach (Statistic statistic in sessionStats)
            {
                if (!data.ContainsId(boss.GetId()))
                {
                    data.AddById(boss.GetId());
                }
                data.ById(boss.GetId(), out StatisticsData.Statistics statistics);
                statistics.Add(statistic.Hash, statistic.Value);
            }
        }
        SaveManager.Async.MarkDirty((data, StatisticsData.PATH));
    }

    private void BossStatisticsUpdater(StatisticsData data, Statistic current)
    {
        if (!data.ContainsId(boss.GetId()))
        {
            data.AddById(boss.GetId());
        }
        if (data.ById(boss.GetId(), out StatisticsData.Statistics statistics))
        {
            object currVal = statistics.ByHash(current.Hash);
            if (currVal == null)
            {
                statistics.Update(current.Hash, current.Value);
                return;
            }
            switch (current.Hash)
            {
                case Statistic.HEALTH_PERCENTAGE:
                    float perc = (float)currVal;
                    if (perc < current.ValueAs<float>())
                    {
                        statistics.Update(current.Hash, current.ValueAs<float>());
                    }

                    break;

                case Statistic.TIME:
                    float time = (float)currVal;
                    if (time > current.ValueAs<float>())
                    {
                        statistics.Update(current.Hash, current.ValueAs<float>());
                    }

                    break;
            }
        }
    }

    private void Start()
    {
        sessionStats = new List<Statistic>();
    }
}