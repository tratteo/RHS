// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : CharacterCombat.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.Performance;
using System;
using UnityEngine;
using static IHealthHolder;

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
    [Header("FX")]
    [SerializeField] private FxHandler bloodFxh;
    private Sword sword;
    private IElementOfInterest focusedTarget;
    private Collider2D[] elemetsOfInterestBuf;
    private UpdateJob detectElementsOfInterestJob;
    private ValueContainerSystem healthSystem;
    private float attackTimer = 0F;
    private float lookSign = 1F;
    private float stunTimer = 0F;
    private bool isDead = false;

    public bool IsStunned => stunTimer > 0F;

    public Ability<CharacterManager> EquippedAbility { get; private set; }

    public bool CanAttack => attackTimer <= 0F;

    public float ThresholdDistance => 50F;

    public event Action<bool> OnStun;

    public override void CommonFixedUpdate(float fixedDeltaTime)
    {
        if (focusedTarget == null)
        {
            transform.localScale = new Vector3(lookSign, 1F, 1F);
            sword.ClearTarget();
        }
        else if (focusedTarget is Component componentTarget)
        {
            if (focusedTarget.GetSightPoint().x > transform.position.x)
            {
                transform.localScale = new Vector3(1F, 1F, 1F);
            }
            else
            {
                transform.localScale = new Vector3(-1F, 1F, 1F);
            }
            sword.SetTarget(componentTarget.transform);
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

    public void Damage(Data data)
    {
        if (!Manager.Kinematic.IsInvulnerable)
        {
            Vector3 axis = (transform.position - data.Dealer.transform.position).normalized;
            axis = Quaternion.AngleAxis(UnityEngine.Random.Range(-40F, 40F), Vector3.forward) * axis;
            gameObject.AddForce2D(3F * data.Amount * axis);
            bloodFxh.Display(gameObject, transform.position, axis, Vector3.zero, FxHandler.Space.WORLD);
            healthSystem.Decrease(data.Amount);
            if (healthSystem.GetPercentage() <= 0 && !isDead)
            {
                Die();
                GameEndedBus.Broadcast(false);
            }
        }
    }

    public void Heal(Data data)
    {
        healthSystem.Increase(data.Amount);
    }

    public float GetHealthPercentage() => healthSystem.GetPercentage();

    public void Stun(float duration)
    {
        if (Manager.Kinematic.IsInvulnerable) return;
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
        elemetsOfInterestBuf = new Collider2D[128];
        detectElementsOfInterestJob = new UpdateJob(new Callback(DetectCloseElementsOfInterest), 0.125F);
        sword.SetOwner(this, baseDamage);
        healthSystem = new ValueContainerSystem(maxHealth);
        healthBar.Bind(healthSystem);
    }

    protected override void Start()
    {
        base.Start();
        if (Assets.Abilities.TryGetAbilityPrefabById(Character.GetEquippedAbility(), out GameObject ability))
        {
            EquippedAbility = Ability<CharacterManager>.AttachTo(ability, Manager);
        }
        else
        {
            EquippedAbility = Ability<CharacterManager>.AttachTo(defaultAbility, Manager);
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
        isDead = true;
        Rigidbody.velocity = Vector2.zero;
        Collider.enabled = false;
    }

    private void DetectCloseElementsOfInterest()
    {
        int res = Physics2D.OverlapCircleNonAlloc(transform.position, senseRadius, elemetsOfInterestBuf, ~LayerMask.GetMask(Layers.NOT_FOCUSABLE));

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