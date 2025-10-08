using Assets.ProjectAI.Scripts.GameController;
using Assets.ProjectAI.Scripts.Player;
using Assets.Services;
using Zenject;

public class CoreInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<SoundController>().AsCached().NonLazy();
        Container.Bind<IAssetService>().To<AssetService>().AsCached().NonLazy();
        Container.Bind<LevelManager>().AsSingle().NonLazy();
        Container.Bind<ISceneManager>().To<SceneManager>().AsSingle().NonLazy();
        Container.Bind<PlayerSelectionService>().AsSingle();
        Container.Bind<ObjectPoolManager>().AsSingle().NonLazy();
        Container.Bind<PlayerPicker>().AsSingle();
        Container.Bind<IUniversalDeviceController>().To<UniversalDeviceController>().AsCached().NonLazy();
        SignalBusInstaller.Install(Container);
    }
}
