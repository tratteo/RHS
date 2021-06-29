using GibFrame;
using GibFrame.Performance;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorsGUI : MonoBehaviour, ICommonUpdate
{
    private List<Indicator> indicators;
    private Camera referenceCamera;
    [SerializeField] private TransformEventBus indicatorsEventBus;
    [SerializeField] private Parameters parameters;

    public static Indicator.Builder GetDefaulted(Transform transform)
    {
        return Indicator.Of(transform).Color(Color.red).Rotate(true).WithSize(100, 100).WithIcon(Assets.Sprites.Arrow);
    }

    public void InflateNew(Indicator.Builder factory)
    {
        indicators.Add(factory.Concretize(parameters.Parent));
    }

    public void RemoveIndicator(Transform target)
    {
        List<Indicator> matches = indicators.FindAll((i) => ReferenceEquals(target, i.Target));
        foreach (Indicator match in matches)
        {
            indicators.Remove(match);
            UnityEngine.Object.Destroy(match.Image.gameObject);
        }
    }

    public void CommonUpdate(float deltaTime)
    {
        for (int i = 0; i < indicators.Count; i++)
        {
            Indicator curr = indicators[i];
            if (curr.Target)
            {
                SetIndicatorTransform(curr);
                curr.Update();
            }
            else
            {
                curr.Image.gameObject.SetActive(false);
            }
        }

        indicators.RemoveAll((i) => !i.Target);
    }

    private void Awake()
    {
        indicators = new List<Indicator>();
        referenceCamera = Camera.main;
    }

    private void SetIndicatorTransform(Indicator curr)
    {
        Vector3 targetPosition = curr.Target.position;
        Vector3 screenPos = referenceCamera.WorldToScreenPoint(targetPosition);
        if (Mathf.Approximately(screenPos.z, 0))
        {
            return;
        }

        // save half screen resulution because we will need it often
        Vector3 halfScreen = new Vector3(Screen.width, Screen.height) / 2;

        // we don't want the Z-Value in our center-vector because it would cause problems when normalizing it
        Vector3 screenPosNoZ = screenPos;
        screenPosNoZ.z = 0;
        // get the vector from the center of the screen to the calculated screen position
        Vector3 screenCenterPos = screenPosNoZ - halfScreen;

        // we have to invert the vector when we are looking away from the target the vector is just projected on the view-plane, think
        // looking in a mirror
        if (screenPos.z < 0)
        {
            screenCenterPos *= -1;
        }

        // debug check, if the ray is pointing in the wanted direction
        // can only be seen with gizmos enabled, in scene view (3D Mode only)
        //Debug.DrawRay(halfScreen, screenCenterPos.normalized * 100000, Color.red);

        // check if the target is on screen
        if (!IsOnScreen(screenPos))
        {
            // if you have a arrow on your symbol, pointing in the direction, enable it here:
            curr.Image.gameObject.SetActive(true);

            // rotate it to point towards the target position
            if (curr.Rotate)
            {
                curr.Image.transform.rotation = Quaternion.FromToRotation(Vector3.up, screenCenterPos);
            }

            // normalized ScreenCenterPosition
            Vector3 norSCP = screenCenterPos.normalized;

            // avoid dividing by zero
            if (norSCP.x == 0)
            {
                norSCP.x = 0.01f;
            }
            if (norSCP.y == 0)
            {
                norSCP.y = 0.01f;
            }

            // stretch the normalized screenCenterPosition so that X is at the edge
            Vector3 xScreenCP = norSCP * (halfScreen.x / Mathf.Abs(norSCP.x));
            // stretch the normalized screenCenterPosition so that Y is at the edge
            Vector3 yScreenCP = norSCP * (halfScreen.y / Mathf.Abs(norSCP.y));

            // compare the streched vectors in length and use the smaller one
            if (xScreenCP.sqrMagnitude < yScreenCP.sqrMagnitude)
            {
                screenPos = halfScreen + xScreenCP;
            }
            else
            {
                screenPos = halfScreen + yScreenCP;
            }
        }
        else
        {
            // if you have a arrow on your symbol, pointing in the direction, disable it here:
            curr.Image.gameObject.SetActive(false);
        }

        // clamp the result, so we can always see the full marker/tracker image

        screenPos.z = 0;

        screenPos.x = Mathf.Clamp(screenPos.x, parameters.Padding.x, Screen.width - parameters.Padding.x);
        screenPos.y = Mathf.Clamp(screenPos.y, parameters.Padding.y, Screen.height - parameters.Padding.y);

        // set the transform position
        curr.Image.transform.position = screenPos;
    }

    private bool IsOnScreen(Vector3 pos)
    {
        return pos.x > parameters.Padding.x && pos.x < Screen.width - parameters.Padding.x && pos.y > parameters.Padding.y & pos.y < Screen.height - parameters.Padding.y;
    }

    private void OnEnable()
    {
        CommonUpdateManager.Register(this);
        indicatorsEventBus.OnEvent += Inflate;
    }

    private void Inflate(Transform transform)
    {
        InflateNew(GetDefaulted(transform));
    }

    private void OnDisable()
    {
        CommonUpdateManager.Unregister(this);
        indicatorsEventBus.OnEvent -= Inflate;
    }

    [Serializable]
    public class Parameters
    {
        [SerializeField] private Vector2 padding;
        [SerializeField, Guarded] private Transform parent;

        public Vector2 Padding => padding;

        public Transform Parent => parent;
    }

    public class Indicator : IEquatable<Indicator>
    {
        private Action<Indicator> UpdateCallback;

        public bool Rotate { get; private set; } = false;

        public Transform Target { get; private set; } = null;

        public Image Image { get; private set; } = null;

        private Indicator(Transform target)
        {
            GameObject obj = new GameObject();
            Target = target;
            obj.name = "Indicator_" + Target.name;
            Image = obj.AddComponent<Image>();
            Image.gameObject.SetActive(false);
            Image.raycastTarget = false;
            Image.sprite = Assets.Sprites.Arrow;
            Image.rectTransform.sizeDelta = new Vector2(100, 100);
        }

        public static Builder Of(Transform target) => new Builder(target);

        public void Update()
        {
            UpdateCallback?.Invoke(this);
        }

        public bool Equals(Indicator other)
        {
            return Target.Equals(other.Target);
        }

        public class Builder
        {
            private readonly Indicator indicator;

            public Builder(Transform target)
            {
                indicator = new Indicator(target);
            }

            public Builder Color(Color color)
            {
                indicator.Image.color = color;
                return this;
            }

            public Builder WithIcon(Sprite icon)
            {
                indicator.Image.sprite = icon;
                return this;
            }

            public Builder WithSize(Vector2 sizeDelta)
            {
                indicator.Image.rectTransform.sizeDelta = sizeDelta;
                return this;
            }

            public Builder WithSize(float x, float y)
            {
                return WithSize(new Vector2(x, y));
            }

            public Builder Rotate(bool rotate)
            {
                indicator.Rotate = rotate;
                return this;
            }

            public Builder UpdateCallback(Action<Indicator> UpdateCallback)
            {
                indicator.UpdateCallback = UpdateCallback;
                return this;
            }

            public Indicator Concretize(Transform parent)
            {
                indicator.Image.transform.SetParent(parent);
                indicator.Image.gameObject.SetActive(true);
                return indicator;
            }
        }
    }
}