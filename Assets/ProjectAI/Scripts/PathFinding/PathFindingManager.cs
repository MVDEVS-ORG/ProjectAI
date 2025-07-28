using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Assets.ProjectAI.Scripts.DungeonScripts;
using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items;


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
        public Color walkableColor = new Color(0, 1, 0, 0.3f);
        public Color unwalkableColor = new Color(1, 0, 0, 0.3f);

        private PathNode[,] nodes;
        private bool[,] baseWalkable;
        private HashSet<Vector2Int> blockedByItems = new HashSet<Vector2Int>();

        private int width, height, offsetX, offsetY;
        private bool _initialBaked = false;
        private bool _itemsBaked = false;

        /// <summary>
        /// True once both static and item bakes have been performed successfully.
        /// </summary>
        public bool IsMapBaked => _initialBaked && _itemsBaked;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        /// <summary>
        /// Static initial bake: rooms, corridors, walls.
        /// </summary>
        public void InitialBake(DungeonData data)
        {
            BoundsInt bounds = wallTileMap.cellBounds;
            width = bounds.size.x;
            height = bounds.size.y;
            offsetX = bounds.xMin;
            offsetY = bounds.yMin;

            nodes = new PathNode[width, height];
            baseWalkable = new bool[width, height];

            // Combine room and corridor tiles
            HashSet<Vector2Int> walkableTiles = new HashSet<Vector2Int>();
            foreach (var kvp in data.roomsDictionary)
                walkableTiles.UnionWith(kvp.Value);
            walkableTiles.UnionWith(data.corridorPositions);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3Int cell = new Vector3Int(x + offsetX, y + offsetY, 0);
                    Vector2Int c2 = new Vector2Int(cell.x, cell.y);
                    bool canWalk = walkableTiles.Contains(c2) && !wallTileMap.HasTile(cell);
                    baseWalkable[x, y] = canWalk;
                    nodes[x, y] = new PathNode { position = cell, walkable = canWalk };
                }
            }

            _initialBaked = true;

#if UNITY_EDITOR
            if (debugDrawGrid) SceneView.RepaintAll();
#endif
        }

        /// <summary>
        /// Awaitable wrapper around InitialBake.
        /// </summary>
        public async Awaitable<bool> InitialBakeAsync(DungeonData data)
        {
            try
            {
                _initialBaked = false;
                InitialBake(data);
                await Awaitable.NextFrameAsync();
                _itemsBaked = false; // require items bake afterwards
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

        /// <summary>
        /// Awaitable wrapper around BakeItems.
        /// </summary>
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
        /// Block tiles under an item immediately.
        /// </summary>
        public void BlockItemArea(Item item)
        {
            if (item == null) return;

            Vector2Int size = item.itemSize;
            Vector3 itemPos = item.transform.position;

            Vector3Int centerTile = floorTilemap.WorldToCell(itemPos);

            // Correct bottom-left calculation that works for both even and odd sizes
            int halfWidth = Mathf.FloorToInt((size.x - 1) / 2f);
            int halfHeight = Mathf.FloorToInt((size.y - 1) / 2f);

            Vector3Int bottomLeft = new Vector3Int(
                centerTile.x - halfWidth,
                centerTile.y - halfHeight,
                0
            );

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector3Int tile = bottomLeft + new Vector3Int(x, y, 0);
                    Vector2Int pos = new Vector2Int(tile.x, tile.y);
                    blockedByItems.Add(pos);

                    int ix = tile.x - offsetX;
                    int iy = tile.y - offsetY;

                    if (IsInBounds(ix, iy) && nodes[ix, iy] != null)
                        nodes[ix, iy].walkable = false;
                }
            }

#if UNITY_EDITOR
            if (debugDrawGrid) SceneView.RepaintAll();
#endif
        }

        /// <summary>
        /// Unblock tiles under a destroyed item immediately.
        /// </summary>
        public void UnblockItemArea(Item item)
        {
            if (item == null) return;

            Vector2Int size = item.itemSize;
            Vector3 itemPos = item.transform.position;

            Vector3Int centerTile = floorTilemap.WorldToCell(itemPos);

            int halfWidth = Mathf.FloorToInt((size.x - 1) / 2f);
            int halfHeight = Mathf.FloorToInt((size.y - 1) / 2f);

            Vector3Int bottomLeft = new Vector3Int(
                centerTile.x - halfWidth,
                centerTile.y - halfHeight,
                0
            );

            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    Vector3Int tile = bottomLeft + new Vector3Int(x, y, 0);
                    Vector2Int pos = new Vector2Int(tile.x, tile.y);
                    blockedByItems.Remove(pos);

                    int ix = tile.x - offsetX;
                    int iy = tile.y - offsetY;

                    if (IsInBounds(ix, iy) && nodes[ix, iy] != null)
                        nodes[ix, iy].walkable = baseWalkable[ix, iy];
                }
            }

