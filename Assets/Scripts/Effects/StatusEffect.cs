using GibFrame.ObjectPooling;
using GibFrame.Performance;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour, ICommonUpdate, IEquatable<StatusEffect>, IPooledObject
{
    public enum Behaviour { RESET, SINGLE, STACK }

    [Tooltip("RESET: when applying the status effect while the target already has a status effect of the same type active, reset the time of the active effect instead of instantiating a new one" +
        "\n\nSINGLE: apply the status effect only if there are no active effects of the same type" +
        "\n\nSTACK: instantiate this effect even if there is already one of the same type active")]
    [SerializeField] private Behaviour behaviour;
    [SerializeField] private float duration = 0F;
    private float currentTime = 0F;

    protected float Duration => duration;

    protected Transform Parent { get; private set; } = null;

    public void ApplyTo(Transform parent)
    {
        Transform holder;
        // The target has never been hit by a status effect, green pass
        if (!(holder = parent.Find("StatusEffectsHolder")))
        {
            GameObject holderObj = new GameObject()
            {
                name = "StatusEffectsHolder"
            };
            holderObj.transform.SetParent(parent);
            holderObj.transform.localPosition = Vector3.zero;
            holder = holderObj.transform;
            InstantiateEffectCopy(holder, parent);
        }
        else
        {
            // Get all the active effects of the same type
            List<StatusEffect> activeEffects = new List<StatusEffect>();
            Component[] components = parent.GetComponentsInChildren(Type.GetType(GetType().ToString()));
            foreach (Component component in components)
            {
                if (component is StatusEffect effect)
                {
                    activeEffects.Add(effect);
                }
            }
            switch (behaviour)
            {
                case Behaviour.STACK:
                    InstantiateEffectCopy(holder, parent);
                    break;

                case Behaviour.RESET:
                    if (activeEffects.Count > 0)
                    {
                        activeEffects.ForEach(e => e.ResetDuration(duration));
                    }
                    else
                    {
                        InstantiateEffectCopy(holder, parent);
                    }
                    break;

                case Behaviour.SINGLE:
                    if (activeEffects.Count <= 0)
                    {
                        InstantiateEffectCopy(holder, parent);
                    }
                    break;
            }
        }
    }

    public virtual void CommonUpdate(float deltaTime)
    {
        currentTime -= deltaTime;
        if (currentTime <= 0)
        {
            Destroy();
        }
    }

    public bool Equals(StatusEffect other)
    {
        return GetType().Equals(other.GetType());
    }

    public void OnObjectSpawn()
    {
        currentTime = duration;
    }

    protected virtual void OnSpawn()
    {
    }

    protected virtual void Destroy()
    {
        gameObject.SetActive(false);
        transform.SetParent(null);
    }

    protected bool GetStatusEffectFunctionalInterface<T>(out T functional)
    {
        functional = Parent.GetComponentInChildren<T>();
        return functional != null;
    }

    private void InstantiateEffectCopy(Transform holder, Transform parent)
    {
        StatusEffect effect = PoolManager.Instance.Spawn(Categories.STATUS_EFFECTS, name, Vector3.zero, Quaternion.identity).GetComponent<StatusEffect>();
        effect.transform.SetParent(holder);
        effect.transform.localPosition = Vector3.zero;
        effect.Parent = parent;
        effect.OnSpawn();
    }

    private void ResetDuration(float newDuration = -1F)
    {
        if (newDuration > 0F)
        {
            currentTime = newDuration;
        }
        else
        {
            currentTime = duration;
        }
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