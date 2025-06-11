using Assets.Services;
using UnityEngine;
using UnityEngine.Analytics;
using Zenject;

public class CoreInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ISceneManager>().To<SceneManager>().AsSingle().NonLazy();
        Container.Bind<IAssetService>().To<AssetService>().AsSingle().NonLazy();
        Container.Bind<ObjectPoolManager>().AsSingle().NonLazy();
    }
}
