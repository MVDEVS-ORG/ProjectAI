using UnityEngine;

public interface IGamePauseController
{
    bool IsPaused { get; }
    void PauseGame();
    void ResumeGame();
}
