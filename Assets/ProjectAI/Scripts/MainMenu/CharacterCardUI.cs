using Assets.ProjectAI.Scripts.Player.Characters;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.ProjectAI.Scripts.MainMenu
{
    public class CharacterCardUI : MonoBehaviour
    {
        public Image characterSprite;
        public TextMeshProUGUI characterName;
        public TextMeshProUGUI shortDescription;
        public Button selectButton;

        private CharacterDescriptionSO _characterData;

        public void Setup(CharacterDescriptionSO data, Action<CharacterDescriptionSO> onClick)
        {
            _characterData = data;
            characterName.text = data.character.ToString();
            characterSprite.sprite = data.characterSprite;
            shortDescription.text = data.description;

            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(() => onClick?.Invoke(_characterData));
        }
    }
}