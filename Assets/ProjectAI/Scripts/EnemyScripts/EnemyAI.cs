using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using System.Collections;
using Assets.ProjectAI.Scripts.PathFinding;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public Tilemap floorTilemap;
    public float moveSpeed = 2f;
    public float attackRange = 1.5f;
    public float detectionRange = 6f;

    private IEnemyState currentState;
    private Coroutine moveRoutine;

    [HideInInspector] public List<Vector3Int> currentPath;
    [HideInInspector] public int currentPathIndex;

    void Start()
    {
        StartCoroutine(WaitForBakeAndStart());
    }

    IEnumerator WaitForBakeAndStart()
    {
        while (!PathFindingManager.Instance.isBaked)
            yield return null;
        floorTilemap = PathFindingManager.Instance.floorTilemap;
        TransitionToState(new IdleState());
    }

    void Update()
    {
        currentState?.Update();
    }

    public void TransitionToState(IEnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter(this);
    }

    public void StartPathMovement(List<Vector3Int> path)
    {
        if (moveRoutine != null) StopCoroutine(moveRoutine);
        currentPath = path;
        currentPathIndex = 0;
        if (path != null && path.Count > 0)
            moveRoutine = StartCoroutine(FollowPath());
    }

    private IEnumerator<WaitForEndOfFrame> FollowPath()
    {
        while (currentPathIndex < currentPath.Count)
        {
            Vector3 targetPos = floorTilemap.GetCellCenterWorld(currentPath[currentPathIndex]);
            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            currentPathIndex++;
        }
    }

    public bool IsPlayerVisible()
    {
        if (player == null)
            return false;
        return Vector3.Distance(transform.position, player.position) < detectionRange;
    }

    public bool IsPlayerInAttackRange()
    {
        return Vector3.Distance(transform.position, player.position) <= attackRange;
    }
    public Vector3Int GetRandomWalkableTile()
    {
        int attempts = 0;
        while (attempts < 100)
        {
            int x = Random.Range(0, floorTilemap.cellBounds.size.x);
            int y = Random.Range(0, floorTilemap.cellBounds.size.y);
            Vector3Int cell = new Vector3Int(x + floorTilemap.cellBounds.xMin, y + floorTilemap.cellBounds.yMin, 0);

            if (floorTilemap.HasTile(cell) && !PathFindingManager.Instance.wallTileMap.HasTile(cell))
            {
                return cell;
            }

            attempts++;
        }

        return floorTilemap.WorldToCell(transform.position); // fallback
    }

    public void StopMovement()
    {
        if (moveRoutine != null)
        {
            StopCoroutine(moveRoutine);
            moveRoutine = null;
        }
    }

/*#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (currentPath == null || currentPath.Count == 0 || PathFindingManager.Instance == null)
            return;

        Tilemap floor = PathFindingManager.Instance.floorTilemap;

        //  Green Dot: Current Position
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.1f);

        //  Red Dot: Final Destination
        Vector3 finalPos = floor.GetCellCenterWorld(currentPath[^1]);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(finalPos, 0.15f);

        //  Blue Line: Path to be followed
        Gizmos.color = Color.blue;
        for (int i = 0; i < currentPath.Count - 1; i++)
        {
            Vector3 from = floor.GetCellCenterWorld(currentPath[i]);
            Vector3 to = floor.GetCellCenterWorld(currentPath[i + 1]);
            Gizmos.DrawLine(from, to);
        }

        //  Optional: Detection Range (Scene Only)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        //  Optional: Attack Range
        Gizmos.color = new Color(1f, 0.3f, 0f); // Orange
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
#endif*/
}
