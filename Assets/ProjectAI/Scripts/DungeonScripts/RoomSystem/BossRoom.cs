using Assets.ProjectAI.Scripts.PathFinding;
using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem
{
    public class BossRoom : MonoBehaviour
    {
        [SerializeField] private Transform _playerSpawnTransform;
        [SerializeField] private Transform _bossSpawnTransform;

        public async void InitializeRoom(PlayerPicker playerPicker, IPlayerController playerController)
        {
            await PathFindingManager.Instance.BakeFromTilemapsAsync();
            await playerPicker.SetPlayer();
            await playerController.SpawnPlayer(_playerSpawnTransform.position, playerPicker.PickPlayer());
        }
    }
}