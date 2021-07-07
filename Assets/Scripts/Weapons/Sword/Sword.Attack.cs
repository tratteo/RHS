// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Sword.Attack.cs
//
// All Rights Reserved

using System;
using UnityEngine;

public partial class Sword
{
    [System.Serializable]
    public class Attack
    {
        [SerializeField] private float duration = 0.02F;
        [SerializeField] private float damageMultiplier = 1F;
        [SerializeField] private float range = 4F;
        [SerializeField] private bool isParriable = true;
        [SerializeField] private Vector3 offset = Vector3.zero;
        [SerializeField] private bool overrideEffect = false;
        [SerializeField] private ParticleSystem effectOverride = null;
        [SerializeField] private bool overrideEffectScale = false;
        [SerializeField] private Vector3 effectScale = new Vector3(1F, 0.7F, 1F);
        [SerializeField] private bool overrideColliderAngle = false;
        [SerializeField] private float colliderAngle = 100F;

        public bool OverrideColliderAngle => overrideColliderAngle;

        public float ColliderAngle => colliderAngle;

        public float Duration => duration;

        public bool OverrideEffectScale => overrideEffectScale;

        public Vector3 EffectScale => effectScale;

        public float DamageMultiplier => damageMultiplier;

        public float Range => range;

        public Action OnComplete { get; private set; } = delegate { };

        public Action OnStart { get; private set; } = delegate { };

        public bool OverrideEffect => overrideEffect;

        public ParticleSystem Effect => effectOverride;

        public bool IsParriable => isParriable;

        public Vector3 Offset => offset;

        private Attack()
        {
        }

        public static Builder Build(float duration, float range) => new Builder(duration, range);

        [System.Serializable]
        public class Builder
        {
            [SerializeField] private Attack attack;

            public Builder(float duration, float range)
            {
                attack = new Attack
                {
                    duration = duration,
                    range = range
                };
            }

            public static implicit operator Attack(Builder builder) => builder.attack;

            public Builder OverrideEffectScale(Vector3 effectScale)
            {
                attack.effectScale = effectScale;
                return this;
            }

            public Builder OverrideColiderAngle(float angle)
            {
                attack.overrideColliderAngle = true;
                attack.colliderAngle = angle;
                return this;
            }

            public Builder OverrideEffect(ParticleSystem effectOverride)
            {
                attack.overrideEffect = true;
                attack.effectOverride = effectOverride;

                return this;
            }

            public Builder OnComplete(Action OnComplete)
            {
                attack.OnComplete = OnComplete;
                return this;
            }

            public Builder Offset(float x, float y, float z)
            {
                attack.offset = new Vector3(x, y, z);
                return this;
            }

            public Builder OnStart(Action OnStart)
            {
                attack.OnStart = OnStart;
                return this;
            }

            public Builder NotParriable()
            {
                attack.isParriable = false;
                return this;
            }

            public Attack Build() => attack;

            public Builder DamageMultiplier(float multiplier)
            {
                attack.damageMultiplier = multiplier;
                return this;
            }
        }
    }
}