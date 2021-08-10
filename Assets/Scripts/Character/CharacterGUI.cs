// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : CharacterGUI.cs
//
// All Rights Reserved

using GibFrame;
using GibFrame.Joystick;
using GibFrame.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class CharacterGUI : CharacterComponent
{
    [Header("Channels")]
    [SerializeField, Guarded] private SessionStatisticsEventBus statisticsEventBus;
    //[Header("Prefabs")]
    //[SerializeField, Guarded] private CooldownUI cooldownUIPrefab;
    [Header("UI")]
    [SerializeField, Guarded] private Image interactionIndicator;
    [SerializeField, Guarded] private Transform cooldownsParent;
    [SerializeField, Guarded] private Joystick joystick;
    [SerializeField, Guarded] private GButton attackButton;
    [SerializeField, Guarded] private GButton dodgeButton;
    [SerializeField, Guarded] private GameObject victoryPanel;
    [SerializeField, Guarded] private GameObject defeatPanel;
    [SerializeField, Guarded] private StatisticsDisplay statsDisplay;

    public override void CommonUpdate(float deltaTime)
    {
        InputBus.Broadcast(new Inputs.DirectionInputData(Inputs.InputType.MOVE, joystick.Direction));

        if (Input.GetKeyDown(KeyCode.D))
        {
            InputBus.Broadcast(new Inputs.DirectionInputData(Inputs.InputType.DODGE, joystick.Direction));
        }
    }

    public void SetInteraction(Sprite icon, Color color)
    {
        if (Manager.Combat.IsStunned) return;
        interactionIndicator.sprite = icon;
        interactionIndicator.color = color;
    }

    public void SetInteraction(Sprite icon = null)
    {
        SetInteraction(icon != null ? icon : Assets.Sprites.Transparent, Color.black);
    }

    public void BindAbilityCooldown(ICooldownOwner owner)
    {
        attackButton.gameObject.GetComponentInChildren<CooldownUI>(true).Bind(owner);
        //CooldownUI.Attach(owner, cooldownUIPrefab, cooldownsParent);
    }

    protected override void OnGameEnded(bool win)
    {
        base.OnGameEnded(win);
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
        Manager.Combat.OnStun += OnStun;
        statisticsEventBus.OnEvent += OnStatisticsReceived;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        Manager.Combat.OnStun -= OnStun;
        statisticsEventBus.OnEvent -= OnStatisticsReceived;
    }

    private void OnStatisticsReceived(List<Statistic> stats)
    {
        statsDisplay.Display(stats);
    }

    private void BindInput()
    {
        dodgeButton.AddOnPressedCallback(new Callback(() => InputBus.Broadcast(new Inputs.DirectionInputData(Inputs.InputType.DODGE, joystick.Direction))));
        attackButton.AddOnLongPressedCallback(new Callback(() => InputBus.Broadcast(new Inputs.InputData(Inputs.InputType.PRIMARY_ABILITY))));
        attackButton.AddOnPressedCallback(new Callback(() => InputBus.Broadcast(new Inputs.InputData(Inputs.InputType.BASE_ATTACK))));
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