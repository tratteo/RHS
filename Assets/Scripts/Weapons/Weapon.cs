// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Weapon.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.Performance;
using UnityEngine;

public abstract class Weapon : MonoBehaviour, ICommonUpdate
{
    [Range(-90F, 90F)] [SerializeField] private float idleRotation;
    [SerializeField] private bool keepWeaponTransformUpVector = false;
    [SerializeField, Guarded] private Transform weaponTransform;
    private SpriteRenderer spriteRenderer;

    private float offsetRotation;
    private float targetRotation;
    private float returnToIdleTimer = 0F;

    public bool IsActive { get; private set; }

    public float GeneralDamageMultiplier { get; set; } = 1F;

    public float ScaleSign => Mathf.Sign(OwnerObj.transform.localScale.x);

    public Transform WeaponTransform => weaponTransform;

    public IAgent Owner { get; private set; }

    protected float IdleRotation => idleRotation;

    protected float Flipped { get; set; } = 1F;

    protected IElementOfInterest Target { get; private set; }

    protected GameObject OwnerObj { get; private set; }

    public virtual void SetOwner(IAgent owner)
    {
        Owner = owner;
        OwnerObj = (owner as MonoBehaviour).gameObject;
    }

    public virtual void SetActive(bool state)
    {
        if (IsActive == state) return;
        IsActive = state;
    }

    public bool HasTarget()
    {
        return Target != null && (Target as Component);
    }

    public virtual void CommonUpdate(float deltaTime)
    {
        if (HasTarget())
        {
            targetRotation = ScaleSign * Vector3.SignedAngle(Vector2.right, Target.GetSightPoint() - OwnerObj.transform.position, Vector3.forward);
            targetRotation += (ScaleSign * offsetRotation) + (ScaleSign * idleRotation);
            spriteRenderer.flipX = Flipped * ScaleSign < 0;
        }
        else
        {
            if (returnToIdleTimer > 0)
            {
                returnToIdleTimer -= deltaTime;
            }
            else
            {
                offsetRotation = 0F;
                Flipped = 1F;
                spriteRenderer.flipX = false;
            }
            targetRotation = offsetRotation + idleRotation;
        }
        if (keepWeaponTransformUpVector)
        {
            spriteRenderer.flipY = weaponTransform.transform.up.y < 0F;
        }
        WeaponTransform.localRotation = Quaternion.Euler(0F, 0F, targetRotation);
    }

    public void SetTarget(IElementOfInterest target)
    {
        Target = target;
    }

    public void ClearTarget()
    {
        Target = null;
    }

    protected void SetOffsetRotation(float offsetRotation)
    {
        this.offsetRotation = offsetRotation;
        returnToIdleTimer = 2F;
    }

    protected virtual void Awake()
    {
        spriteRenderer = weaponTransform.GetComponent<SpriteRenderer>();
    }

    protected virtual void Start()
    {
        CommonUpdateManager.Register(this);
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