// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : DamageOverTimeArea.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.ObjectPooling;
using GibFrame.Performance;
using System.Collections.Generic;
using UnityEngine;

public class DamageOverTimeArea : MonoBehaviour, ICommonUpdate, IPooledObject
{
    private List<IHealthHolder> healthHolders;
    [SerializeField] private IAgent.FactionRelation faction;
    [SerializeField] private float damageOverTime = 5F;
    [SerializeField] private float damageInterval = 0.5F;
    [SerializeField] private float duration = 10F;
    private UpdateJob damageJob;
    private float timer = 0F;

    public void CommonUpdate(float deltaTime)
    {
        damageJob.Step(deltaTime);
        if (timer > 0)
        {
            timer -= deltaTime;
            if (timer < 0F)
            {
                timer = 0F;
                gameObject.SetActive(false);
            }
        }
    }

    public void OnObjectSpawn()
    {
        damageJob ??= new UpdateJob(new Callback(ProcessHealthHolders), damageInterval);
        damageJob.Resume();
        timer = duration;
    }

    private void Awake()
    {
        healthHolders = new List<IHealthHolder>();
    }

    private void ProcessHealthHolders()
    {
        List<IHealthHolder> temp = new List<IHealthHolder>(healthHolders);
        temp.ForEach(h =>
        {
            if (h != null)
            {
                h.Damage(new IHealthHolder.Data(gameObject, damageOverTime));
            }
        });
        temp.RemoveAll(h => h == null);
    }

    private void OnEnable()
    {
        CommonUpdateManager.Register(this);
    }

    private void OnDisable()
    {
        damageJob?.Suspend();
        CommonUpdateManager.Unregister(this);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IHealthHolder healthHolder = collision.gameObject.GetComponent<IHealthHolder>();
        if (healthHolder == null) return;
        IAgent agent = collision.gameObject.GetComponent<IAgent>();
        if (agent == null || (agent != null && agent.GetFactionRelation() != faction))
        {
            healthHolders.Add(healthHolder);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        IHealthHolder healthHolder = collision.gameObject.GetComponent<IHealthHolder>();
        if (healthHolder == null) return;
        healthHolders.Remove(healthHolder);
    }
}