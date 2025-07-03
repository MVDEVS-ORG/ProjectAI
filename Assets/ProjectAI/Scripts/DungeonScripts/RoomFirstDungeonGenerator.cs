using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.ProjectAI.Scripts.DungeonScripts.Interfaces;
using System.Threading.Tasks;
namespace Assets.ProjectAI.Scripts.DungeonScripts
{
    public class RoomFirstDungeonGenerator : SimpleRandomWalkDungeonGenerator, IDungeonGenerator
    {
        [SerializeField]
        private int minRoomWidth = 4, minRoomHeight = 4;
        [SerializeField]
        private int dungeonWidth = 20, dungeonHeight = 20;
        [SerializeField]
        [Range(0, 10)]
        private int offset = 1;
        [SerializeField]
        private bool randomWalkRooms = false;
        [SerializeField]
        private RoomContentGenerator roomContentGenerator;

        // PCG Data
        private Dictionary<Vector2Int, HashSet<Vector2Int>> _roomsDictionary = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
        private HashSet<Vector2Int> _floorPositions = new HashSet<Vector2Int>();
        private HashSet<Vector2Int> _corridorPositions = new HashSet<Vector2Int>();
        private HashSet<Vector2Int> _doorPositions = new HashSet<Vector2Int>();
        // GizmosData
        private List<Color> roomColors = new List<Color>();
        [SerializeField]
        private bool showRoomGizmo = false, showCorridorsPositions;

        protected override async Awaitable<DungeonData> RunProceduralGeneration()
        {
            ClearRoomData();
            await CreateRooms();

            DungeonData data = new DungeonData
            {
                roomsDictionary = this._roomsDictionary,
                corridorPositions = this._corridorPositions,
                floorPositions = this._floorPositions,
                doorPositions = this._doorPositions
            };
            return data;
            //await roomContentGenerator.GenerateRoomContent(data);
        }
        private void DetectDoorPositions()
        {
            _doorPositions.Clear();

            // 1️⃣ Flatten all room tiles
            var cleanRoomTiles = new HashSet<Vector2Int>();
            foreach (var room in _roomsDictionary.Values)
                cleanRoomTiles.UnionWith(room);

            // 2️⃣ Copy corridors, strip overlaps
            var cleanCorridorTiles = new HashSet<Vector2Int>(_corridorPositions);
            cleanCorridorTiles.ExceptWith(cleanRoomTiles);
            cleanRoomTiles.ExceptWith(_corridorPositions);

            // 3️⃣ For each corridor tile, look for exactly one room neighbor
            //    *and* ensure the corridor extends on the opposite side
            foreach (var corridorTile in cleanCorridorTiles)
            {
                foreach (var dir in Direction2D.cardinalDirectionList)
                {
                    var roomNeighbor = corridorTile + dir;
                    if (!cleanRoomTiles.Contains(roomNeighbor))
                        continue;    // not the room‐facing side

                    var opposite = corridorTile - dir;
                    // must be corridor on the other side
                    if (!cleanCorridorTiles.Contains(opposite))
                        break;       // if it's not a corridor continuation, it isn't an entrance

                    //  this tile is the one and only door spot
                    _doorPositions.Add(corridorTile);
                    goto NextTile;  // break out of both loops
                }
            NextTile:
                ;
            }
        }

        private void OnDrawGizmos()
        {
            if (_doorPositions == null) return;
            Gizmos.color = Color.cyan;
            foreach (var pos in _doorPositions)
            {
                Gizmos.DrawSphere(new Vector3(pos.x + 0.5f, pos.y + 0.5f, 0), 0.2f);
            }
        }

        private async Awaitable CreateRooms()
        {
            var roomsList = ProceduralGenerationAlgorithms.BinarySpacePartitioning(
                new BoundsInt((Vector3Int)startPosition, new Vector3Int(dungeonWidth, dungeonHeight, 0)),
                minRoomWidth, minRoomHeight);

            _floorPositions.Clear();

            if (randomWalkRooms)
            {
                CreateRoomsRandomly(roomsList);
            }
            else
            {
                CreateSimpleRooms(roomsList);
            }

            List<Vector2Int> roomCenters = new List<Vector2Int>();
            foreach (var room in roomsList)
            {
                roomCenters.Add((Vector2Int)(Vector3Int.RoundToInt(room.center)));
            }

            HashSet<Vector2Int> corridors = ConnectRooms(roomCenters);
            _floorPositions.UnionWith(corridors);

            tilemapVisualizer.PaintFloorTiles(_floorPositions);
            await WallGenerator.CreateWalls(_floorPositions, tilemapVisualizer);
            DetectDoorPositions();
        }

