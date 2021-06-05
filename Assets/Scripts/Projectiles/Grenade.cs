using GibFrame;
using GibFrame.Performance;
using UnityEngine;

public class Grenade : Projectile, ICommonUpdate, IDeflectable
{
    private readonly Collider2D[] detonationBuf = new Collider2D[8];
    [Header("Channels")]
    [SerializeField, Guarded] private CameraShakeChannelEvent cameraShakeChannel;
    [Header("Parameters")]
    [SerializeField] private float detonationTime = 2F;
    [SerializeField] private float detonationRadius = 4F;
    private float detonationTimer = 0F;

    public bool CanBeDeflected { get; private set; } = true;

    public void CommonUpdate(float deltaTime)
    {
        Debug.Log("Tick");
        if (detonationTimer > 0)
        {
            detonationTimer -= Time.deltaTime;
        }
        else
        {
            Debug.Log("Detonate");
            Detonate();
        }
    }

    public void Launch(float force)
    {
        Rigidbody.AddForce((Rigidbody.drag + 1F) * force * transform.right, UnityEngine.ForceMode2D.Impulse);
        detonationTimer = detonationTime;
    }

    public void Deflect(IAgent agent)
    {
        if (agent.GetFactionRelation() == IAgent.FactionRelation.HOSTILE)
        {
            gameObject.layer = LayerMask.NameToLayer(Layers.ENEMY_PROJECTILES);
        }
        else if (agent.GetFactionRelation() == IAgent.FactionRelation.FRIENDLY)
        {
            gameObject.layer = LayerMask.NameToLayer(Layers.PROJECTILES);
        }

        Rigidbody.velocity = -Rigidbody.velocity;
        CanBeDeflected = false;
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collideTagExceptions.Contains(collision.collider.tag))
        {
            //Debug.Log("Detonate");
            Detonate();
        }
    }

    private void Detonate()
    {
        cameraShakeChannel.Broadcast(new CameraShakeChannelEvent.Shake(0.15F, 4F));
        int amount = Physics2D.OverlapCircleNonAlloc(transform.position, detonationRadius, detonationBuf);
        for (int i = 0; i < amount; i++)
        {
            IHealthHolder healthHolder;
            if ((healthHolder = detonationBuf[i].gameObject.GetComponent<IHealthHolder>()) != null)
            {
                healthHolder.Damage(GetDamage());
            }
            IEffectsReceiverHolder effectReceiver;
            if ((effectReceiver = detonationBuf[i].gameObject.GetComponent<IEffectsReceiverHolder>()) != null)
            {
                effectReceiver.GetEffectsReceiver().AddEffects(onHitEffects.ToArray());
            }
        }
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        CommonUpdateManager.Register(this);
    }

    private void OnDisable()
    {
        CommonUpdateManager.Unregister(this);
    }
}