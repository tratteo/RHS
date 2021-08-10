using GibFrame;
using System.Collections;
using UnityEngine;

public class SuccessionShotsBossAbility : Ability<BossEnemy>
{
    [SerializeField] private int index = 0;
    [SerializeField] private RandomizedInt rounds;
    [SerializeField] private RandomizedFloat shootDelay;

    protected override IEnumerator Execute_C()
    {
        Weapon weapon = Parent.Weapon;
        if (weapon is Shooter shooter)
        {
            int current = shooter.ProjectileIndex;
            shooter.SetPrefabIndex(index);
            for (int i = 0; i < rounds; i++)
            {
                shooter.TriggerShoot();
                yield return new WaitForSeconds(shootDelay);
            }
            shooter.SetPrefabIndex(current);
        }
        Complete();
    }
}