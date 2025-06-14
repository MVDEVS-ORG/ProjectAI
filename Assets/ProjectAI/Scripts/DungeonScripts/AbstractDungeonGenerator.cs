using System.Collections;
using UnityEngine;

namespace Assets.DungenGame.Scripts.NewAlgo
{
    public abstract class AbstractDungeonGenerator: MonoBehaviour
    {
        [SerializeField]
        protected TilemapVisualizer tilemapVisualizer = null;
        [SerializeField]
        protected Vector2Int startPosition = Vector2Int.zero;

        public void GenerateDungeon()
        {
            tilemapVisualizer.Clear();
            RunProceduralGeneration();
        }

        protected abstract void RunProceduralGeneration();
    }
}