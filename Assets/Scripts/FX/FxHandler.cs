// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : FxHandler.cs
//
// All Rights Reserved

using UnityEngine;

public abstract class FxHandler : MonoBehaviour
{
    public enum Space { LOCAL, WORLD }

    public abstract void Display(GameObject caster, Vector3 position, Vector3 sourceDirection, Vector3 flip, Space space);

    public virtual void Display(GameObject caster)
    {
        Display(caster, caster.transform.position, Vector2.right, Vector3.zero, Space.LOCAL);
    }

    public virtual void Display(GameObject caster, Vector3 position, Vector3 sourceDirection)
    {
        Display(caster, position, sourceDirection, Vector3.zero, Space.LOCAL);
    }

    public virtual void Display(GameObject caster, Vector3 position, Vector3 sourceDirection, Vector3 flip)
    {
        Display(caster, position, sourceDirection, flip, Space.LOCAL);
    }
}