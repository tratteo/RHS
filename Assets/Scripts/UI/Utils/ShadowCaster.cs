using GibFrame.Performance;
using UnityEngine;

public class ShadowCaster : MonoBehaviour, ICommonUpdate
{
    [SerializeField] private Vector3 offset;
    private IShadowOwner shadow;
    private Transform ownerTransform;
    private float startY;
    private bool grounded = true;

    public void CommonUpdate(float deltaTime)
    {
        AdjustPosition();
    }

    private void Awake()
    {
        ownerTransform = transform.root;
        shadow = ownerTransform.GetComponent<IShadowOwner>();
        if (shadow == null)
        {
            Debug.LogWarning("Shadow caster has no shadow owner component");
        }
    }

    private void OnChangeGroundedState(bool state)
    {
        grounded = state;
        if (!state)
        {
            startY = ownerTransform.position.y;
        }
    }

    private void AdjustPosition()
    {
        if (shadow == null || grounded)
        {
            transform.position = ownerTransform.position + offset;
        }
        else
        {
            transform.position = new Vector3(ownerTransform.position.x, startY, 0F) + offset;
        }
    }

    private void OnEnable()
    {
        CommonUpdateManager.Register(this);
        shadow.OnChangeGroundedState += OnChangeGroundedState;
    }

    private void OnDisable()
    {
        CommonUpdateManager.Unregister(this);
        shadow.OnChangeGroundedState -= OnChangeGroundedState;
    }
}