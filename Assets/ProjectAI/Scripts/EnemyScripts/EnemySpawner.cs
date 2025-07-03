using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem;
using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items;
using Assets.ProjectAI.Scripts.HelperClasses;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.EnemyScripts
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private PrefabPlacer _prefabPlacer;
        [SerializeField] private float _detectionRange = 4f;

        private ObjectPoolManager _poolManager;
        private List<EnemyPlacementData> _enemyPlacementData = new List<EnemyPlacementData>();
        private ItemPlacementHelper _itemPlacementHelper;
        private Transform _playerTransform;

        private bool _hasSpawned = false;

        public void InitializeSpawner(ObjectPoolManager opManager, List<EnemyPlacementData> enemyPlacementData, ItemPlacementHelper itemPlacementHelper, Transform playerView)
        {
            _poolManager = opManager;
            _enemyPlacementData = enemyPlacementData;
            _itemPlacementHelper = itemPlacementHelper;
            _playerTransform = playerView;
            //_prefabPlacer.PlaceEnemies(enemyPlacementData, itemPlacementHelper)
        }

        private void Update()
        {
            if (_playerTransform == null || _hasSpawned) return;
            //Check for player position and spawn enemies when in Range
            if (Vector3.Distance(transform.position, _playerTransform.position) <= _detectionRange)
            {
                Debug.LogError("Spawing enemies");
                _hasSpawned = true; // Ensure this runs only once (optional, depends on your design)
                SpawnEnemies();
            }
        }
        private async void SpawnEnemies()
        {
            await _prefabPlacer.PlaceEnemies(_poolManager, _enemyPlacementData, _itemPlacementHelper, _playerTransform);
        }
    }
}