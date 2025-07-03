using Assets.ProjectAI.Scripts.PathFinding;
using System.Collections.Generic;
using UnityEngine;

public class SearchState : IEnemyState
{
    private EnemyAI _enemy;
    private Transform _player;
    private float searchTime = 3f;
    private float timer;
    private Vector3 lastKnownPos;

    public void Enter(EnemyAI enemy, Transform player)
    {
        _enemy = enemy;
        _player = player;
        lastKnownPos = _player.position;
        Vector3Int start = enemy.floorTilemap.WorldToCell(enemy.transform.position);
        Vector3Int goal = enemy.floorTilemap.WorldToCell(lastKnownPos);
        List<Vector3Int> path = PathFindingManager.Instance.FindPath(start, goal);
        enemy.StartPathMovement(path);
        timer = 0f;
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (_enemy.IsPlayerVisible())
        {
            _enemy.TransitionToState(new ChaseState());
            return;
        }

        if (timer > searchTime)
        {
            _enemy.TransitionToState(new IdleState());
        }
    }

    public void Exit() { }
}