        private void CreateRoomsRandomly(List<BoundsInt> roomsList)
        {
            foreach (var roomBounds in roomsList)
            {
                Vector2Int roomCenter = new Vector2Int(
                    Mathf.RoundToInt(roomBounds.center.x),
                    Mathf.RoundToInt(roomBounds.center.y)
                );

                HashSet<Vector2Int> roomFloor = RunRandomWalk(randomWalkParameters, roomCenter);

                HashSet<Vector2Int> boundedRoomFloor = new HashSet<Vector2Int>();
                foreach (var position in roomFloor)
                {
                    if (position.x >= (roomBounds.xMin + offset) && position.x <= (roomBounds.xMax - offset) &&
                        position.y >= (roomBounds.yMin + offset) && position.y <= (roomBounds.yMax - offset))
                    {
                        boundedRoomFloor.Add(position);
                        _floorPositions.Add(position);
                    }
                }

                SaveRoomData(roomCenter, boundedRoomFloor);
            }
        }

        private void CreateSimpleRooms(List<BoundsInt> roomsList)
        {
            foreach (var roomBounds in roomsList)
            {
                Vector2Int roomCenter = new Vector2Int(
                    Mathf.RoundToInt(roomBounds.center.x),
                    Mathf.RoundToInt(roomBounds.center.y)
                );

                HashSet<Vector2Int> roomFloor = new HashSet<Vector2Int>();

                for (int col = offset; col < roomBounds.size.x - offset; col++)
                {
                    for (int row = offset; row < roomBounds.size.y - offset; row++)
                    {
                        Vector2Int position = (Vector2Int)roomBounds.min + new Vector2Int(col, row);
                        roomFloor.Add(position);
                        _floorPositions.Add(position);
                    }
                }

                SaveRoomData(roomCenter, roomFloor);
            }
        }

        private HashSet<Vector2Int> ConnectRooms(List<Vector2Int> roomCenters)
        {
            HashSet<Vector2Int> corridors = new HashSet<Vector2Int>();
            Vector2Int currentRoomCenter = roomCenters[Random.Range(0, roomCenters.Count)];
            roomCenters.Remove(currentRoomCenter);

            while (roomCenters.Count > 0)
            {
                Vector2Int closest = FindClosestPointTo(currentRoomCenter, roomCenters);
                roomCenters.Remove(closest);

                HashSet<Vector2Int> newCorridor = CreateCorridor(currentRoomCenter, closest);
                corridors.UnionWith(newCorridor);

                currentRoomCenter = closest;
            }

            // Create a working copy of the corridor set
            HashSet<Vector2Int> cleanCorridors = new HashSet<Vector2Int>(corridors);

            foreach (var corridorPos in corridors)
            {
                bool leftRightAreRoom = _floorPositions.Contains(corridorPos + Vector2Int.left) &&
                                        _floorPositions.Contains(corridorPos + Vector2Int.right);

                bool upDownAreRoom = _floorPositions.Contains(corridorPos + Vector2Int.up) &&
                                     _floorPositions.Contains(corridorPos + Vector2Int.down);

                if (leftRightAreRoom || upDownAreRoom)
                {
                    cleanCorridors.Remove(corridorPos); // Remove from corridor

                    // ✅ Add corridorPos to the closest room in _roomsDictionary
                    Vector2Int nearestRoomKey = GetNearestRoomKey(corridorPos);
                    if (_roomsDictionary.ContainsKey(nearestRoomKey))
                    {
                        _roomsDictionary[nearestRoomKey].Add(corridorPos);
                    }
                }
            }

            _corridorPositions = cleanCorridors;
            return cleanCorridors;
        }

        private Vector2Int GetNearestRoomKey(Vector2Int position)
        {
            Vector2Int closest = Vector2Int.zero;
            float minDistance = float.MaxValue;

            foreach (var key in _roomsDictionary.Keys)
            {
                float dist = Vector2.Distance(position, key);
                if (dist < minDistance)
                {
                    minDistance = dist;
                    closest = key;
                }
            }

            return closest;
        }


        private HashSet<Vector2Int> CreateCorridor(Vector2Int start, Vector2Int end)
        {
            HashSet<Vector2Int> corridor = new HashSet<Vector2Int>();
            Vector2Int position = start;
            corridor.Add(position);

            while (position.y != end.y)
            {
                position += (end.y > position.y) ? Vector2Int.up : Vector2Int.down;
                corridor.Add(position);
            }

            while (position.x != end.x)
            {
                position += (end.x > position.x) ? Vector2Int.right : Vector2Int.left;
                corridor.Add(position);
            }

            return corridor;
        }

        private Vector2Int FindClosestPointTo(Vector2Int current, List<Vector2Int> positions)
        {
            Vector2Int closest = Vector2Int.zero;
            float minDistance = float.MaxValue;

            foreach (var position in positions)
            {
                float distance = Vector2.Distance(current, position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = position;
                }
            }

            return closest;
        }

        private void SaveRoomData(Vector2Int center, HashSet<Vector2Int> roomFloor)
        {
            _roomsDictionary[center] = roomFloor;
            roomColors.Add(Random.ColorHSV());
        }

        private void ClearRoomData()
        {
            _roomsDictionary.Clear();
            roomColors.Clear();
            _floorPositions.Clear();
            _corridorPositions.Clear();
        }
    }
}
