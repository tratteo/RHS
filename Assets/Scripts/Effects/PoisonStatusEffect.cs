using GibFrame;
using GibFrame.Performance;
using UnityEngine;

public class PoisonStatusEffect : StatusEffect
{
    [Header("Poison")]
    [SerializeField] private float damage = 1F;
    [SerializeField] private float interval = 0.5F;
    private UpdateJob damageJob;
    private IHealthHolder healthHolder;

    public override void CommonUpdate(float deltaTime)
    {
        base.CommonUpdate(deltaTime);
        damageJob.CommonUpdate(deltaTime);
    }

    protected void Start()
    {
        if (!GetStatusEffectFunctionalInterface(out healthHolder))
        {
            Destroy();
        }
        damageJob = new UpdateJob(new Callback(() => healthHolder.Damage(new IHealthHolder.Data(gameObject, damage))), interval);
    }
}