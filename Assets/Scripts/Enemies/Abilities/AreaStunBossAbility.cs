using GibFrame;
using System.Collections;
using UnityEngine;

public class AreaStunBossAbility : Ability<BossEnemy>
{
    private readonly Collider2D[] buf = new Collider2D[8];
    [SerializeField] private float baseDamage = 350F;
    [SerializeField] private float channelTime = 1.25F;
    [SerializeField] private float explosionRadius = 8F;
    [SerializeField] private float stunDuration = 2F;

    private IHealthHolder healthHolder;

    public override bool CanPerform()
    {
        return base.CanPerform() && Vector2.Distance(Parent.TargetContext.Transform.position, Parent.transform.position) < explosionRadius * 0.8F;
    }

    protected override IEnumerator Execute_C()
    {
        Parent.Move(Vector2.zero);
        Parent.SetInteraction(Assets.Sprites.Exclamation, Color.red);
        yield return new WaitForSeconds(channelTime);
        Parent.SetInteraction();
        int amount = Physics2D.OverlapCircleNonAlloc(transform.position, explosionRadius, buf, ~LayerMask.GetMask(Layers.HOSTILES));
        GDebug.DrawWireSphere(transform.position, explosionRadius, Color.green, 1F, 3);
        for (int i = 0; i < amount; i++)
        {
            IStunnable stunnable;
            if ((stunnable = buf[i].gameObject.GetComponent<IStunnable>()) != null)
            {
                stunnable.Stun(stunDuration);
            }

            if ((healthHolder = buf[i].gameObject.GetComponent<IHealthHolder>()) != null)
            {
                healthHolder.Damage(baseDamage);
            }
        }
        Complete();
    }
}