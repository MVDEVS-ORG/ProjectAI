using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.ProjectAI.Scripts.DungeonScripts.Interfaces;
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
        [SerializeField] private bool _isDoorNeeded = true;

        // PCG Data
        private Dictionary<Vector2Int, HashSet<Vector2Int>> _roomsDictionary = new Dictionary<Vector2Int, HashSet<Vector2Int>>();
        private HashSet<Vector2Int> _floorPositions = new HashSet<Vector2Int>();
        private HashSet<Vector2Int> _corridorPositions = new HashSet<Vector2Int>();
        private HashSet<Vector2Int> _doorPositions = new HashSet<Vector2Int>();
        // GizmosData
        private List<Color> roomColors = new List<Color>();
        [SerializeField]
        private bool showCorridorsPositions;
        [SerializeField] private bool _isTopWallRequired;
        protected override async Awaitable<DungeonData> RunProceduralGeneration()
        {
            ClearRoomData();
            await CreateRooms();
            //DetectDoorPositions();

            DungeonData data = new DungeonData
            {
                roomsDictionary = this._roomsDictionary,
                corridorPositions = this._corridorPositions,
                floorPositions = this._floorPositions,
                doorPositions = this._doorPositions,
                isDoorNeeded = this._isDoorNeeded,
            };
            return data;
            //await _roomContentGenerator.GenerateRoomContent(data);
        }
        public DungeonData DetectDoorPositions(DungeonData data)
        {
            if (_isDoorNeeded)
            {
                _doorPositions.Clear();

                var allDirections = Direction2D.eightDirectionList;

                foreach (var tile in data.corridorPositions)
                {
                    CheckDoorPosition(data, allDirections, tile);
                }

                data.doorPositions = new HashSet<Vector2Int>(_doorPositions);
                Debug.Log($"Detected {_doorPositions.Count} door positions (with corner validation).");
            }
            return data;
        }

        private void CheckDoorPosition(DungeonData data, List<Vector2Int> allDirections, Vector2Int tile)
        {
            string neighborBinary = "";
            foreach (var dir in allDirections)
            {
                var neighbor = tile + dir;
                neighborBinary += data.floorPositions.Contains(neighbor) ? "1" : "0";
            }

            switch (neighborBinary)
            {
                // Straight openings (no extended check needed)
                case "11001001": //Top Door
                    _doorPositions.Add(tile);
                    break;
                case "10011100": //Down Door
                    _doorPositions.Add(tile);
                    break;
                case "00100111": //Left Door
                    _doorPositions.Add(tile);
                    break;
                case "01110010": //Right Door
                    _doorPositions.Add(tile);
                    break;

                // Corner openings (extended neighbor check required)
                case "11001000": // UP-RIGHT corner
                    if (IsExtendedNeighborWall(tile, new Vector2Int(0, 1), data))
                        _doorPositions.Add(tile);
                    break;

                case "10011000": // DOWN-RIGHT corner
                    if (IsExtendedNeighborWall(tile, new Vector2Int(0, -1), data))
                        _doorPositions.Add(tile);
                    break;

                case "10001100": // DOWN-LEFT corner
                    if (IsExtendedNeighborWall(tile, new Vector2Int(0, -1), data))
                        _doorPositions.Add(tile);
                    break;

                case "10001001": // UP-LEFT corner
                    if (IsExtendedNeighborWall(tile, new Vector2Int(0, 1), data))
                        _doorPositions.Add(tile);
                    break;
                case "01100010": //Right-Up corner
                    if (IsExtendedNeighborWall(tile, new Vector2Int(1, 0), data))
                        _doorPositions.Add(tile);
                    break;
                case "00100011": //Left-Up corner
                    if (IsExtendedNeighborWall(tile, new Vector2Int(-1, 0), data))
                        _doorPositions.Add(tile);
                    break;
                case "00110010": //Right - Down corner
                    if (IsExtendedNeighborWall(tile, new Vector2Int(1, 0), data))
                        _doorPositions.Add(tile);
                    break;
                case "00100110": //Left - Down corner
                    if (IsExtendedNeighborWall(tile, new Vector2Int(-1, 0), data))
                        _doorPositions.Add(tile);
                    break;

            }
        }

        private bool IsExtendedNeighborWall(Vector2Int tile, Vector2Int dir, DungeonData data)
        {
            var neighborTile = tile + dir;
            int neighborFloorTileCount = 0;
            foreach(var dirs in Direction2D.eightDirectionList)
            {
                if(data.floorPositions.Contains(neighborTile + dirs))
                {
                    neighborFloorTileCount++;
                }
            }
            return (neighborFloorTileCount >= 4);
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
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.gray;
            foreach (var floor in _corridorPositions)
            {
                Gizmos.DrawCube(new Vector3(floor.x + 0.5f, floor.y + 0.5f, 0), Vector3.one);
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
            WallGenerator.CreateWalls(_floorPositions, tilemapVisualizer, _isTopWallRequired);
            tilemapVisualizer.PaintBackgroundTiles(dungeonWidth, dungeonHeight);
            await Awaitable.EndOfFrameAsync();
            //DetectDoorPositions();
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

                    // Add corridorPos to the closest room in _roomsDictionary
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
