using UnityEngine;

public class SpeedStatusEffect : StatusEffect
{
    [SerializeField, Range(0F, 10F)] private float multiplier = 1F;
    private ISpeedOwner speedOwner;

    protected void Start()
    {
        if (!GetStatusEffectFunctionalInterface(out speedOwner))
        {
            Destroy();
        }
        else
        {
            speedOwner.SpeedMultiplier = multiplier;
        }
    }

    protected override void Destroy()
    {
        if (speedOwner != null)
        {
            speedOwner.SpeedMultiplier = 1F;
        }
        base.Destroy();
    }
}