// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : ItemUI.cs
//
// All Rights Reserved

using GibFrame;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour, IDragHandler, IPointerUpHandler, IPointerDownHandler, IPointerExitHandler, IEquatable<ItemUI>
{
    private static readonly Color SELECTED_COLOR = new Color(255 / 255F, 160 / 255F, 50 / 255F, 1);
    private readonly float pressTime = 0.15F;
    private Text title;
    private bool dragging = false;
    private GameObject placeHolder;
    private float currentPressTime = 0F;
    private bool requestingDrag;
    private bool dragged = false;
    private PointerEventData dragData;
    private Image icon;
    private Image border;
    private RectTransform rectTransform;
    private IEquipmentManager equipmentManager;
    private bool showTooltip = false;
    private RectTransform parentRectTransform;
    private Color initialColor;

    public Vector3 LocalPosition { set => placeHolder.transform.localPosition = value; }

    public GraphicRaycaster Raycaster { get; set; }

    public Item Item { get; private set; }

    public InventoryComponent CurrentInventory { get; set; }

    private bool CanDrag { get => (currentPressTime >= pressTime); }

    public void MoveTo(Transform parent)
    {
        placeHolder.transform.SetParent(parent, false);
        transform.SetParent(placeHolder.transform);
        transform.localPosition = Vector3.zero;
        InventoryComponent inventory = parent.GetComponent<InventoryComponent>();
        if (inventory != null && inventory.MaxItems == 1)
        {
            parentRectTransform.sizeDelta = parent.GetComponent<RectTransform>().sizeDelta;
        }
    }

    public void Destroy()
    {
        Destroy(placeHolder);
    }

    public void Setup(Item item, GraphicRaycaster raycaster, GameObject placeHolder, bool tooltip = true)
    {
        showTooltip = tooltip;
        Item = item;
        SetUI();
        Raycaster = raycaster;
        this.placeHolder = placeHolder;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (CanDrag)
        {
            if (!dragging)
            {
                dragging = true;
                bool extracted = RaycastTargetsAndExecute<bool, InventoryComponent>(eventData, (slot) =>
                {
                    if (slot.CanExtract(this))
                    {
                        transform.SetParent(Raycaster.transform);
                        transform.SetAsLastSibling();
                        return true;
                    }
                    return false;
                });
                dragData = eventData;
            }
        }
    }

    public bool Equals(ItemUI other)
    {
        return Item.Equals(other.Item);
    }

    public bool TryEnhance(float rate)
    {
        if (Item is IEnhanceable)
        {
            (Item as IEnhanceable).Enhance(rate);
            SetUI();
            return true;
        }
        return false;
    }

    public bool TryUnhenance()
    {
        if (Item is IEnhanceable)
        {
            (Item as IEnhanceable).Unhenance();
            SetUI();
            return true;
        }
        return false;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (dragging)
        {
            bool inserted = false;
            inserted = RaycastTargetsAndExecute<bool, InventoryComponent>(eventData, (slot) =>
            {
                if (ReferenceEquals(slot, CurrentInventory))
                {
                    return false;
                }
                if (equipmentManager != null && !equipmentManager.CanEquip(this, slot))
                {
                    return false;
                }
                else if (slot.CanDragItem && slot.CanInsert(this))
                {
                    if (CurrentInventory.Extract(this) && slot.Insert(this))
                    {
                        CurrentInventory = slot;
                        Vibration.OneShot(Vibration.CLICK);
                        return true;
                    }
                    return false;
                }
                return false;
            });

            if (!inserted)
            {
                transform.SetParent(placeHolder.transform);
                transform.localPosition = Vector3.zero;
            }
        }
        if (currentPressTime < pressTime && requestingDrag)
        {
            ShowTooltip();
        }
        dragData = null;
        dragging = false;
        requestingDrag = false;
        dragged = false;
        currentPressTime = 0F;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!dragging)
        {
            requestingDrag = true;
            currentPressTime = 0;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!dragging)
        {
            currentPressTime = 0;
            requestingDrag = false;
            dragged = false;
        }
    }

    private void ShowTooltip()
    {
        if (!showTooltip) return;
        //Vector2 pos = new Vector2(parentRectTransform.position.x - Screen.width / 2, parentRectTransform.position.y - Screen.height / 2 + parentRectTransform.sizeDelta.y / 1.5F);
        //Tooltip tooltip = Tooltip.Inflate(Global.UI.TooltipTitle(Item.GetName()) + "\n" + Item.ToString(), Factories.UI.PREFERENCES.At(pos).ElementPadding(1.33F * parentRectTransform.sizeDelta.y));
        //AudioManager.Instance.PlaySound("Tooltip");
        //border.color = SELECTED_COLOR;
        //tooltip.AddOnDestroyedCallback((t) => border.color = initialColor);
    }

    private void SetUI()
    {
        title.text = Item.GetName();
        //switch (Item.Rarity)
        //{
        //    case Item.RarityType.COMMON:
        //        icon.sprite = AssetsProvider.UI.SP.Transparent_S;
        //        break;

        // case Item.RarityType.RARE: icon.sprite = AssetsProvider.UI.SP.Rare_S; break;

        // case Item.RarityType.EPIC: icon.sprite = AssetsProvider.UI.SP.Epic_S; break;

        //    case Item.RarityType.LEGENDARY:
        //        icon.sprite = AssetsProvider.UI.SP.Legendary_S;
        //        break;
        //}
    }

    private void Awake()
    {
        title = GetComponentInChildren<Text>();
        border = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
        icon = transform.GetFirstComponentInChildrenWithName<Image>("Icon", true);
        equipmentManager = UnityUtils.GetFirstInterfaceOfType<IEquipmentManager>();
        parentRectTransform = transform.parent.GetComponent<RectTransform>();
        initialColor = border.color;
    }

    private void Update()
    {
        if (requestingDrag)
        {
            currentPressTime += Time.deltaTime;
        }
        if (!dragged)
        {
            if (currentPressTime >= pressTime)
            {
                Vibration.OneShot(Vibration.CLICK);
                dragged = true;
                requestingDrag = false;
            }
            else
            {
                //Tooltip.Inflate("Equipment", Item.ToString(), Factories.UI.PREFERENCES.At(transform.position));
            }
        }

        if (dragging)
        {
            transform.position = dragData.position;
        }
    }

    private T RaycastTargetsAndExecute<T, E>(PointerEventData eventData, Func<E, T> func, params Func<E, bool>[] predicates) where E : Component
    {
        List<RaycastResult> results = new List<RaycastResult>();
        Raycaster.Raycast(eventData, results);

        foreach (RaycastResult res in results)
        {
            E target;
            if ((target = res.gameObject.GetComponent<E>()) != null)
            {
                bool accepted = true;
                foreach (Func<E, bool> predicate in predicates)
                {
                    if (!predicate(target))
                    {
                        accepted = false;
                        break;
                    }
                }
                if (accepted)
                {
                    return func(target);
                }
            }
        }
        return default;
    }
}