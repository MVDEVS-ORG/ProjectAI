using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts
{
    [Serializable]
    public class DungeonData
    {
        public Dictionary<Vector2Int, HashSet<Vector2Int>> roomsDictionary;
        public HashSet<Vector2Int> floorPositions;
        public HashSet<Vector2Int> corridorPositions;

        public HashSet<Vector2Int> GetRoomFloorwithoutCorridors(Vector2Int dictionaryKey)
        {
            HashSet<Vector2Int> roomFloorNoCorridors = new HashSet<Vector2Int>(roomsDictionary[dictionaryKey]);
            roomFloorNoCorridors.ExceptWith(corridorPositions);
            return roomFloorNoCorridors;
        }
    }

    [System.Serializable]
    public class DungeonDataSerializable
    {
        public List<RoomEntry> rooms = new List<RoomEntry>();
        public List<Vector2Int> floorPositions = new List<Vector2Int>();
        public List<Vector2Int> corridorPositions = new List<Vector2Int>();

        public DungeonDataSerializable(DungeonData data)
        {
            foreach (var kvp in data.roomsDictionary)
            {
                rooms.Add(new RoomEntry
                {
                    center = kvp.Key,
                    tiles = new List<Vector2Int>(kvp.Value)
                });
            }

            floorPositions = new List<Vector2Int>(data.floorPositions);
            corridorPositions = new List<Vector2Int>(data.corridorPositions);
        }

        [System.Serializable]
        public class RoomEntry
        {
            public Vector2Int center;
            public List<Vector2Int> tiles;
        }
    }
}