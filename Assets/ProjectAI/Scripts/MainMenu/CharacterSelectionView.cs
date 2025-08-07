using Assets.ProjectAI.Scripts.Player.Characters;
using System;
using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.MainMenu
{
    public class CharacterSelectionView : MonoBehaviour
    {
        public Transform cardsContainer;
        public CharacterCardUI cardPrefab;
        public CharacterDetailsUI detailsPanel;

        public void HideDetails() => detailsPanel.gameObject.SetActive(false);
        public void ShowDetails(CharacterDescriptionSO data, Action<Character> OnCharacterSelected) => detailsPanel.Setup(data, OnCharacterSelected);
    }
}