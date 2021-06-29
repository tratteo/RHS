// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : SimpleFxHandler.cs
//
// All Rights Reserved

using GibFrame;
using UnityEngine;

public class SimpleFxHandler : FxHandler
{
    [SerializeField] private ParticleSystem[] effects;

    public override void Display(GameObject caster, Vector3 position, Vector3 sourceDirection, Vector3 flip, Space space)
    {
        ParticleSystem sel = effects.PickRandom(UnityEngine.Random.Range(int.MinValue, int.MaxValue));
        sel.GetComponent<ParticleSystemRenderer>().flip = flip;
        switch (space)
        {
            case Space.LOCAL:
                sel.transform.SetParent(transform);

                break;

            case Space.WORLD:
                sel.transform.SetParent(null);
                sel.transform.localScale = sel.transform.localScale.AsPositive();
                break;
        }

        sel.transform.right = sourceDirection;
        sel.transform.localPosition = position;
        sel.Play();
    }
}