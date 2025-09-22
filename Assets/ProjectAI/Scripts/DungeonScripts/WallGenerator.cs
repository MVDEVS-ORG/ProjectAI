using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts
{
    public static class WallGenerator
    {
        public static void CreateWalls(HashSet<Vector2Int> floorPositions, TilemapVisualizer tilemapVisualizer, bool isTopWallRequired = true)
        {
            var basicWallPositions = FindWallsInDirections(floorPositions, Direction2D.cardinalDirectionList);
            var cornerWallPositions = FindCornerWallPositions(floorPositions, basicWallPositions);
            CreateBasicWall(tilemapVisualizer, basicWallPositions, floorPositions, isTopWallRequired);
            CreateCornerWalls(tilemapVisualizer, cornerWallPositions, floorPositions, basicWallPositions, isTopWallRequired);
        }

        private static void CreateCornerWalls(
            TilemapVisualizer tilemapVisualizer,
            HashSet<Vector2Int> cornerWallPositions,
            HashSet<Vector2Int> floorPositions,
            HashSet<Vector2Int> basicWallPosition,
            bool isTopWallRequired = true)
        {
            foreach (var position in cornerWallPositions)
            {
                // Skip if this is actually a floor tile
                if (floorPositions.Contains(position))
                    continue;

                // Build 8-direction neighbor mask for bottom tile
                string neighborsBinaryType = "";
                foreach (var direction in Direction2D.eightDirectionList)
                {
                    var neighborPosition = position + direction;
                    neighborsBinaryType += floorPositions.Contains(neighborPosition) ? "1" : "0";
                }

                //Check position is for Top Corner Walls?

                if (isTopWallRequired && !basicWallPosition.Contains(position + Vector2Int.up) && !floorPositions.Contains(position + Vector2Int.up))
                {
                    // This is a TOP corner wall → paint 2 units high
                    tilemapVisualizer.PaintSingleCornerWall(position, neighborsBinaryType);

                    Vector2Int above = position + Vector2Int.up;
                    if (!floorPositions.Contains(above) && !basicWallPosition.Contains(above))
                    {
                        string neighborsAbove = "";
                        foreach (var direction in Direction2D.eightDirectionList)
                        {
                            var neighborPosition = above + direction;
                            neighborsAbove += floorPositions.Contains(neighborPosition) ? "1" : "0";
                        }
                        tilemapVisualizer.PaintSingleCornerWall(above, neighborsAbove);
                        foreach (var direction in Direction2D.eightDirectionList)
                        {
                            var neighborPosition = above + Vector2Int.up + direction;
                            neighborsAbove += floorPositions.Contains(neighborPosition) ? "1" : "0";
                        }
                        tilemapVisualizer.PaintSingleCornerWall(above + Vector2Int.up, neighborsAbove);
                    }

                }
                else
                {
                    // Bottom corner → paint only single tile
                    tilemapVisualizer.PaintSingleCornerWall(position, neighborsBinaryType);
                }
            }
        }


        private static void CreateBasicWall(
         TilemapVisualizer tilemapVisualizer,
         HashSet<Vector2Int> basicWallPositions,
         HashSet<Vector2Int> floorPositions,
         bool isTopWallRequired = true)
        {
            foreach (var position in basicWallPositions)
            {
                string neighborsBinaryType = ""; 
                foreach (var direction in Direction2D.cardinalDirectionList) 
                { 
                    var neighborPosition = position + direction; 
                    if (floorPositions.Contains(neighborPosition)) 
                    { 
                        neighborsBinaryType += "1"; 
                    } 
                    else 
                    { 
                        neighborsBinaryType += "0"; 
                    } 
                } 
                // Check if this is a "top wall"
                bool isTopWall = floorPositions.Contains(position + Vector2Int.down); 
                if (isTopWallRequired && isTopWall) 
                {
                    // Paint first wall at normal position\
                    var above = position + Vector2Int.up;
                    tilemapVisualizer.PaintSingleBasicWall(position, neighborsBinaryType); // Paint extra wall one tile above
                    tilemapVisualizer.PaintSingleBasicWall(above , neighborsBinaryType); 
                    if(!basicWallPositions.Contains(above + Vector2Int.left))
                    {
                        tilemapVisualizer.PaintSingleBasicWall(above + Vector2Int.left, neighborsBinaryType);
                    }
                    if (!basicWallPositions.Contains(above + Vector2Int.right))
                    {
                        tilemapVisualizer.PaintSingleBasicWall(above + Vector2Int.right, neighborsBinaryType);
                    }
                } 
                else 
                { 
                    // Paint normally
                    tilemapVisualizer.PaintSingleBasicWall(position, neighborsBinaryType);
                } 
            }
               
        }

        private static HashSet<Vector2Int> FindWallsInDirections(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
        {
            HashSet<Vector2Int> wallPostions = new HashSet<Vector2Int>();
            foreach (var position in floorPositions)
            {
                foreach (var direction in directionList)
                {
                    var neighnorPosition = position + direction;
                    if (floorPositions.Contains(neighnorPosition) == false)
                    {
                        wallPostions.Add(neighnorPosition);
                    }
                }
            }
            return wallPostions;
            
        }

        private static HashSet<Vector2Int> FindCornerWallPositions(
        HashSet<Vector2Int> floorPositions,
        HashSet<Vector2Int> basicWallPositions)
        {
            HashSet<Vector2Int> cornerWallPositions = new HashSet<Vector2Int>();

            // Treat floor + wall tiles as "solid"
            HashSet<Vector2Int> solidPositions = new HashSet<Vector2Int>(floorPositions);
            solidPositions.UnionWith(basicWallPositions);

            foreach (var pos in floorPositions)
            {
                foreach (var direction in Direction2D.diagonalDirectionList)
                {
                    var neighborPos = pos + direction;

                    // If neighbor is empty but diagonal is touching a floor/wall, add corner
                    if (!solidPositions.Contains(neighborPos))
                    {
                        cornerWallPositions.Add(neighborPos);
                    }
                }
            }
            return cornerWallPositions;
        }

    }
}