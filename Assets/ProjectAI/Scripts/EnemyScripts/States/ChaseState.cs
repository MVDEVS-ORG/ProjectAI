using System.Collections.Generic;
using Assets.ProjectAI.Scripts.EnemyScripts;
using Assets.ProjectAI.Scripts.PathFinding;
using UnityEngine;

public class ChaseState : IEnemyState
{
    private EnemyAI _enemy;
    private Transform _player;
    private float _pathRefreshTime = Random.Range(0.5f, 1f);
    private float _timer;

    public void Enter(EnemyAI enemy, Transform player, ObjectPoolManager op)
    {
        _enemy = enemy;
        _player = player;
        RequestPath();
        _timer = 0f;
        _enemy.animator?.SetBool("Walking", true);
    }

    public void Update()
    {
        if (!_enemy.IsPlayerVisible())
        {
            _enemy.TransitionToState(_enemy.GetNextStateFromMap(EnemyStateTypes.Search));
            return;
        }

        if (_enemy.IsPlayerInAttackRange())
        {
            _enemy.TransitionToState(_enemy.GetNextStateFromMap(EnemyStateTypes.Attack));
            return;
        }

        _timer += Time.deltaTime;
        if (_timer > _pathRefreshTime)
        {
            RequestPath();
            _timer = 0f;
        }
    }

    public void Exit()
    {
        _enemy.animator?.SetBool("Walking", false);
        _enemy = null;
        _player = null;
        _timer = 0f;
    }

    private void RequestPath()
    {
        Vector3 targetPos;
        List<Vector3Int> path = new();
        float distanceToPlayer = Vector3.Distance(_enemy.transform.position, _player.position);

        if (distanceToPlayer < 2f)
            targetPos = GetOffsetAroundPlayer(_enemy.gameObject, _player);
        else
            targetPos = _player.position + Vector3.down * 1.5f;

        // Convert positions to grid cells
        Vector2Int targetCell = Vector2Int.RoundToInt(targetPos);
        Vector2Int nearestValidTarget = PathFindingManager.Instance.GetNearestValidWalkableTile(targetCell);

        Vector3Int startCell = PathFindingManager.Instance.WorldToCell(_enemy.transform.position);

        // Ensure start cell is walkable too
        if (!PathFindingManager.Instance.IsWalkable((Vector2Int)startCell))
        {
            startCell = (Vector3Int)PathFindingManager.Instance.GetNearestValidWalkableTile((Vector2Int)startCell);
        }

        path = PathFindingManager.Instance.FindPath(startCell, (Vector3Int)nearestValidTarget);

        if (path == null || path.Count == 0)
        {
            Debug.LogError($"{_enemy.name} no path found s-{startCell}, e- {nearestValidTarget}");
            return;
        }

        _enemy.StartPathMovement(path);
    }

    private Vector3 GetOffsetAroundPlayer(GameObject self, Transform player)
    {
        List<GameObject> allEnemies = new List<GameObject>();
        foreach (var enemy in EnemyManager.spawnedEnemies)
        {
            if (Vector3.Distance(enemy.transform.position, player.position) < 6f)
            {
                allEnemies.Add(enemy);
            }
        }
        int index = allEnemies.IndexOf(self);
        int nearbyEnemyCount = allEnemies.Count;
        float angle = (360f / Mathf.Max(nearbyEnemyCount, 1)) * index;
        float radius = 1.5f; // Distance from the _player

        Vector3 offset = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad),
            0
        ) * radius;

        return _player.position + offset;
    }
}
