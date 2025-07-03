using UnityEngine;
using System.Collections.Generic;
using Assets.ProjectAI.Scripts.PathFinding;
using Assets.ProjectAI.Scripts.EnemyScripts;
using System.Linq;

public class ChaseState : IEnemyState
{
    private EnemyAI enemy;
    private float pathRefreshTime = Random.Range(0.5f, 1f);
    private float timer;

    public void Enter(EnemyAI enemy)
    {
        this.enemy = enemy;
        RequestPath();
        timer = 0f;
    }

    public void Update()
    {
        if (!enemy.IsPlayerVisible())
        {
            enemy.TransitionToState(new SearchState());
            return;
        }

        if (enemy.IsPlayerInAttackRange())
        {
            enemy.TransitionToState(new AttackState());
            return;
        }

        timer += Time.deltaTime;
        if (timer > pathRefreshTime)
        {
            RequestPath();
            timer = 0f;
        }
    }

    public void Exit() { }

    private void RequestPath()
    {
        Vector3 targetPos = GetOffsetAroundPlayer(enemy.gameObject, enemy.player);
        Vector3Int start = enemy.floorTilemap.WorldToCell(enemy.transform.position);
        Vector3Int goal = enemy.floorTilemap.WorldToCell(targetPos);
        List<Vector3Int> path = PathFindingManager.Instance.FindPath(start, goal);
        enemy.StartPathMovement(path);
    }
    private Vector3 GetOffsetAroundPlayer(GameObject self, Transform player)
    {
        List<GameObject> allEnemies = EnemyManager.spawnedEnemies
        .Where(e => Vector3.Distance(e.transform.position, player.position) < 6f)
        .ToList();
        int index = allEnemies.IndexOf(self);
        int nearbyEnemyCount = allEnemies.Count;
        float angle = (360f / Mathf.Max(nearbyEnemyCount, 1)) * index;
        float radius = 1.5f; // Distance from the player

        Vector3 offset = new Vector3(
            Mathf.Cos(angle * Mathf.Deg2Rad),
            Mathf.Sin(angle * Mathf.Deg2Rad),
            0
        ) * radius;

        return enemy.player.position + offset;
    }
}
