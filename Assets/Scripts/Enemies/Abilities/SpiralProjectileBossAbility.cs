using GibFrame;
using System.Collections;
using UnityEngine;

public class SpiralProjectileBossAbility : Ability<BossEnemy>
{
    [SerializeField, Guarded] private GameObject projectilePrefab;
    [SerializeField] private int amount = 12;
    [SerializeField] private float turns = 1F;
    [SerializeField] private float delay = 1F;

    protected override IEnumerator Execute_C()
    {
        Vector3 axis = Parent.transform.right.Perturbate(5F).normalized;
        WaitForSeconds wait = new WaitForSeconds(delay);
        float stride = 360F * turns / amount;
        for (int i = 0; i < amount; i++)
        {
            Projectile projectile = Projectile.Create(projectilePrefab.name, Parent.transform.position, Quaternion.LookRotation(Vector3.forward, axis), Parent, Parent.BattleContext.Transform);
            //projectile.transform.right = axis;
            axis = Quaternion.AngleAxis(stride, Vector3.forward) * axis;
            yield return wait;
        }
        Complete();
    }

    private void Awake()
    {
        PoolDispatcher.Instance.RequestPool(Categories.PROJECTILES, projectilePrefab, amount * 2);
    }
}