using UnityEngine;

public class StunStatusEffect : StatusEffect
{
    private IStunnable stunnable;

    protected override void OnSpawn()
    {
        Debug.Log("Spawn stun");
        if (!GetStatusEffectFunctionalInterface(out stunnable))
        {
            Destroy();
        }
        else
        {
            Debug.Log("Stun: " + stunnable);
            stunnable.Stun(Duration);
        }
    }
}