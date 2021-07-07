// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : SpawnMinionBossAbility.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.ObjectPooling;
using System.Collections;
using UnityEngine;

public class SpawnMinionBossAbility : Ability<BossEnemy>
{
    [SerializeField, Guarded] private GameObject prefab;
    [SerializeField] private RandomizedInt batchAmount;
    [SerializeField] private RandomizedFloat spawnTimer;
    [SerializeField] private int maxAmount = 5;
    private float currentTimer = 0F;
    private int currentAmount = 0;

    public override bool CanPerform()
    {
        return base.CanPerform() && currentTimer <= 0F;
    }

    public override void CommonUpdate(float deltaTime)
    {
        base.CommonUpdate(deltaTime);
        if (currentTimer > 0)
        {
            currentTimer -= deltaTime;
            if (currentTimer < 0)
            {
                currentTimer = 0F;
            }
        }
    }

    protected override IEnumerator Execute_C()
    {
        int amount = this.batchAmount;
        for (int i = 0; i < amount; i++)
        {
            if (currentAmount < maxAmount)
            {
                GameObject obj = PoolManager.Instance.Spawn(Categories.ENEMY_MINIONS, prefab.name, Parent.transform.position + new Vector3(UnityEngine.Random.Range(-1F, 1F), UnityEngine.Random.Range(-1F, 1F), 0F).normalized * UnityEngine.Random.Range(8F, 20F), Parent.transform.rotation);
                EnemyMinion minion = obj.GetComponent<EnemyMinion>();
                if (minion)
                {
                    minion.OnDeath += () => currentAmount--;
                    minion.SetOwner(Parent);
                    currentAmount++;
                }
                else
                {
                    Destroy(obj);
                }
            }
        }

        yield return null;
        currentTimer = spawnTimer;
        Complete();
    }

    private void Awake()
    {
        PoolDispatcher.Instance.RequestPool(Categories.ENEMY_MINIONS, prefab, 50);
    }

    private void Start()
    {
        currentTimer = spawnTimer;
    }
}