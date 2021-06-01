// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Sword.cs
//
// All Rights Reserved

using GibFrame.Performance;
using System;
using UnityEngine;

public partial class Sword : Weapon, ICommonFixedUpdate
{
    [Header("Sword")]
    [SerializeField] private Blocker blocker;
    [SerializeField] private ProjectileDeflector deflector;
    [SerializeField] private ParticleSystem defaultSlashEffect;
    [SerializeField] private ParticleSystem parryEffect;
    [SerializeField] private float colliderAngle = 100F;
    [SerializeField] private int colliderQuality = 5;

    private CircleCollider2D parryCollider;
    private PolygonCollider2D castCollider;
    private bool effectFlipped = false;

    private int slashTicks = 0;

    private ParticleSystemRenderer effectRenderer;

    private Vector2[] colliderPoints;

    public bool IsBlocking => blocker.Active;

    public Slash LastSlash { get; private set; }

    public bool IsSlashing { get; private set; } = false;

    public float ClashPower => GetDamage();

    public event Action<Clash> OnBlocked = delegate { };

    public event Action<IHealthHolder, float> OnDamaged = delegate { };

    public void ToggleBlock(bool state, float probability = -1F)
    {
        if (!blocker) return;
        if (state)
        {
            blocker.EnableBlock(probability);
        }
        else
        {
            blocker.DisableBlock();
        }
    }

    public override void SetOwner(IAgent owner, float damage)
    {
        base.SetOwner(owner, damage);
        if (deflector)
        {
            deflector.Setup(owner);
        }
    }

    public void TriggerSlash(Slash slash)
    {
        IsSlashing = true;
        LastSlash = slash;
        slashTicks = Mathf.RoundToInt(slash.Duration / Time.fixedDeltaTime);
        SetHitDamageMultiplier(slash.DamageMultiplier);
        parryCollider.radius = LastSlash.Range;
        parryCollider.enabled = true;
        slash.OnStart?.Invoke();
        if (deflector)
        {
            deflector.StartDeflecting(colliderPoints);
        }
    }

    public void CommonFixedUpdate(float fixedDeltaTime)
    {
        if (slashTicks > 0)
        {
            slashTicks--;
        }
        else if (IsSlashing && LastSlash != null)
        {
            EndSlash(false);
        }
    }

    public void HitRotate()
    {
        Flipped *= -1F;
        SetOffsetRotation(Flipped > 0 ? 0F : -(IdleRotation * 2F + 180F));
    }

    public void BlockedBy(Sword other)
    {
        EndSlash(true);
        Clash clash = new Clash(this, other);
        OnBlocked?.Invoke(clash);
        if (Owner is CharacterCombat)
        {
            Vibration.OneShot(Vibration.LIGHT_CLICK);
        }

        if (parryEffect)
        {
            parryEffect.transform.position = clash.Position;
            parryEffect.Emit((int)clash.ClashPower);
        }
    }

    protected override void Awake()
    {
        base.Awake();
        castCollider = GetComponent<PolygonCollider2D>();
        parryCollider = GetComponent<CircleCollider2D>();
        if (defaultSlashEffect)
        {
            effectRenderer = defaultSlashEffect.GetComponent<ParticleSystemRenderer>();
        }
        colliderPoints = new Vector2[colliderQuality + 1];
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.Equals(gameObject)) return;
        if (!LastSlash.IsParriable) return;
        Sword clasher;
        if (clasher = other.GetComponent<Sword>())
        {
            if (clasher.LastSlash.IsParriable)
            {
                BlockedBy(clasher);
            }
        }
    }

    private void EndSlash(bool blocked)
    {
        IsSlashing = false;
        parryCollider.enabled = false;
        if (LastSlash != null)
        {
            LastSlash.OnComplete?.Invoke();
        }
        if (!blocked)
        {
            ProcessHit();
        }
        HitRotate();
    }

    private void ProcessHit()
    {
        effectFlipped = !effectFlipped;

        if (CollidersCast())
        {
            if (defaultSlashEffect)
            {
                if (effectFlipped)
                {
                    effectRenderer.flip = new Vector3(1, 0, 0);
                }
                else
                {
                    effectRenderer.flip = new Vector3(0, 0, 0);
                }
                defaultSlashEffect.transform.localScale = new Vector3(1F, 0.75F, 1F) * LastSlash.Range;

                if (Target != null)
                {
                    defaultSlashEffect.transform.right = Target.GetSightPoint() - OwnerObj.transform.position;
                }
                else
                {
                    Vector3 newScale = Vector3.Scale(new Vector3(ScaleSign, 1F, 1F), defaultSlashEffect.transform.localScale);
                    defaultSlashEffect.transform.localScale = newScale;
                    defaultSlashEffect.transform.right = OwnerObj.transform.right;
                }
                defaultSlashEffect.transform.localPosition = LastSlash.Offset;
                defaultSlashEffect.Play();
            }
        }
    }

    /// <summary>
    ///   Setup the polygon collider points and perform the collider cast to process the hitted objects
    /// </summary>
    /// <returns> True if the hit is not blocked, false if the hit is blocked </returns>
    private bool CollidersCast()
    {
        castCollider.offset = LastSlash.Offset;
        if (colliderPoints.Length != colliderQuality + 1)
        {
            colliderPoints = new Vector2[colliderQuality + 1];
        }
        colliderPoints[0] = Vector3.zero;
        Vector3 axis;
        bool hasTarget = HasTarget();
        if (hasTarget)
        {
            axis = (Target.GetSightPoint() - OwnerObj.transform.position).normalized * LastSlash.Range;
        }
        else
        {
            axis = OwnerObj.transform.right * LastSlash.Range;
        }

        float stride = -colliderAngle / (colliderQuality - 1);
        axis = Quaternion.AngleAxis(colliderAngle / 2F, Vector3.forward) * axis;
        for (int i = 1; i < colliderQuality + 1; i++)
        {
            colliderPoints[i] = axis;
            if (hasTarget)
            {
                colliderPoints[i].x *= ScaleSign;
            }
            axis = Quaternion.AngleAxis(stride, Vector3.forward) * axis;
        }
        castCollider.SetPath(0, colliderPoints);

        RaycastHit2D[] res = new RaycastHit2D[8];
        castCollider.enabled = true;
        int amount = castCollider.Cast(Vector2.right, res, 0F);
        castCollider.enabled = false;

        for (int i = 0; i < amount; i++)
        {
            Collider2D hit = res[i].collider;
            if (!hit.gameObject.Equals(OwnerObj) && !hit.gameObject.Equals(gameObject))
            {
                IHealthHolder health;
                if ((health = hit.gameObject.GetComponent<IHealthHolder>()) != null)
                {
                    float damage = GetDamage();
                    health.Damage(damage);
                    OnDamaged?.Invoke(health, damage);
                }
            }
        }
        return true;
    }

    private void OnEnable()
    {
        CommonUpdateManager.Register(this);
    }

    private void OnDisable()
    {
        CommonUpdateManager.Unregister(this);
    }

    public class Clash
    {
        public Sword Other { get; private set; }

        public Sword Primary { get; private set; }

        public Vector2 Position { get; private set; }

        public float ClashPower { get; private set; }

        public Clash(Sword primary, Sword other)
        {
            Other = other;
            Primary = primary;
            ClashPower = primary.ClashPower + other.ClashPower;
            Position = primary.OwnerObj.transform.position + (other.OwnerObj.transform.position - primary.OwnerObj.transform.position) / 2F;
        }
    }
}