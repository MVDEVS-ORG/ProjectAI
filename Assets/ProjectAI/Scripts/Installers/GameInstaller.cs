using Unity.Cinemachine;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private CinemachineCamera cam;
    public override void InstallBindings()
    {
        Container.Bind<PlayerPicker>().AsSingle();
        Container.Bind<IPlayerController>().To<PlayerController>().AsCached().OnInstantiated(PlayerCameraSetup);
        Container.Bind<IGameController>().To<GameController>().AsCached().NonLazy();
    }

    private void PlayerCameraSetup(InjectContext context,object playerController)
    {
        (playerController as PlayerController).SetCam(cam);
    }

}
