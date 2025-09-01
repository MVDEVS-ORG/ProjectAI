using Unity.Cinemachine;
using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items;
using Assets.ProjectAI.Scripts.DungeonScripts;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private CinemachineCamera _cam;
    [SerializeField] private DungeonMapController _dungeonMapController;
    [SerializeField] private RoomFirstDungeonGenerator _roomFirstDungeonGenerator;
    [SerializeField] private RoomContentGenerator _roomContentGenerator;
    [SerializeField] private TilemapVisualizer _tilemapVisualizer;
    [SerializeField] private GameObject _prefabPlacer;
    public override void InstallBindings()
    {
        Container.Bind<IGamePauseController>().To<GamePauseController>().AsSingle().NonLazy();
        Container.Bind<GamepadRumble>().AsSingle();
        Container.Bind<CameraController>().AsSingle().OnInstantiated(PlayerCameraSetup);
        Container.Bind<IUpgradeController>().To<UpgradeController>().AsSingle();
        Container.Bind<IGunsController>().To<GunsController>().AsSingle();
        Container.Bind<IMeleeWeaponController>().To<MeleeWeaponController>().AsSingle();
        Container.Bind<IPlayerController>().To<PlayerController>().AsSingle();
        Container.Bind<TilemapVisualizer>().FromInstance(_tilemapVisualizer).AsSingle().NonLazy();
        Container.Bind<RoomContentGenerator>().FromInstance(_roomContentGenerator).AsSingle();
        Container.Bind<PrefabPlacer>().FromNewComponentOnNewPrefab(_prefabPlacer).AsTransient();
        Container.BindInterfacesAndSelfTo<RoomFirstDungeonGenerator>()
            .FromInstance(_roomFirstDungeonGenerator)
            .AsSingle();
        Container.Bind<DungeonMapController>().FromInstance(_dungeonMapController).AsSingle();
        Container.Bind<IGameController>().To<GameController>().AsCached().NonLazy();
        Container.DeclareSignal<CamEffectsSignal>();
    }

    private void PlayerCameraSetup(InjectContext context,object cameraController)
    {
        (cameraController as CameraController).InitializeCamera(_cam);
    }

}
