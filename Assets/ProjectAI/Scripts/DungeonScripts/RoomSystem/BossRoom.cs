using Assets.ProjectAI.Scripts.EnemyScripts.Bosses;
using Assets.ProjectAI.Scripts.PathFinding;
using Assets.ProjectAI.Scripts.Player;
using Assets.Services;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem
{
    public class BossRoom : MonoBehaviour
    {
        [SerializeField] private Transform _playerSpawnTransform;
        [SerializeField] private Transform _bossSpawnTransform;
        [SerializeField] private float _spawnRadius = 5f;
        [SerializeField] private GameObject _emergencyWalls;
        [SerializeField] private GameObject _door;
        [SerializeField] private Transform _camTransform;

        [Inject] private IAssetService _assetService;
        [Inject] private ObjectPoolManager _poolManager;
        [Inject] private PlayerSelectionService _playerSelectionService;
        
        private SignalBus _signalBus;

        private CameraController _cameraController;
        private Transform _player;
        private IPlayerController _playerController;

        public async void InitializeRoom(PlayerPicker playerPicker, IPlayerController playerController, CameraController cameraController, SignalBus signalBus)
        {
            _cameraController = cameraController;
            _playerController = playerController;
            _signalBus = signalBus;
            //await PathFindingManager.Instance.BakeFromTilemapsAsync();
# if UNITY_EDITOR
            await playerPicker.SetPlayer();
#endif
            Character selected = _playerSelectionService.SelectedCharacter;
            await playerController.SpawnPlayer(_playerSpawnTransform.position, playerPicker.SelectPlayer(selected));

            GameObject boss = await _assetService.InstantiateWithPRAsync(
                AddressableIds.ORBReactor,
                _bossSpawnTransform.position,
                Quaternion.identity
            );
            ORBReactor ORB = boss.GetComponent<ORBReactor>();
            ORB.InitializeBoss(_poolManager, _assetService, _playerController, _cameraController, _camTransform, _signalBus);
            ORB.EmergencyWalls = _emergencyWalls;
            ORB.BossRoomDoor = _door;
        }
    }
}