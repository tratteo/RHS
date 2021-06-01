// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Sword.Slash.cs
//
// All Rights Reserved

using System;
using UnityEngine;

public partial class Sword
{
    [System.Serializable]
    public class Slash
    {
        [SerializeField] private float duration = 0.02F;
        [SerializeField] private float damageMultiplier = 1F;
        [SerializeField] private float range = 4F;
        [SerializeField] private bool isParriable = true;
        [SerializeField] private Vector3 offset = Vector3.zero;

        public float Duration => duration;

        public float DamageMultiplier => damageMultiplier;

        public float Range => range;

        public Action OnComplete { get; private set; } = delegate { };

        public Action OnStart { get; private set; } = delegate { };

        public bool IsParriable => isParriable;

        public Vector3 Offset => offset;

        private Slash()
        {
        }

        public static Builder Build(float duration, float range) => new Builder(duration, range);

        [System.Serializable]
        public class Builder
        {
            [SerializeField] private Slash slash;

            public Builder(float duration, float range)
            {
                slash = new Slash
                {
                    duration = duration,
                    range = range
                };
            }

            public static implicit operator Slash(Builder builder) => builder.slash;

            public Builder OnComplete(Action OnComplete)
            {
                slash.OnComplete = OnComplete;
                return this;
            }

            public Builder Offset(float x, float y, float z)
            {
                slash.offset = new Vector3(x, y, z);
                return this;
            }

            public Builder OnStart(Action OnStart)
            {
                slash.OnStart = OnStart;
                return this;
            }

            public Builder Parriable(bool parriable)
            {
                slash.isParriable = parriable;
                return this;
            }

            public Slash Build() => slash;

            public Builder DamageMultiplier(float multiplier)
            {
                slash.damageMultiplier = multiplier;
                return this;
            }
        }
    }
}