using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.Player
{
    public class PlayerSelectionService
    {
        public Character SelectedCharacter { get; private set; }

        public void SelectCharacter(Character character)
        {
            SelectedCharacter = character;
            Debug.Log("Character selected: " + character);
        }
    }
}