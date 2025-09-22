using System.Collections.Generic;
using UnityEngine;
using Assets.ProjectAI.Scripts.DungeonScripts;
using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items;
using Zenject;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Assets.ProjectAI.Scripts.PathFinding
{
    public class PathFindingManager : MonoBehaviour
    {
        public static PathFindingManager Instance { get; private set; }

        [Header("Debug Grid (Editor Only)")]
        public bool debugDrawGrid = true;
        public Color walkableColor = new Color(0, 1, 0, 0.3f);
        public Color unwalkableColor = new Color(1, 0, 0, 0.3f);

        private PathNode[,] nodes;
        private bool[,] baseWalkable;
        private HashSet<Vector2Int> blockedByItems = new();

        private int width, height, offsetX, offsetY;
        private bool _initialBaked = false;
        private bool _itemsBaked = false;
        private List<Vector2Int> _walkablePositions = new();

        /// <summary>
        /// True once both static and item bakes have been performed successfully.
        /// </summary>
        public bool IsMapBaked => _initialBaked && _itemsBaked;

        [Inject]
        public void Initialize()
        {
            if (Instance == null)
                Instance = this;
            else
            {
                Destroy(Instance);
                Instance = this;
            }
        }

        /// <summary>
        /// Bake navigation grid using DungeonData (rooms + corridors).
        /// </summary>
        public void InitialBake(DungeonData data)
        {
            // Combine floor + corridor positions
            HashSet<Vector2Int> walkableTiles = new HashSet<Vector2Int>();
            walkableTiles.UnionWith(data.floorPositions);
            walkableTiles.UnionWith(data.corridorPositions);

            // Determine grid bounds dynamically
            int minX = int.MaxValue, minY = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue;

            foreach (var pos in walkableTiles)
            {
                if (pos.x < minX) minX = pos.x;
                if (pos.y < minY) minY = pos.y;
                if (pos.x > maxX) maxX = pos.x;
                if (pos.y > maxY) maxY = pos.y;
            }

            // Safety: if walkableTiles empty, create 1x1 around (0,0)
            if (walkableTiles.Count == 0)
            {
                minX = minY = 0;
                maxX = maxY = 0;
            }

            width = maxX - minX + 1;
            height = maxY - minY + 1;
            offsetX = minX;
            offsetY = minY;

            nodes = new PathNode[width, height];
            baseWalkable = new bool[width, height];
            blockedByItems.Clear();

            _walkablePositions.Clear();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2Int pos = new Vector2Int(x + offsetX, y + offsetY);
                    bool canWalk = walkableTiles.Contains(pos);

                    baseWalkable[x, y] = canWalk;
                    nodes[x, y] = new PathNode
                    {
                        position = new Vector3Int(pos.x, pos.y, 0),
                        walkable = canWalk
                    };
                    if (baseWalkable[x, y])
                        _walkablePositions.Add(new Vector2Int(x + offsetX, y + offsetY));
                }
            }

            _initialBaked = true;

#if UNITY_EDITOR
            if (debugDrawGrid) SceneView.RepaintAll();
#endif
        }

        public async Awaitable<bool> InitialBakeAsync(DungeonData data)
        {
            try
            {
                _initialBaked = false;
                InitialBake(data);
                await Awaitable.NextFrameAsync();
                _itemsBaked = false; // Require items bake afterwards
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"InitialBakeAsync failed: {e}");
                return false;
            }
        }

        /// <summary>
        /// Apply all current items as blocked areas.
        /// </summary>
        public void BakeItems(DungeonData data)
        {
            blockedByItems.Clear();
            _itemsBaked = false;

            if (data.items != null)
                foreach (var item in data.items)
                    BlockItemArea(item);

            _itemsBaked = true;
        }

        public async Awaitable<bool> BakeItemsAsync(DungeonData data)
        {
            try
            {
                BakeItems(data);
                await Awaitable.NextFrameAsync();
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"BakeItemsAsync failed: {e}");
                return false;
            }
        }

        /// <summary>
        /// Block tiles under an item based on its itemSize and grid offset.
        /// </summary>
        public void BlockItemArea(Item item)
        {
            if (item == null) return;

            Vector2Int center = Vector2Int.RoundToInt(item.transform.position);
            Vector2Int size = item.itemSize;

            int halfWidth = Mathf.FloorToInt((size.x - 1) / 2f);
            int halfHeight = Mathf.FloorToInt((size.y - 1) / 2f);

            Vector2Int bottomLeft = new Vector2Int(center.x - halfWidth, center.y - halfHeight);

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2Int pos = bottomLeft + new Vector2Int(x, y);
                    blockedByItems.Add(pos);

                    int ix = pos.x - offsetX;
                    int iy = pos.y - offsetY;

                    if (IsInBounds(ix, iy) && nodes[ix, iy] != null)
                        nodes[ix, iy].walkable = false;
                }
            }

