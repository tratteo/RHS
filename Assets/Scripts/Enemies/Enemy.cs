// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Enemy.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.Performance;
using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class Enemy : MonoBehaviour, ICommonUpdate, ICommonFixedUpdate, IElementOfInterest, IAgent, IHealthHolder, IDescribable
{
    public enum Status { ATTACKING, IDLING }

    private readonly Collider2D[] senseBuffer = new Collider2D[8];
    [SerializeField] private SerializedDescribable describable;
    [Header("Parameters")]
    [SerializeField] private float maxHealth;
    [SerializeField] private float senseRadius = 5F;
    [Header("Kinematic")]
    [SerializeField] private float idleSpeed = 2F;
    [SerializeField] private float runSpeed = 5F;
    [SerializeField] private float obstacleDistanceThreshold = 1F;
    [SerializeField] private Vector2 dashDuration = new Vector2(0.1F, 0.35F);
    [Header("HUD")]
    [SerializeField, Guarded] private ValueContainerBar healthBar;
    [SerializeField, Guarded] private Image interactionImage;
    [Header("Debug")]
    [SerializeField] private bool debugRender = true;
    private float currentSpeed;
    private float movementSpeed;
    private float currentMovementSpeedMultiplier = 1F;
    private UpdateJob senseJob;
    private IHealthHolder targetHealth;

    public Collider2D Collider { get; private set; }

    public AttackContext TargetContext { get; private set; }

    public bool EnableSelfMovement { get; set; } = true;

    public bool IsDashing { get; private set; }

    public float AccelerationMultiplier { get; set; } = 1F;

    public Status CurrentStatus { get; private set; }

    public virtual float ThresholdDistance => 20F;

    public Rigidbody2D Rigidbody { get; private set; }

    public ValueContainerSystem HealthSystem { get; private set; }

    protected Vector2 TargetVelocity { get; private set; }

    public event Action OnDeath;

    public void MovementSpeedMultiplier(float multiplier)
    {
        currentMovementSpeedMultiplier = multiplier;
        movementSpeed = currentSpeed * multiplier;
    }

    public virtual float Dash(Vector2 direction, float force)
    {
        IsDashing = true;
        float duration = ((dashDuration.y - dashDuration.x) * 0.01F) * direction.magnitude * force + dashDuration.x;
        duration = Mathf.Clamp(duration, dashDuration.x, dashDuration.y);
        Rigidbody.AddForce(direction * force * Rigidbody.mass, ForceMode2D.Impulse);
        new Timer(this, duration, false, true, new Callback(() => IsDashing = false));
        return duration;
    }

    public virtual void SetInteraction(Sprite icon, Color color)
    {
        interactionImage.sprite = icon;
        interactionImage.color = color;
    }

    public virtual void SetInteraction(Sprite icon = null)
    {
        interactionImage.sprite = icon != null ? icon : Assets.Sprites.Transparent;
        interactionImage.color = Color.white;
    }

    public virtual void CommonFixedUpdate(float fixedDeltaTime)
    {
        if (EnableSelfMovement)
        {
            DetectBounds();
        }
        if (!IsDashing)
        {
            Rigidbody.velocity = Vector2.Lerp(Rigidbody.velocity, TargetVelocity, 0.15F * AccelerationMultiplier);
        }
    }

    public virtual void CommonUpdate(float deltaTime)
    {
        senseJob.Step(deltaTime);
        if (CurrentStatus == Status.ATTACKING)
        {
            if (TargetContext == null || targetHealth == null || targetHealth.GetHealthPercentage() <= 0F)
            {
                SetStatus(Status.IDLING);
            }
            else
            {
                if (TargetContext.Transform.position.x > transform.position.x)
                {
                    transform.localScale = new Vector3(1F, 1F, 1F);
                }
                else
                {
                    transform.localScale = new Vector3(-1F, 1F, 1F);
                }
            }
        }
    }

    public virtual void Damage(float amount)
    {
        HealthSystem.Decrease(amount);
    }

    public virtual void Heal(float amount)
    {
        HealthSystem.Increase(amount);
    }

    public IAgent.FactionRelation GetFactionRelation() => IAgent.FactionRelation.HOSTILE;

    public float GetHealthPercentage() => HealthSystem.GetPercentage();

    public virtual IElementOfInterest.InterestPriority GetInterestPriority() => IElementOfInterest.InterestPriority.AVERAGE;

    public Vector3 GetSightPoint() => transform.position;

    public abstract Weapon GetWeapon();

    public virtual void Move(Vector2 direction)
    {
        TargetVelocity = direction.normalized * movementSpeed;
    }

    public string GetId() => describable.GetId();

    public string GetName() => describable.GetName();

    public Sprite GetIcon() => describable.GetIcon();

    public string GetDescription() => describable.GetDescription();

    protected virtual void OnEnable()
    {
        CommonUpdateManager.Register(this);
        HealthSystem.OnExhaust += Die;
        HealthSystem.OnExhaust += () => OnDeath?.Invoke();
        SetStatus(Status.IDLING);
    }

    protected virtual void OnDisable()
    {
        CommonUpdateManager.Unregister(this);
        HealthSystem.OnExhaust -= Die;
        HealthSystem.OnExhaust -= () => OnDeath?.Invoke();
    }

    protected virtual void Awake()
    {
        senseJob = new UpdateJob(new Callback(Sense), 0.25F);
        HealthSystem = new ValueContainerSystem(maxHealth);
        healthBar.Bind(HealthSystem);
        Rigidbody = GetComponent<Rigidbody2D>();
        Collider = GetComponent<Collider2D>();
    }

    protected virtual void SetStatus(Status status)
    {
        switch (status)
        {
            case Status.IDLING:
                currentSpeed = idleSpeed;
                movementSpeed = idleSpeed * currentMovementSpeedMultiplier;
                Move(Vector2.zero);
                TargetContext = null;
                targetHealth = null;
                GetWeapon().ClearTarget();
                break;

            case Status.ATTACKING:
                currentSpeed = runSpeed;
                movementSpeed = runSpeed * currentMovementSpeedMultiplier;
                break;
        }
        CurrentStatus = status;
    }

    protected virtual void EngageBattle(Transform target)
    {
        targetHealth = target.GetComponent<IHealthHolder>();
        if (targetHealth != null)
        {
            SetStatus(Status.ATTACKING);
            Move(Vector2.zero);
            Rigidbody.velocity = Vector2.zero;
            GetWeapon().SetTarget(target.GetComponent<IElementOfInterest>());
            TargetContext = new AttackContext(target, targetHealth);
        }
    }

    protected virtual void Die()
    {
        gameObject.SetActive(false);
    }

    protected virtual void DetectBounds()
    {
        RaycastHit2D hit;
        if ((hit = Physics2D.BoxCast(transform.position, new Vector2(1F, 1F), 0F, Vector2.right, obstacleDistanceThreshold, ~LayerMask.GetMask(Layers.HOSTILES, Layers.WEAPONS))).collider != null)
        {
            if (!hit.collider.isTrigger)
            {
                Move((new Vector2(transform.position.x, transform.position.y) - hit.point).Perturbate(0.5F).normalized);
            }
        }
        if ((hit = Physics2D.BoxCast(transform.position, new Vector2(1F, 1F), 0F, Vector2.left, obstacleDistanceThreshold, ~LayerMask.GetMask(Layers.HOSTILES, Layers.WEAPONS))).collider != null)
        {
            if (!hit.collider.isTrigger)
            {
                Move((new Vector2(transform.position.x, transform.position.y) - hit.point).Perturbate(0.5F).normalized);
            }
        }
        if ((hit = Physics2D.BoxCast(transform.position, new Vector2(1F, 1F), 0F, Vector2.up, obstacleDistanceThreshold, ~LayerMask.GetMask(Layers.HOSTILES, Layers.WEAPONS))).collider != null)
        {
            if (!hit.collider.isTrigger)
            {
                Move((new Vector2(transform.position.x, transform.position.y) - hit.point).Perturbate(0.5F).normalized);
            }
        }
        if ((hit = Physics2D.BoxCast(transform.position, new Vector2(1F, 1F), 0F, Vector2.down, obstacleDistanceThreshold, ~LayerMask.GetMask(Layers.HOSTILES, Layers.WEAPONS))).collider != null)
        {
            if (!hit.collider.isTrigger)
            {
                Move((new Vector2(transform.position.x, transform.position.y) - hit.point).Perturbate(0.5F).normalized);
            }
        }
    }

    protected virtual void OnSense(Collider2D[] buffer, int number)
    {
        int validAmount = 0;
        for (int i = 0; i < number; i++)
        {
            if (!buffer[i].gameObject.Equals(gameObject))
            {
                validAmount++;
                if (CurrentStatus == Status.IDLING)
                {
                    IAgent agent;
                    if ((agent = buffer[i].gameObject.GetComponent<IAgent>()) != null)
                    {
                        if (agent.GetFactionRelation() != GetFactionRelation())
                        {
                            IHealthHolder health = buffer[i].gameObject.GetComponent<IHealthHolder>();
                            if (health != null)
                            {
                                EngageBattle(buffer[i].transform);
                            }
                        }
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (debugRender)
        {
            Gizmos.DrawWireSphere(transform.position, senseRadius);
        }
    }

    private void Sense()
    {
        int amount = Physics2D.OverlapCircleNonAlloc(transform.position, senseRadius, senseBuffer, ~LayerMask.GetMask(Layers.ENVIROMENT));
        OnSense(senseBuffer, amount);
    }

    public class AttackContext
    {
        public Transform Transform { get; private set; }

        public IHealthHolder TargetHealth { get; private set; }

        public AttackContext(Transform targetTransform, IHealthHolder targetHealth)
        {
            Transform = targetTransform;
            TargetHealth = targetHealth;
        }
    }
}