using GibFrame.ObjectPooling;
using GibFrame.Performance;
using System;
using UnityEngine;

public class StatusEffect : MonoBehaviour, ICommonUpdate, IEquatable<StatusEffect>, IPooledObject
{
    [SerializeField] private bool stackable = false;
    [SerializeField] private bool resetDuration = true;
    [SerializeField] private float duration = 0F;
    private float currentTime = 0F;

    protected Transform Parent { get; private set; } = null;

    public static void Apply(Transform parent, StatusEffect statusEffectPrefab)
    {
        Transform holder;
        if (!(holder = parent.Find("StatusEffectsHolder")))
        {
            GameObject holderObj = new GameObject()
            {
                name = "StatusEffectsHolder"
            };
            holderObj.transform.SetParent(parent);
            holderObj.transform.localPosition = Vector3.zero;
            holder = holderObj.transform;
            StatusEffect obj = PoolManager.Instance.Spawn(Categories.STATUS_EFFECTS, statusEffectPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<StatusEffect>();
            obj.transform.SetParent(holder);
            obj.transform.localPosition = Vector3.zero;
            obj.Parent = parent;
        }
        else
        {
            StatusEffect eff = (StatusEffect)parent.GetComponentInChildren(Type.GetType(statusEffectPrefab.GetType().ToString()));
            if (eff != null && statusEffectPrefab.resetDuration)
            {
                eff.ResetDuration(statusEffectPrefab.duration);
            }
            if (statusEffectPrefab.stackable || eff == null)
            {
                StatusEffect obj = PoolManager.Instance.Spawn(Categories.STATUS_EFFECTS, statusEffectPrefab.name, Vector3.zero, Quaternion.identity).GetComponent<StatusEffect>();
                obj.transform.SetParent(holder);
                obj.transform.localPosition = Vector3.zero;
                obj.Parent = parent;
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