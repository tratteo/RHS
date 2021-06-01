// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Inputs.cs
//
// All Rights Reserved

using UnityEngine;

public static class Inputs
{
    public enum InputType { BASE_ATTACK, MOVE, DODGE, PRIMARY_ABILITY }

    public class InputData
    {
        public InputType Type { get; private set; }

        public InputData(InputType type)
        {
            Type = type;
        }
    }

    public class DirectionInputData : InputData
    {
        public Vector2 Direction { get; private set; }

        public DirectionInputData(InputType type, Vector2 direction) : base(type)
        {
            Direction = direction;
        }
    }
}