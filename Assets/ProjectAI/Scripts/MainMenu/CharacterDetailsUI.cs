using Assets.ProjectAI.Scripts.Player.Characters;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.ProjectAI.Scripts.MainMenu
{
    public class CharacterDetailsUI : MonoBehaviour
    {
        public Image characterSprite;
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI description;
        public Image passiveAbilitySprite;
        public TextMeshProUGUI passiveDesc;
        public Image activeAbilitySprite;
        public TextMeshProUGUI activeDesc;

        public Button changeAvatarButton;
        public Button enterDungeonButton;

        public void Setup(CharacterDescriptionSO data, Action<Character> OnCharacterSelected)
        {
            characterName.text = data.character.ToString();
            characterSprite.sprite = data.characterSprite;
            description.text = data.description;

            passiveAbilitySprite.sprite = data.passiveAbility.abilitySprite;
            passiveDesc.text = data.passiveAbility.description;

            activeAbilitySprite.sprite = data.activeAbility.abilitySprite;
            activeDesc.text = data.activeAbility.description;

            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(changeAvatarButton.gameObject);

            enterDungeonButton.onClick.AddListener(() => OnCharacterSelected?.Invoke(data.character));

            gameObject.SetActive(true);
        }
    }
}