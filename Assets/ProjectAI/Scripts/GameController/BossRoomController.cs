using Assets.ProjectAI.Scripts.DungeonScripts;
using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem;
using Assets.ProjectAI.Scripts.PathFinding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace Assets.ProjectAI.Scripts.GameController
{
    public class BossRoomController : IBossRoomController
    {
        [Inject] private IPlayerController _playerController;
        [Inject] private PlayerPicker _playerPicker;
        [Inject] private ISceneManager _sceneManager;
        [Inject] private IUpgradeController _upgradeController;
        [Inject] private ObjectPoolManager _poolManager;
        [Inject] private CameraController _cameraController;
        [Inject] private SignalBus _signalBus;
  
        private List<GameObject> _enemies = new List<GameObject>();
        private Vector2 _playerSpawnPositon;

        [Inject]
        public void Initialize()
        {
            _ = (this as IBossRoomController).InitializeBossRoom();
        }
        async Task IBossRoomController.InitializeBossRoom()
        {
            try
            {
                GameObject bossRoomObj = await _poolManager.SpawnObjectAsync(
                    AddressableIds.BossRoom_LVL_1, 
                    Vector2.zero, 
                    Quaternion.identity, 
                    ObjectPoolManager.PoolType.GameObjects
                    );
                bossRoomObj.TryGetComponent<BossRoom>(out var bossRoom);
                bossRoom.InitializeRoom(_playerPicker, _playerController, _cameraController, _signalBus);
                _playerController.EnableController(true);
                await _upgradeController.Initialize();
                await _sceneManager.FadeBack();
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);
            }
            
        }
    }
}