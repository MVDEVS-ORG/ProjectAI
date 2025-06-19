using Assets.ProjectAI.Scripts.DungeonScripts;
using Assets.ProjectAI.Scripts.DungeonScripts.Interfaces;
using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Assets.ProjectAI.Scripts.Installers
{
    public class DungeonInstaller : MonoInstaller
    {
        [SerializeField] private DungeonMapController _dungeonMapController;
        [SerializeField] private RoomFirstDungeonGenerator _roomFirstDungeonGenerator;
        [SerializeField] private RoomContentGenerator _roomContentGenerator;
        [SerializeField] private TilemapVisualizer _tilemapVisualizer;
        [SerializeField] private PrefabPlacer _prefabPlacer;

        public override void InstallBindings()
        {
            Container.Bind<RoomFirstDungeonGenerator>().FromInstance(_roomFirstDungeonGenerator).AsSingle();
            Container.Bind<IDungeonGenerator>().To<RoomFirstDungeonGenerator>().FromInstance(_roomFirstDungeonGenerator).AsSingle();

            Container.Bind<DungeonMapController>().FromInstance(_dungeonMapController).AsSingle().NonLazy();
            Container.Bind<RoomContentGenerator>().FromInstance(_roomContentGenerator).AsSingle();
            Container.Bind<TilemapVisualizer>().FromInstance(_tilemapVisualizer).AsSingle();
            Container.Bind<PrefabPlacer>().FromInstance(_prefabPlacer).AsSingle();
        }
    }
}