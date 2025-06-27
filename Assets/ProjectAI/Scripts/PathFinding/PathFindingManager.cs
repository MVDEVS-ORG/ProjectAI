using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.ProjectAI.Scripts.PathFinding
{
    public class PathFindingManager : MonoBehaviour
    {
        public static PathFindingManager Instance { get; private set; }

        [Header("Assign the Obstacle Tilemap (walls)")]
        public Tilemap wallTileMap;
        [Header("Assign the floor Tilemap (walkable area)")]
        public Tilemap floorTilemap;

        [Header("Debug Grid (Editor Only)")]
        public bool debugDrawGrid = true;
        public Color walkableColor = new Color(0, 1, 0, 0.3f);   // semi-transparent green
        public Color unwalkableColor = new Color(1, 0, 0, 0.3f); // semi-transparent red

        private PathNode[,] nodes;
        private int width, height, offsetX, offsetY;

        public bool isBaked => nodes != null;


        /*public GameObject enemyPrefab;
        public Transform spawnPosition;
        public Transform player;*/

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        public async Awaitable<bool> BakeMap()
        {
            BakeGrid();
            Debug.LogError($"Baking {isBaked}");
            while (!isBaked)
            {
                await Awaitable.EndOfFrameAsync();
                Debug.LogError($"Baking {isBaked}");
            }
            return true;
/*            GameObject enemyGo = Instantiate(enemyPrefab, spawnPosition.position, Quaternion.identity, spawnPosition);
            var enemyAI = enemyGo.GetComponent<EnemyAI>();
            enemyAI.player = player;
            enemyAI.floorTilemap = floorTilemap;*/
        }
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!debugDrawGrid || nodes == null) return;

            Debug.LogError("coloring");
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    PathNode node = nodes[x, y];
                    if (node == null) continue;

                    Vector3 worldPos = wallTileMap.GetCellCenterWorld(node.position);
                    Gizmos.color = node.walkable ? walkableColor : unwalkableColor;
                    Gizmos.DrawCube(worldPos, Vector3.one * 0.9f);
                }
            }
        }
