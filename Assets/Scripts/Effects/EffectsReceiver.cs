// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : EffectsReceiver.cs
//
// All Rights Reserved

using GibFrame.Performance;
using System.Collections.Generic;

public class EffectsReceiver : ICommonUpdate
{
    private readonly List<ActiveEffect> activeEffects;
    private readonly object parent;

    public bool Active { get; private set; }

    public EffectsReceiver(object parent)
    {
        activeEffects = new List<ActiveEffect>();
        this.parent = parent;
    }

    ~EffectsReceiver()
    {
        CommonUpdateManager.Unregister(this);
    }

    public void AddEffects(params StatusEffect[] effects)
    {
        foreach (StatusEffect effect in effects)
        {
            if (!TryReset(effect))
            {
                effect.ApplyTo(parent);
                activeEffects.Add(new ActiveEffect(effect));
            }
        }
    }

    public void SetActive(bool active)
    {
        Active = active;
        if (active)
        {
            CommonUpdateManager.Register(this);
        }
        else
        {
            CommonUpdateManager.Unregister(this);
        }
    }

    public void Purge()
    {
        foreach (ActiveEffect effect in activeEffects)
        {
            if (effect != null)
            {
                effect.Effect.RemoveFrom(parent);
            }
        }
        activeEffects.Clear();
    }

    public void CommonUpdate(float deltaTime)
    {
        if (Active)
        {
            foreach (ActiveEffect effect in activeEffects)
            {
                if (effect != null)
                {
                    effect.Step(deltaTime);
                    if (effect.IsExpired())
                    {
                        effect.Effect.RemoveFrom(parent);
                    }
                }
            }
            activeEffects.RemoveAll((e) => e.IsExpired());
        }
    }

    private bool TryReset(StatusEffect effect)
    {
        foreach (ActiveEffect cur in activeEffects)
        {
            if (cur.Effect.GetType().Equals(effect.GetType()))
            {
                cur.Reset(effect.Duration);
                return true;
            }
        }
        return false;
    }

    private class ActiveEffect
    {
        private float currentTime;

        public StatusEffect Effect { get; private set; }

        public ActiveEffect(StatusEffect effect)
        {
            Effect = effect;
        }

        public void Step(float deltaTime)
        {
            currentTime += deltaTime;
        }

        public void Reset(float duration)
        {
            currentTime = 0F;
            Effect.EditDuration(duration);
        }

        public bool IsExpired()
        {
            return Effect.Duration != -1F && currentTime >= Effect.Duration;
        }
    }
}