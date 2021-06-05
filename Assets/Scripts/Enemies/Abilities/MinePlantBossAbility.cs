using GibFrame;
using GibFrame.ObjectPooling;
using System.Collections;
using UnityEngine;

public class MinePlantBossAbility : Ability<BossEnemy>
{
    [SerializeField, Guarded] private GameObject minePrefab;

    public override void CommonUpdate(float deltaTime)
    {
        base.CommonUpdate(deltaTime);
        // Debug.Log(IsPerforming);
    }

    protected override IEnumerator Execute_C()
    {
        GameObject obj = PoolManager.Instance.Spawn(Categories.PROJECTILES, minePrefab.name, Parent.transform.position, Quaternion.identity);
        Mine mine = obj.GetComponent<Mine>();
        if (mine)
        {
            mine.SetupLayer(Parent);
        }
        Complete();
        yield break;
    }

    private void Awake()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        PoolCategory category = PoolManager.Instance.GetCategory(Categories.PROJECTILES);
        if (category == null)
        {
            category = new PoolCategory(Categories.PROJECTILES);
        }
        Pool pool = category.GetPool(minePrefab.name);
        if (pool == null)
        {
            pool = new Pool(minePrefab.name, minePrefab, 50);
            category.AddPool(pool);
            PoolManager.Instance.AddCategory(category);
        }
    }
}