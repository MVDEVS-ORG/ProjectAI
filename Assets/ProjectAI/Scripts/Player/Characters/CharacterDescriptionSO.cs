using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.Player.Characters
{
    [CreateAssetMenu(fileName = "Character Description", menuName = "Scriptable Objects/CharactersDescriptionSO")]
    public class CharacterDescriptionSO : ScriptableObject
    {
        public Character character;
        [TextArea(2, 4)]
        public string description;
        public Sprite characterSprite;

        [Header("Passive Ability Section")]
        public Ability passiveAbility;
        [Header("Active Ability Section")]
        public Ability activeAbility;

    }

    [Serializable]
    public class Ability
    {
        public Sprite abilitySprite;
        [TextArea(2,3)]
        public string description;
    }
}