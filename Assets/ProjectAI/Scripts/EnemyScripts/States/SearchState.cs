using Assets.ProjectAI.Scripts.PathFinding;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class SearchState : IEnemyState
{
    private EnemyAI _enemy;
    private Transform _player;
    private float searchTime = 3f;
    private float timer;
    private Vector3 lastKnownPos;
    public void Enter(EnemyAI enemy, Transform player, ObjectPoolManager op)
    {
        _enemy = enemy;
        _player = player;
        lastKnownPos = _player.position;
        var startPos = PathFindingManager.Instance.floorTilemap.WorldToCell(_enemy.transform.position);
        var targetPositon = PathFindingManager.Instance.floorTilemap.WorldToCell(lastKnownPos);
        List<Vector3Int> path = PathFindingManager.Instance.FindPath(startPos, targetPositon);
        enemy.StartPathMovement(path);
        timer = 0f;
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (_enemy.IsPlayerVisible())
        {
            _enemy.TransitionToState(_enemy.GetNextStateFromMap(EnemyStateTypes.Chase));
            return;
        }

        if (timer > searchTime)
        {
            _enemy.TransitionToState(_enemy.GetNextStateFromMap(EnemyStateTypes.Idle));
        }
    }

    public void Exit() 
    {
        _enemy = null;
        _player = null;
        lastKnownPos = Vector2.zero;
        timer = 0f;
    }
}
