using Assets.ProjectAI.Scripts.DungeonScripts;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class GameController : IGameController
{
    [Inject] private IPlayerController _playerController;
    [Inject] private PlayerPicker _playerPicker;
    [Inject] private DungeonMapController _dungeonMapController;

    [Inject]
    private void Initialize()
    {
        _ = (this as IGameController).StartGame();
    }
    async Task IGameController.StartGame()
    {
        await _dungeonMapController.Initialize();
        await _playerPicker.SetPlayer();
        await _playerController.SpawnPlayer(Vector3.zero, _playerPicker.PickPlayer());
    }
}
