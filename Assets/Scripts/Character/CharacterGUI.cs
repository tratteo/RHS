// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : CharacterGUI.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.Joystick;
using GibFrame.UI;
using UnityEngine;
using UnityEngine.UI;

public class CharacterGUI : CharacterComponent
{
    [Header("Channels")]
    [SerializeField, Guarded] private InputChannelEvent inputChannel;
    [Header("Prefabs")]
    [SerializeField, Guarded] private CooldownUI cooldownUIPrefab;
    [Header("UI")]
    [SerializeField, Guarded] private Image interactionIndicator;
    [SerializeField, Guarded] private Transform cooldownsParent;
    [SerializeField, Guarded] private Joystick joystick;
    [SerializeField, Guarded] private GButton attackButton;
    [SerializeField, Guarded] private GButton dodgeButton;
    [SerializeField, Guarded] private GameObject victoryPanel;
    [SerializeField, Guarded] private GameObject defeatPanel;

    public override void CommonUpdate(float deltaTime)
    {
        inputChannel.Broadcast(new Inputs.DirectionInputData(Inputs.InputType.MOVE, joystick.Direction));
    }

    public void SetInteraction(Sprite icon, Color color)
    {
        if (Manager.Combat.IsStunned) return;
        interactionIndicator.sprite = icon;
        interactionIndicator.color = color;
    }

    public void SetInteraction(Sprite icon = null)
    {
        SetInteraction(icon != null ? icon : Assets.Sprites.Transparent, Color.white);
    }

    public void BindCooldown(ICooldownOwner owner)
    {
        CooldownUI.Attach(owner, cooldownUIPrefab, cooldownsParent);
    }

    protected override void OnDeath(bool win)
    {
        base.OnDeath(win);
        joystick.gameObject.SetActive(false);
        victoryPanel.SetActive(win);
        defeatPanel.SetActive(!win);
    }

    protected override void Start()
    {
        base.Start();
        BindInput();
        dodgeButton.gameObject.GetComponentInChildren<CooldownUI>(true).Bind(Manager.Kinematic);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        EventBus.OnStunEvent.Invocation += OnStun;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventBus.OnStunEvent.Invocation -= OnStun;
    }

    private void BindInput()
    {
        dodgeButton.AddOnPressedCallback(new Callback(() => inputChannel.Broadcast(new Inputs.DirectionInputData(Inputs.InputType.DODGE, joystick.Direction))));
        attackButton.AddOnLongPressedCallback(new Callback(() => inputChannel.Broadcast(new Inputs.InputData(Inputs.InputType.PRIMARY_ABILITY))));
        attackButton.AddOnPressedCallback(new Callback(() => inputChannel.Broadcast(new Inputs.InputData(Inputs.InputType.BASE_ATTACK))));
    }

    private void OnStun(bool stun)
    {
        if (stun)
        {
            interactionIndicator.sprite = Assets.Sprites.Stun;
            interactionIndicator.color = Color.white;
        }
        else
        {
            interactionIndicator.sprite = Assets.Sprites.Transparent;
            interactionIndicator.color = Color.white;
        }
    }
}