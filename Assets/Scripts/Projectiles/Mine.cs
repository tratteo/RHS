using GibFrame;
using UnityEngine;

public class Mine : Projectile
{
    private readonly Collider2D[] buf = new Collider2D[8];
    [Header("Channels")]
    [SerializeField, Guarded] private CameraShakeChannelEvent cameraShakeChannel;
    [SerializeField] private float explosionRadius = 8F;
    [SerializeField] private float explosionForce = 2500F;

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collideTagExceptions.Contains(collision.collider.tag))
        {
            int amount = Physics2D.OverlapCircleNonAlloc(transform.position, explosionRadius, buf, GetActionLayer());
            for (int i = 0; i < amount; i++)
            {
                IHealthHolder healthHolder;
                if ((healthHolder = buf[i].gameObject.GetComponent<IHealthHolder>()) != null)
                {
                    healthHolder.Damage(GetDamage());
                }
                IEffectsReceiverHolder effectReceiver;
                if ((effectReceiver = buf[i].gameObject.GetComponent<IEffectsReceiverHolder>()) != null)
                {
                    effectReceiver.GetEffectsReceiver().AddEffects(onHitEffects.ToArray());
                }

                buf[i].gameObject.AddForce2D((buf[i].transform.position - transform.position).normalized * explosionForce);
            }

            cameraShakeChannel.Broadcast(new CameraShakeChannelEvent.Shake(0.2F, 8F));
            gameObject.SetActive(false);
        }
    }
}