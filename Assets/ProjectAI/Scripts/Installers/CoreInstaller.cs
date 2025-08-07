using Assets.ProjectAI.Scripts.Player;
using Assets.Services;
using UnityEngine;
using UnityEngine.Analytics;
using Zenject;

public class CoreInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IAssetService>().To<AssetService>().AsSingle().NonLazy();
        Container.Bind<ISceneManager>().To<SceneManager>().AsSingle().NonLazy();
        Container.Bind<PlayerSelectionService>().AsSingle();
        Container.Bind<ObjectPoolManager>().AsSingle().NonLazy();
        Container.Bind<PlayerPicker>().AsSingle();
        SignalBusInstaller.Install(Container);
    }
}
