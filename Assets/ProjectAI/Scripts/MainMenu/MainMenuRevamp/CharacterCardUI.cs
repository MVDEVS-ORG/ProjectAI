using Assets.ProjectAI.Scripts.Player.Characters;
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
        private CharacterDetailsUI _characterDetailsUI;
        private CharacterSelectionView _characterSelectionView;

        public void Setup(CharacterDescriptionSO data, CharacterDetailsUI characterDetailsUI, CharacterSelectionView view)
        {
            _characterSelectionView = view;
            _characterDetailsUI = characterDetailsUI;
            _characterData = data;
            characterName.text = data.character.ToString();
            characterSprite.sprite = data.characterSprite;
            shortDescription.text = data.description;
            selectButton.onClick.AddListener(OpenCharacterDetails);
        }

        private void OpenCharacterDetails()
        {
            _characterDetailsUI.gameObject.SetActive(true);
            _characterDetailsUI.Setup(_characterData);
            _characterSelectionView.gameObject.SetActive(false);
        }
    }
}