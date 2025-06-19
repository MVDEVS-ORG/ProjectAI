using Unity.Cinemachine;
using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items;
using Assets.ProjectAI.Scripts.DungeonScripts;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private CinemachineCamera cam;
    [SerializeField] private DungeonMapController _dungeonMapController;
    [SerializeField] private RoomFirstDungeonGenerator _roomFirstDungeonGenerator;
    [SerializeField] private RoomContentGenerator _roomContentGenerator;
    [SerializeField] private TilemapVisualizer _tilemapVisualizer;
    [SerializeField] private PrefabPlacer _prefabPlacer;
    public override void InstallBindings()
    {
        Container.Bind<PlayerPicker>().AsSingle();
        Container.Bind<IPlayerController>().To<PlayerController>().AsCached().OnInstantiated(PlayerCameraSetup);

        Container.Bind<TilemapVisualizer>().FromInstance(_tilemapVisualizer).AsSingle();
        Container.Inject(_tilemapVisualizer);
        Container.Bind<RoomContentGenerator>().FromInstance(_roomContentGenerator).AsSingle();
        Container.Bind<PrefabPlacer>().FromInstance(_prefabPlacer).AsSingle();
        Container.BindInterfacesAndSelfTo<RoomFirstDungeonGenerator>()
            .FromInstance(_roomFirstDungeonGenerator)
            .AsSingle();
        Container.Bind<DungeonMapController>().FromInstance(_dungeonMapController).AsSingle();
        Container.Bind<IGameController>().To<GameController>().AsCached().NonLazy();
    }

    private void PlayerCameraSetup(InjectContext context,object playerController)
    {
        (playerController as PlayerController).SetCam(cam);
    }

}