#endif

        void BakeGrid()
        {
            BoundsInt bounds = wallTileMap.cellBounds;
            width = bounds.size.x;
            height = bounds.size.y;
            offsetX = bounds.xMin;
            offsetY = bounds.yMin;

            nodes = new PathNode[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3Int cell = new Vector3Int(x + offsetX, y + offsetY, 0);

                    bool isInsideFloor = floorTilemap.HasTile(cell);
                    bool isBlocked = wallTileMap.HasTile(cell);
                    bool isWalkable = isInsideFloor && !isBlocked;
                    if (!isInsideFloor) continue;
                    nodes[x, y] = new PathNode
                    {
                        position = cell,
                        walkable = isWalkable,
                    };
                }
            }
            Debug.Log("Pathfinding Grid is baked");
        }

        public List<Vector3Int> FindPath(Vector3Int startCell, Vector3Int targetCell)
        {
            if (nodes == null)
            {
                Debug.LogWarning("Pathfinding grid not baked yet.");
                return null;
            }

            int sx = startCell.x - offsetX;
            int sy = startCell.y - offsetY;
            int tx = targetCell.x - offsetX;
            int ty = targetCell.y - offsetY;

            if (!IsInBounds(sx, sy) || !IsInBounds(tx, ty))
            {
                Debug.LogWarning($"Start or target out of bounds. Start: {sx},{sy}, Target: {tx},{ty}");
                return null;
            }

            PathNode startNode = nodes[sx, sy];
            PathNode endNode = nodes[tx, ty];

            if (startNode == null || endNode == null || !startNode.walkable || !endNode.walkable)
            {
                Debug.LogWarning("Start or target node is null or not walkable.");
                return null;
            }

            // 1. Cache original walkability of other enemies' positions
            Dictionary<PathNode, bool> modifiedNodes = new Dictionary<PathNode, bool>();
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (GameObject enemy in enemies)
            {
                if (enemy == null || enemy.transform == null) continue;

                Vector3Int enemyCell = floorTilemap.WorldToCell(enemy.transform.position);
                int ex = enemyCell.x - offsetX;
                int ey = enemyCell.y - offsetY;

                if (IsInBounds(ex, ey))
                {
                    PathNode node = nodes[ex, ey];
                    if (node != null && node.walkable && node != startNode && node != endNode)
                    {
                        modifiedNodes[node] = node.walkable;
                        node.walkable = false;
                    }
                }
            }

            // 2. Reset all nodes
            foreach (var node in nodes)
            {
                node?.Reset();
            }

            startNode.gCost = 0;
            startNode.hCost = Heuristic(startNode.position, endNode.position);

            var openSet = new SimplePriorityQueue<PathNode>();
            openSet.Enqueue(startNode, startNode.fCost);

            while (openSet.Count > 0)
            {
                PathNode current = openSet.Dequeue();
                if (current.closed) continue;
                current.closed = true;

                if (current == endNode)
                {
                    RestoreWalkableNodes(modifiedNodes); // <- restore before return
                    return ReconstrucPath(startNode, endNode);
                }

                foreach (var direction in Direction2D.eightDirectionList)
                {
                    int nx = current.position.x - offsetX + direction.x;
                    int ny = current.position.y - offsetY + direction.y;

                    if (!IsInBounds(nx, ny)) continue;

                    PathNode neighbor = nodes[nx, ny];
                    if (neighbor == null || !neighbor.walkable || neighbor.closed) continue;

                    // Diagonal check
                    if (Mathf.Abs(direction.x) == 1 && Mathf.Abs(direction.y) == 1)
                    {
                        Vector3Int horizontal = new Vector3Int(current.position.x + direction.x, current.position.y, 0);
                        Vector3Int vertical = new Vector3Int(current.position.x, current.position.y + direction.y, 0);

                        if (!floorTilemap.HasTile(horizontal) || !floorTilemap.HasTile(vertical))
                        {
                            continue;
                        }
                    }

                    int moveCost = current.gCost + ((direction.x == 0 || direction.y == 0) ? 10 : 14);
                    if (moveCost < neighbor.gCost)
                    {
                        neighbor.gCost = moveCost;
                        neighbor.hCost = Heuristic(neighbor.position, endNode.position);
                        neighbor.parent = current;
                        openSet.Enqueue(neighbor, neighbor.fCost);
                    }
                }
            }

            Debug.LogWarning("No path found.");
            RestoreWalkableNodes(modifiedNodes);
            return null;
        }

        private void RestoreWalkableNodes(Dictionary<PathNode, bool> modifiedNodes)
        {
            foreach (var kvp in modifiedNodes)
            {
                kvp.Key.walkable = kvp.Value;
            }
        }

        private List<Vector3Int> ReconstrucPath(PathNode startnode, PathNode endNode)
        {
            List<Vector3Int> path = new();
            PathNode current = endNode;

            while (current != startnode)
            {
                path.Add(current.position);
                current = current.parent;
            }
            path.Reverse();
            return path;
        }

        private int Heuristic(Vector3Int a, Vector3Int b)
        {
            int dx = Mathf.Abs(a.x - b.x);
            int dy = Mathf.Abs(a.y - b.y);
            return 14 * Mathf.Min(dx, dy) + 10 * Mathf.Abs(dx - dy);
        }

        private bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }
    }
}
public class SimplePriorityQueue<T>
{
    private List<(T item, int priority)> elements = new();

    public int Count => elements.Count;

    public void Enqueue(T item, int priority)
    {
        elements.Add((item, priority));
        int c = elements.Count - 1;

        while (c > 0)
        {
            int parent = (c - 1) / 2;
            if (elements[c].priority >= elements[parent].priority) break;

            (elements[c], elements[parent]) = (elements[parent], elements[c]);
            c = parent;
        }
    }

    public T Dequeue()
    {
        int last = elements.Count - 1;
        T item = elements[0].item;
        elements[0] = elements[last];
        elements.RemoveAt(last);
        Heapify(0);
        return item;
    }

    private void Heapify(int i)
    {
        int smallest = i;
        int left = 2 * i + 1;
        int right = 2 * i + 2;

        if (left < elements.Count && elements[left].priority < elements[smallest].priority)
            smallest = left;

        if (right < elements.Count && elements[right].priority < elements[smallest].priority)
            smallest = right;

        if (smallest != i)
        {
            (elements[i], elements[smallest]) = (elements[smallest], elements[i]);
            Heapify(smallest);
        }
    }
}