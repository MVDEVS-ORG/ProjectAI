using UnityEngine;
using System.Collections.Generic;
using Assets.ProjectAI.Scripts.PathFinding;
using Assets.ProjectAI.Scripts.EnemyScripts;

public class ChaseState : IEnemyState
{
    private EnemyAI _enemy;
    private Transform _player;
    private float _pathRefreshTime = Random.Range(0.5f, 1f);
    private float _timer;

    public void Enter(EnemyAI enemy, Transform player)
    {
        _enemy = enemy;
        _player = player;
        RequestPath();
        _timer = 0f;
    }

    public void Update()
    {
        if (!_enemy.IsPlayerVisible())
        {
            _enemy.TransitionToState(new SearchState());
            return;
        }

        if (_enemy.IsPlayerInAttackRange())
        {
            _enemy.TransitionToState(new AttackState());
            return;
        }

        _timer += Time.deltaTime;
        if (_timer > _pathRefreshTime)
        {
            RequestPath();
            _timer = 0f;
        }
    }

    public void Exit() { }

    private void RequestPath()
    {
        Vector3 targetPos = GetOffsetAroundPlayer(_enemy.gameObject, _player);
        Vector3Int start = _enemy.floorTilemap.WorldToCell(_enemy.transform.position);
        Vector3Int goal = _enemy.floorTilemap.WorldToCell(targetPos);
        List<Vector3Int> path = PathFindingManager.Instance.FindPath(start, goal);
        _enemy.StartPathMovement(path);
    }
    private Vector3 GetOffsetAroundPlayer(GameObject self, Transform player)
    {
        List<GameObject> allEnemies = new List<GameObject>();
        foreach(var enemy in EnemyManager.spawnedEnemies)
        {
            if(Vector3.Distance(enemy.transform.position, player.position) < 6f)
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
