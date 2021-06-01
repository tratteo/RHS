// Copyright (c) Matteo Beltrame
//
// Package com.Siamango.RHS : Assets.cs
//
// All Rights Reserved

using GibFrame;
using System.Collections.Generic;
using UnityEngine;

public class Assets : MonoSingleton<Assets>
{
    [Header("Assets")]
    [SerializeField] private SpritesAssets sprites;
    [SerializeField] private AbilitiesAssets abilities;

    public static AbilitiesAssets Abilities => Instance.abilities;

    public static SpritesAssets Sprites => Instance.sprites;

    [System.Serializable]
    public class SpritesAssets
    {
        [SerializeField] private Sprite arrow;
        [SerializeField] private Sprite exclamation;
        [SerializeField] private Sprite transparent;
        [SerializeField] private Sprite stun;

        public Sprite Arrow => arrow;

        public Sprite Exclamation => exclamation;

        public Sprite Transparent => transparent;

        public Sprite Stun => stun;
    }

    [System.Serializable]
    public class AbilitiesAssets
    {
        [SerializeField] private List<GameObject> characterAbilities;

        public bool TryGetAbilityPrefabById(string id, out GameObject ability)
        {
            ability = characterAbilities.Find(a => a.GetComponent<IDescribable>().GetId().Equals(id));
            if (ability == null)
            {
                return false;
            }
            else
            {
                return ability;
            }
        }
    }
}