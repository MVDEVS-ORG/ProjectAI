using System;
using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.GameController
{
    public class LevelManager
    {
        public int CurrentLevel { get; private set; }

        public event Action<int> OnLevelChanged; //event to notify when Level has Changed

        public LevelManager()
        {
            CurrentLevel = 1;
        }

        /// <summary>
        /// Sets level to 1. Call this when you go to Main Menu.
        /// </summary>
        public void ResetLevel()
        {
            CurrentLevel = 1;
            OnLevelChanged?.Invoke(CurrentLevel);
        }

        /// <summary>
        /// Increases level by 1. Call this when a boss is defeated.
        /// </summary>
        public void IncreaseLevel()
        {
            CurrentLevel++;
            OnLevelChanged?.Invoke(CurrentLevel);
        }

        /// <summary>
        /// Allows setting level manually (e.g. for debug or load system).
        /// </summary>
        public void SetLevel(int Level)
        {
            CurrentLevel = Level;
            OnLevelChanged?.Invoke(CurrentLevel);
        }
    }
}