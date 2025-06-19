using Assets.DungenGame.Scripts.NewAlgo.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.ProjectAI.Scripts.DungeonScripts
{
    public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
    {
        [SerializeField]
        protected SimpleRandomWalkSO randomWalkParameters;

        protected override async Awaitable RunProceduralGeneration()
        {
            HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters, startPosition);
            tilemapVisualizer.Clear();
            tilemapVisualizer.PaintFloorTiles(floorPositions);
            await WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
        }

        protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkSO parameters, Vector2Int position)
        {
            var currentPosition = position;
            HashSet<Vector2Int>floorPositions = new HashSet<Vector2Int>();

            for (int i = 0; i < parameters.iterations; i++)
            {
                var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currentPosition, parameters.walkLength);
                floorPositions.UnionWith(path);
                if (parameters.startRandomlyEachIteration)
                {
                    currentPosition = floorPositions.ElementAt(Random.Range(0, floorPositions.Count));
                }
            }
            return floorPositions;
        }
    }
}