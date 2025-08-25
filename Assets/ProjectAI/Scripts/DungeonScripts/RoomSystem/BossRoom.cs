using Assets.ProjectAI.Scripts.EnemyScripts.Bosses;
using Assets.ProjectAI.Scripts.PathFinding;
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

        [Inject] IAssetService _assetService;
        [Inject] ObjectPoolManager _poolManager;
        private Transform _player;
        private IPlayerController _playerController;

        public async void InitializeRoom(PlayerPicker playerPicker, IPlayerController playerController)
        {
            _playerController = playerController;
            await PathFindingManager.Instance.BakeFromTilemapsAsync();
            await playerPicker.SetPlayer();
            await playerController.SpawnPlayer(_playerSpawnTransform.position, playerPicker.PickPlayer());

            GameObject boss = await _assetService.InstantiateWithPRAsync(
                AddressableIds.ORBReactor,
                _bossSpawnTransform.position,
                Quaternion.identity
            );
            boss.GetComponent<ORBReactor>().Initilaize(_poolManager, _assetService, _playerController);
        }
    }
}