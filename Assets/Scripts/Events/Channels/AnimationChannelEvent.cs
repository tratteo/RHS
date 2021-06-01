// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : AnimationChannelEvent.cs
//
// All Rights Reserved

using UnityEngine;
using static AnimationChannelEvent;

[CreateAssetMenu(fileName = "AnimationChannel", menuName = "Scriptable Objects/Channels/AnimationChannel", order = 0)]
public class AnimationChannelEvent : SingleParamChannelEvent<AnimationData>
{
    public class AnimationData
    {
        public string Id { get; private set; }

        public object Arg { get; private set; }

        public AnimationData(string id, object args)
        {
            Id = id;
            Arg = args;
        }

        public T GetArgAs<T>() => (T)Arg;
    }
}