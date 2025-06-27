using System.Collections;
using UnityEngine;

namespace Assets.ProjectAI.Scripts.DungeonScripts
{
    public abstract class AbstractDungeonGenerator: MonoBehaviour
    {
        [SerializeField]
        protected TilemapVisualizer tilemapVisualizer = null;
        [SerializeField]
        protected Vector2Int startPosition = Vector2Int.zero;

        public async Awaitable<DungeonData> GenerateDungeon()
        {
            tilemapVisualizer.Clear();
            return await RunProceduralGeneration();
        }

        protected abstract Awaitable<DungeonData> RunProceduralGeneration();
    }
}