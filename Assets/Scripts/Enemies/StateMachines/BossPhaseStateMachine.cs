// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : BossPhaseStateMachine.cs
//
// All Rights Reserved

using GibFrame;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossPhaseStateMachine : BossStateMachine
{
    [Header("Kinematic")]
    [Range(0F, 10F)] [SerializeField] private float movementSpeedMultiplier = 1F;
    [Range(0F, 10F)] [SerializeField] private float accelerationMultiplier = 1F;
    [SerializeField] private RandomizedFloat movementUpdate;
    [Header("Combat")]
    [Range(0F, 10F)] [SerializeField] private float damageMultiplier = 1F;
    [SerializeField] private RandomizedFloat abilityUpdateTime;
    [SerializeField] private List<GameObject> abilitiesPrefabs;

    private List<Ability<BossEnemy>> abilities;

    private bool isPerformingAbility = false;
    private float movementTimer = 0F;
    private float abilityTimer = 0F;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Owner.GetWeapon().BaseDamage = Owner.GetWeapon().BaseDamage * damageMultiplier;
        Owner.MovementSpeedMultiplier(movementSpeedMultiplier);
        Owner.AccelerationMultiplier = accelerationMultiplier;
        movementTimer = movementUpdate;
        isPerformingAbility = false;
        abilityTimer = 0F;

        abilities = new List<Ability<BossEnemy>>();
        abilitiesPrefabs.ForEach(p => abilities.Add(Ability<BossEnemy>.Attach(p, Owner)));
    }

    public bool TryGetAbility(out Ability<BossEnemy> ability)
    {
        List<Ability<BossEnemy>> match = abilities.FindAll(a => a.CanPerform());
        if (match.Count <= 0)
        {
            ability = null;
            return false;
        }
        else
        {
            ability = match[UnityEngine.Random.Range(0, match.Count)];
            return true;
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!isPerformingAbility && CanExecute())
        {
            if (abilityTimer > 0F)
            {
                abilityTimer -= Time.deltaTime;
            }
            else
            {
                if (TryGetAbility(out Ability<BossEnemy> ability))
                {
                    if (ability.Perform())
                    {
                        Owner.EnableSelfMovement = false;
                        isPerformingAbility = true;
                        ability.OnComplete += () =>
                        {
                            isPerformingAbility = false;
                            Owner.EnableSelfMovement = true;
                            movementTimer = movementUpdate;
                            abilityTimer = abilityUpdateTime;
                        };
                    }
                }
                abilityTimer = abilityUpdateTime;
            }

            if (!isPerformingAbility)
            {
                if (movementTimer <= 0F)
                {
                    Owner.Move(GetMovementDirection());
                    movementTimer = movementUpdate;
                }
                else
                {
                    movementTimer -= Time.deltaTime;
                }
            }
        }
    }

    public bool CanExecute()
    {
        return Owner.CurrentStatus == Enemy.Status.ATTACKING && Owner.TargetContext != null;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
        abilities.ForEach(a => a.HardStop());
    }

    protected abstract float GetAttackRange();

    protected virtual Vector3 GetMovementDirection()
    {
        if (Vector2.Distance(Owner.TargetContext.Transform.position, Owner.transform.position) > GetAttackRange())
        {
            if (Random.Range(0F, 1F) < 0.2F)
            {
                return Vector2.zero;
            }
            else
            {
                Vector2 dir = (Owner.TargetContext.Transform.position - Owner.transform.position).normalized;

                dir = Quaternion.AngleAxis(UnityEngine.Random.Range(-50F, 50F), Vector3.forward) * dir;
                Debug.DrawRay(Owner.transform.position, dir, Color.red, 0.5F);
                return dir;
            }
        }
        else
        {
            Vector2 dir = (Owner.transform.position - Owner.TargetContext.Transform.position).normalized;
            if (UnityEngine.Random.value < 0.5F)
            {
                dir = Quaternion.AngleAxis(90F, Vector3.forward) * dir;
            }
            else
            {
                dir = Quaternion.AngleAxis(-90F, Vector3.forward) * dir;
            }
            Debug.DrawRay(Owner.transform.position, dir, Color.red, 0.5F);
            return dir;
        }
    }
}