#if UNITY_EDITOR
            if (debugDrawGrid) SceneView.RepaintAll();
#endif
        }

        /// <summary>
        /// Unblock tiles under a destroyed item.
        /// </summary>
        public void UnblockItemArea(Item item)
        {
            if (item == null) return;

            Vector2Int center = Vector2Int.RoundToInt(item.transform.position);
            Vector2Int size = item.itemSize;

            int halfWidth = Mathf.FloorToInt((size.x - 1) / 2f);
            int halfHeight = Mathf.FloorToInt((size.y - 1) / 2f);

            Vector2Int bottomLeft = new Vector2Int(center.x - halfWidth, center.y - halfHeight);

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector2Int pos = bottomLeft + new Vector2Int(x, y);
                    blockedByItems.Remove(pos);

                    int ix = pos.x - offsetX;
                    int iy = pos.y - offsetY;

                    if (IsInBounds(ix, iy) && nodes[ix, iy] != null)
                        nodes[ix, iy].walkable = baseWalkable[ix, iy];
                }
            }

#if UNITY_EDITOR
            if (debugDrawGrid) SceneView.RepaintAll();
#endif
        }

        public Vector2Int GetNearestValidWalkableTile(Vector2Int startCell)
        {
            // If already walkable, return it
            if (IsWalkable(startCell)) return startCell;

            // Breadth-first search to find nearest walkable
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
            queue.Enqueue(startCell);
            visited.Add(startCell);

            List<Vector2Int> directions = new List<Vector2Int>
            {
                Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
            };

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();

                foreach (var dir in directions)
                {
                    Vector2Int next = current + dir;
                    if (visited.Contains(next)) continue;
                    visited.Add(next);

                    if (IsWalkable(next)) return next;
                    queue.Enqueue(next);
                }
            }

            Debug.LogWarning($"No walkable tile found near {startCell}");
            return startCell; // fallback
        }

        public bool IsWalkable(Vector2Int cell)
        {
            int localX = cell.x - offsetX;
            int localY = cell.y - offsetY;
            if (localX < 0 || localY < 0 || localX >= width || localY >= height)
                return false;
            return baseWalkable[localX, localY];
        }

        /// <summary>
        /// Convert a world position (Vector3) to the nearest grid cell (Vector3Int), clamped to the baked grid.
        /// </summary>
        public Vector3Int WorldToCell(Vector3 worldPos)
        {
            int cellX = Mathf.RoundToInt(worldPos.x);
            int cellY = Mathf.RoundToInt(worldPos.y);

            int localX = Mathf.Clamp(cellX - offsetX, 0, Mathf.Max(0, width - 1));
            int localY = Mathf.Clamp(cellY - offsetY, 0, Mathf.Max(0, height - 1));

            return new Vector3Int(localX + offsetX, localY + offsetY, 0);
        }

        /// <summary>
        /// Overload to accept a Vector3Int (already integer world coords) and clamp to grid.
        /// Useful if you already have a cell position from GetRandomWalkableTile.
        /// </summary>
        public Vector3Int WorldToCell(Vector3Int worldCell)
        {
            int localX = Mathf.Clamp(worldCell.x - offsetX, 0, Mathf.Max(0, width - 1));
            int localY = Mathf.Clamp(worldCell.y - offsetY, 0, Mathf.Max(0, height - 1));
            return new Vector3Int(localX + offsetX, localY + offsetY, 0);
        }

        /// <summary>
        /// Convert a cell coordinate back to world center (center of tile).
        /// </summary>
        public Vector3 CellToWorld(Vector3Int cell)
        {
            return new Vector3(cell.x + 0.5f, cell.y + 0.5f, 0f);
        }

        public Vector3Int GetRandomWalkableTileNear(Vector3Int center, int radius)
        {
            // Optional helper if you want nearby randoms; not required for now.
            var tilePos = GetNearestValidWalkableTile(new Vector2Int(center.x, center.y));
            Vector3Int ranTile = new Vector3Int(tilePos.x, tilePos.y, 0);
            return ranTile;
        }

        /// <summary>
        /// Returns a random walkable tile from the baked grid.
        /// </summary>
        public Vector2Int GetRandomWalkableTile()
        {
            if (_walkablePositions.Count == 0)
            {
                Debug.LogError("No walkable positions found in dungeon!");
                return Vector2Int.zero;
            }

            int index = UnityEngine.Random.Range(0, _walkablePositions.Count);
            return _walkablePositions[index];
        }

        public List<Vector3Int> FindPath(Vector3Int startCell, Vector3Int targetCell)
        {
            if (nodes == null) return null;

            Vector2Int start = (Vector2Int)startCell;
            Vector2Int target = (Vector2Int)targetCell;

            int sx = start.x - offsetX;
            int sy = start.y - offsetY;
            int tx = target.x - offsetX;
            int ty = target.y - offsetY;

            if (!IsInBounds(sx, sy) || !IsInBounds(tx, ty))
                return null;

            PathNode startNode = nodes[sx, sy];
            PathNode endNode = nodes[tx, ty];

            if (startNode == null || endNode == null || !startNode.walkable || !endNode.walkable)
                return null;

            if (!baseWalkable[sx, sy] || !baseWalkable[tx, ty])
                return null;

            if (blockedByItems.Contains(start) || blockedByItems.Contains(target))
                return null;

            foreach (var node in nodes)
                node?.Reset();

            startNode.gCost = 0;
            startNode.hCost = Heuristic(startNode.position, endNode.position);

            var openSet = new SimplePriorityQueue<PathNode>();
            openSet.Enqueue(startNode, startNode.fCost);

            while (openSet.Count > 0)
            {
                PathNode current = openSet.Dequeue();
                current.closed = true;

                if (current == endNode)
                    return ReconstructPath(startNode, endNode);

                foreach (var dir in Direction2D.eightDirectionList)
                {
                    int nx = current.position.x - offsetX + dir.x;
                    int ny = current.position.y - offsetY + dir.y;
                    if (!IsInBounds(nx, ny)) continue;

                    PathNode neighbor = nodes[nx, ny];
                    if (neighbor == null || neighbor.closed || !neighbor.walkable)
                        continue;

                    // Prevent corner cutting
                    if (dir.x != 0 && dir.y != 0)
                    {
                        Vector2Int adj1 = new(current.position.x + dir.x, current.position.y);
                        Vector2Int adj2 = new(current.position.x, current.position.y + dir.y);

                        int a1x = adj1.x - offsetX;
                        int a1y = adj1.y - offsetY;
                        int a2x = adj2.x - offsetX;
                        int a2y = adj2.y - offsetY;

                        if (!IsInBounds(a1x, a1y) || !IsInBounds(a2x, a2y)) continue;
                        if (!baseWalkable[a1x, a1y] || !baseWalkable[a2x, a2y]) continue;
                        if (blockedByItems.Contains(adj1) || blockedByItems.Contains(adj2)) continue;
                    }

                    int newCost = current.gCost + ((dir.x == 0 || dir.y == 0) ? 10 : 14);
                    if (newCost < neighbor.gCost)
                    {
                        neighbor.gCost = newCost;
                        neighbor.hCost = Heuristic(neighbor.position, endNode.position);
                        neighbor.parent = current;
                        openSet.Enqueue(neighbor, neighbor.fCost);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the world position of the center of a given cell (grid coordinate).
        /// </summary>
        public Vector3 GetCellCenterWorld(Vector3Int cell)
        {
            return new Vector3(cell.x + 0.5f, cell.y + 0.5f, 0f);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!debugDrawGrid || nodes == null) return;
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    var node = nodes[x, y];
                    if (node == null) continue;

                    Vector2Int coord = new Vector2Int(node.position.x, node.position.y);
                    bool showWalkable = baseWalkable[x, y] && !blockedByItems.Contains(coord);

                    Vector3 worldPos = new Vector3(node.position.x + 0.5f, node.position.y + 0.5f, 0);
                    Gizmos.color = showWalkable ? walkableColor : unwalkableColor;
                    Gizmos.DrawCube(worldPos, Vector3.one * 0.9f);
                }

        }
#endif

        private int Heuristic(Vector3Int a, Vector3Int b)
        {
            int dx = Mathf.Abs(a.x - b.x);
            int dy = Mathf.Abs(a.y - b.y);
            return 14 * Mathf.Min(dx, dy) + 10 * Mathf.Abs(dx - dy);
        }

        private List<Vector3Int> ReconstructPath(PathNode startNode, PathNode endNode)
        {
            List<Vector3Int> path = new List<Vector3Int>();
            PathNode current = endNode;
            while (current != startNode)
            {
                path.Add(current.position);
                current = current.parent;
            }
            path.Reverse();
            return path;
        }

        private bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
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
}
