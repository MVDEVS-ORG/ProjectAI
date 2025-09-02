using Assets.ProjectAI.Scripts.DungeonScripts;
using Assets.ProjectAI.Scripts.PathFinding;
using Assets.ProjectAI.Scripts.Player;
using Assets.Services;
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
    [Inject] private ISceneManager _sceneManager;
    [Inject] private IUpgradeController _upgradeController;
    [Inject] private PlayerSelectionService _playerSelectionService;

    private List<GameObject> _enemies = new List<GameObject>();

    [Inject]
    private void Initialize()
    {
        UnityEngine.Random.InitState(Environment.TickCount);
        Debug.Log("Game InitializeRoom started");
        _ = (this as IGameController).StartGame();
    }
    async Task IGameController.StartGame()
    {
        try
        {
            await _dungeonMapController.Initialize();
            _enemies = _dungeonMapController.GetAllSpawnedEnemies();
            _playerController.EnableController(true);
            await _upgradeController.Initialize();
            await _sceneManager.FadeBack();
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
        }
        //await _playerController.SpawnPlayer(Vector3.zero, _playerPicker.PickPlayer());
    }
}
