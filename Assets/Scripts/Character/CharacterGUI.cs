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
        interactionIndicator.sprite = icon;
        interactionIndicator.color = color;
    }

    public void SetInteraction(Sprite icon = null)
    {
        interactionIndicator.sprite = icon != null ? icon : Assets.Sprites.Transparent;
        interactionIndicator.color = Color.white;
    }

    protected override void OnGameEnded(bool win)
    {
        base.OnGameEnded(win);
        victoryPanel.SetActive(win);
        defeatPanel.SetActive(!win);
    }

    protected override void Start()
    {
        base.Start();
        BindInput();
        BindCooldowns();
    }

    private void BindInput()
    {
        dodgeButton.AddOnPressedCallback(new Callback(() => inputChannel.Broadcast(new Inputs.DirectionInputData(Inputs.InputType.DODGE, joystick.Direction * 40F))));
        attackButton.AddOnLongPressedCallback(new Callback(() => inputChannel.Broadcast(new Inputs.InputData(Inputs.InputType.PRIMARY_ABILITY))));
        attackButton.AddOnPressedCallback(new Callback(() => inputChannel.Broadcast(new Inputs.InputData(Inputs.InputType.BASE_ATTACK))));
    }

    private void BindCooldowns()
    {
        CooldownUI coolUI = Instantiate(cooldownUIPrefab, cooldownsParent);
        coolUI.Bind(Manager.Combat.EquippedAbility);
    }
}