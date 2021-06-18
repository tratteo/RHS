// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : CharacterCombat.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.Performance;
using System;
using UnityEngine;

public class CharacterCombat : CharacterComponent, IAgent, IHealthHolder, IStunnable, IElementOfInterest, IWeaponOwner, IStatisticsProvider
{
    [Header("Channels")]
    [SerializeField, Guarded] private CameraShakeEventBus cameraShakeChannel;
    [Header("Parameters")]
    [SerializeField] private float maxHealth = 250F;
    [SerializeField] private float baseDamage = 10F;
    [SerializeField] private float senseRadius = 12F;
    [SerializeField] private float attackCooldown = 0.3F;
    [SerializeField] private Sword.Attack.Builder baseAttack;
    [SerializeField, Guarded] private GameObject defaultAbility;
    [Header("UI")]
    [SerializeField, Guarded] private ValueContainerBar healthBar;
    private Sword sword;
    private IElementOfInterest focusedTarget;
    private Collider2D[] elemetsOfInterestBuf;
    private UpdateJob detectElementsOfInterestJob;
    private ValueContainerSystem healthSystem;
    private float attackTimer = 0F;
    private float lookSign = 1F;
    private float stunTimer = 0F;

    public bool IsStunned => stunTimer > 0F;

    public Ability<CharacterManager> EquippedAbility { get; private set; }

    public bool CanAttack => attackTimer <= 0F;

    public float ThresholdDistance => 20F;

    public event Action<bool> OnStun;

    public override void CommonFixedUpdate(float fixedDeltaTime)
    {
        if (focusedTarget == null)
        {
            transform.localScale = new Vector3(lookSign, 1F, 1F);
            sword.ClearTarget();
        }
        else if (focusedTarget as Component)
        {
            if (focusedTarget.GetSightPoint().x > transform.position.x)
            {
                transform.localScale = new Vector3(1F, 1F, 1F);
            }
            else
            {
                transform.localScale = new Vector3(-1F, 1F, 1F);
            }
            sword.SetTarget(focusedTarget);
        }
    }

    public override void CommonUpdate(float deltaTime)
    {
        detectElementsOfInterestJob.Step(deltaTime);
        if (attackTimer > 0)
        {
            attackTimer -= deltaTime;
            if (attackTimer <= 0)
            {
                attackTimer = 0F;
            }
        }
        if (stunTimer > 0F)
        {
            stunTimer -= deltaTime;
            if (stunTimer <= 0)
            {
                stunTimer = 0F;
                OnStun?.Invoke(false);
            }
        }
    }

    public IAgent.FactionRelation GetFactionRelation() => IAgent.FactionRelation.FRIENDLY;

    public Weapon GetWeapon() => sword;

    public void Damage(float amount)
    {
        if (!Manager.Kinematic.IsInvulnerable)
        {
            healthSystem.Decrease(amount);
            if (healthSystem.GetPercentage() <= 0)
            {
                Die();
                GameEndedBus.Broadcast(false);
            }
        }
    }

    public void Heal(float amount)
    {
        healthSystem.Increase(amount);
    }

    public float GetHealthPercentage() => healthSystem.GetPercentage();

    public void Stun(float duration)
    {
        stunTimer = duration;
        OnStun?.Invoke(true);
    }

    public Vector3 GetSightPoint() => transform.position;

    public IElementOfInterest.InterestPriority GetInterestPriority() => IElementOfInterest.InterestPriority.MANDATORY;

    public Statistic[] GetStats()
    {
        return new Statistic[] { new Statistic(Statistic.HEALTH_PERCENTAGE, healthSystem.GetPercentage()) };
    }

    protected override void Awake()
    {
        base.Awake();
        sword = GetComponentInChildren<Sword>();
        elemetsOfInterestBuf = new Collider2D[8];
        detectElementsOfInterestJob = new UpdateJob(new Callback(DetectCloseElementsOfInterest), 0.25F);
        sword.SetOwner(this, baseDamage);
        healthSystem = new ValueContainerSystem(maxHealth);
        healthBar.Bind(healthSystem);
    }

    protected override void Start()
    {
        base.Start();
        if (Assets.Abilities.TryGetAbilityPrefabById(Character.GetEquippedAbility(), out GameObject ability))
        {
            EquippedAbility = Ability<CharacterManager>.Attach(ability, Manager);
        }
        else
        {
            EquippedAbility = Ability<CharacterManager>.Attach(defaultAbility, Manager);
        }
        Manager.GUI.BindCooldown(EquippedAbility);
    }

    protected override void OnInput(Inputs.InputData data)
    {
        if (IsStunned) return;
        switch (data.Type)
        {
            case Inputs.InputType.BASE_ATTACK:
                if (CanAttack)
                {
                    sword.TriggerAttack(baseAttack.OnStart(() => cameraShakeChannel.Broadcast(new CameraShakeEventBus.Shake(CameraShakeEventBus.HIT, transform.position))));
                    attackTimer = attackCooldown;
                }
                break;

            case Inputs.InputType.PRIMARY_ABILITY:
                EquippedAbility.Perform();
                break;

            case Inputs.InputType.MOVE:
                Inputs.DirectionInputData dirData = data as Inputs.DirectionInputData;
                if (!Mathf.Approximately(dirData.Direction.magnitude, 0F))
                {
                    lookSign = dirData.Direction.x > 0 ? 1F : -1F;
                }
                break;
        }
    }

    private void Die()
    {
        Rigidbody.velocity = Vector2.zero;
        Collider.enabled = false;
    }

    private void DetectCloseElementsOfInterest()
    {
        int res = Physics2D.OverlapCircleNonAlloc(transform.position, senseRadius, elemetsOfInterestBuf, ~0);

        IElementOfInterest selected = null;
        float minDistance = float.MaxValue;
        int pass = 0;
        elemetsOfInterestBuf.ForEach(res, (c) =>
        {
            if (c && !c.gameObject.Equals(gameObject))
            {
                IElementOfInterest element = c.gameObject.GetComponent<IElementOfInterest>();
                if (element != null)
                {
                    float distance = Vector2.Distance(transform.position, element.GetSightPoint());
                    if (distance <= element.ThresholdDistance)
                    {
                        pass++;
                        if (selected == null)
                        {
                            selected = element;
                            minDistance = distance;
                        }
                        else if ((int)element.GetInterestPriority() > (int)selected.GetInterestPriority())
                        {
                            selected = element;
                            minDistance = distance;
                        }
                        else if (element.GetInterestPriority() == selected.GetInterestPriority())
                        {
                            if (distance < minDistance)
                            {
                                selected = element;
                                minDistance = distance;
                            }
                        }
                    }
                }
            }
        });
        if (pass <= 0)
        {
            focusedTarget = null;
        }
        else
        {
            focusedTarget = selected;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, senseRadius);
    }
}