#if UNITY_EDITOR
            if (debugDrawGrid) SceneView.RepaintAll();
#endif
        }

        public Vector2Int GetNearestValidWalkableTile(Vector2Int position)
        {
            int x = position.x - offsetX;
            int y = position.y - offsetY;

            if (IsInBounds(x, y) && baseWalkable[x, y] && nodes[x, y]?.walkable == true && !blockedByItems.Contains(position))
            {
                return position;
            }

            // Spiral outward search in 8 directions
            int maxRadius = Mathf.Max(width, height);
            for (int radius = 1; radius < maxRadius; radius++)
            {
                foreach (var dir in Direction2D.eightDirectionList)
                {
                    Vector2Int checkPos = position + dir * radius;

                    int cx = checkPos.x - offsetX;
                    int cy = checkPos.y - offsetY;

                    if (!IsInBounds(cx, cy)) continue;

                    if (baseWalkable[cx, cy] && nodes[cx, cy]?.walkable == true && !blockedByItems.Contains(checkPos))
                    {
                        return checkPos;
                    }
                }
            }

            // Fallback: return original position if nothing found
            return position;
        }

        public Vector3Int GetRandomWalkableTile()
        {
            int attempts = 0;

            while (attempts < 100)
            {
                int x = Random.Range(0, floorTilemap.cellBounds.size.x);
                int y = Random.Range(0, floorTilemap.cellBounds.size.y);
                Vector3Int cell = new Vector3Int(x + floorTilemap.cellBounds.xMin, y + floorTilemap.cellBounds.yMin, 0);

                int nx = cell.x - offsetX;
                int ny = cell.y - offsetY;

                if (!IsInBounds(nx, ny))
                {
                    attempts++;
                    continue;
                }

                PathNode node = nodes[nx, ny];
                if (node != null && node.walkable && baseWalkable[nx, ny] && !blockedByItems.Contains((Vector2Int)cell))
                {
                    return cell;
                }

                attempts++;
            }

            // Fallback to current position snapped to tile
            return floorTilemap.WorldToCell(transform.position);
        }

        /// <summary>
        /// Find a path using A* from start to target.
        /// </summary>
        public List<Vector3Int> FindPath(Vector3Int startCell, Vector3Int targetCell)
        {
            if (nodes == null) return null;


            if (!floorTilemap.HasTile(startCell) || !floorTilemap.HasTile(targetCell))
                return null;

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
                    return ReconstrucPath(startNode, endNode);

                foreach (var dir in Direction2D.eightDirectionList)
                {
                    int nx = current.position.x - offsetX + dir.x;
                    int ny = current.position.y - offsetY + dir.y;
                    if (!IsInBounds(nx, ny)) continue;

                    PathNode neighbor = nodes[nx, ny];
                    if (neighbor == null || neighbor.closed || !neighbor.walkable)
                        continue;

                    // Diagonal corner-cutting prevention
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

            return null; // No path found
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
                    Vector3 worldPos = wallTileMap.GetCellCenterWorld(node.position);
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

        private List<Vector3Int> ReconstrucPath(PathNode startNode, PathNode endNode)
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