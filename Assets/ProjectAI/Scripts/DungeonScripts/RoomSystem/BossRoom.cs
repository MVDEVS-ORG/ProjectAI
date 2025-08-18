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
        private Transform _player;
        private IPlayerController _playerController;

        public async void InitializeRoom(PlayerPicker playerPicker, IPlayerController playerController)
        {
            _playerController = playerController;
            await PathFindingManager.Instance.BakeFromTilemapsAsync();
            await playerPicker.SetPlayer();
            await playerController.SpawnPlayer(_playerSpawnTransform.position, playerPicker.PickPlayer());
            
            await WaitForPlayer();
        }

        async Awaitable WaitForPlayer()
        {
            // Wait until the player is within spawn radius
            _player = await _playerController.GetPlayerTransform();
            while (Vector3.Distance(_bossSpawnTransform.position, _player.position) > _spawnRadius)
            {
                //Debug.LogError(Vector3.Distance(transform.position, _player.position));
                await Awaitable.NextFrameAsync(); // Wait a frame before checking again
                _player = await _playerController.GetPlayerTransform();
            }

            // Once player is close enough, spawn the boss
            await _assetService.InstantiateWithPRAsync(
                AddressableIds.ORBReactor,
                _bossSpawnTransform.position,
                Quaternion.identity
            );
        }
    }
}