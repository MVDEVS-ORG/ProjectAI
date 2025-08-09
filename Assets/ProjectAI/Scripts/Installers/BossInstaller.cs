using Assets.ProjectAI.Scripts.DungeonScripts;
using Assets.ProjectAI.Scripts.DungeonScripts.RoomSystem.Items;
using Assets.ProjectAI.Scripts.GameController;
using Assets.ProjectAI.Scripts.PathFinding;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;
using Zenject;

namespace Assets.ProjectAI.Scripts.Installers
{
    public class BossInstaller : MonoInstaller
    {
        [SerializeField] private CinemachineCamera _cam;
        [SerializeField] private GameObject _prefabPlacer;
        public override void InstallBindings()
        {
            Container.Bind<GamepadRumble>().AsSingle();
            Container.Bind<CameraController>().AsSingle().OnInstantiated(PlayerCameraSetup);
            Container.Bind<IUpgradeController>().To<UpgradeController>().AsSingle();
            Container.Bind<IGunsController>().To<GunsController>().AsSingle();
            Container.Bind<IMeleeWeaponController>().To<MeleeWeaponController>().AsSingle();
            Container.Bind<IPlayerController>().To<PlayerController>().AsSingle();
            Container.Bind<PrefabPlacer>().FromNewComponentOnNewPrefab(_prefabPlacer).AsTransient();
            Container.Bind<IBossRoomController>()
            .To<BossRoomController>()
            .AsCached()
            .NonLazy();
            Container.DeclareSignal<CamEffectsSignal>();
        }

        private void PlayerCameraSetup(InjectContext context, object cameraController)
        {
            (cameraController as CameraController).InitializeCamera(_cam);
        }
    }
}