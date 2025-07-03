using Assets.ProjectAI.Scripts.DungeonScripts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class GameController : IGameController
{
    [Inject] private IPlayerController _playerController;
    [Inject] private PlayerPicker _playerPicker;
    [Inject] private DungeonMapController _dungeonMapController;

    private List<GameObject> _enemies = new List<GameObject>();

    [Inject]
    private void Initialize()
    {
        Debug.Log("Game Initialize started");
        _ = (this as IGameController).StartGame();
    }
    async Task IGameController.StartGame()
    {
        try
        {
            await _playerPicker.SetPlayer();
            await _dungeonMapController.Initialize();
            _enemies = _dungeonMapController.GetAllSpawnedEnemies();
            Debug.LogError($"Spawned Enemies are: {_enemies.Count}");
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
        }
        //await _playerController.SpawnPlayer(Vector3.zero, _playerPicker.PickPlayer());
    }
}
