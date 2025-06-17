using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<PlayerPicker>().AsSingle();
        Container.Bind<IPlayerController>().To<PlayerController>().AsCached();
        Container.Bind<IGameController>().To<GameController>().AsCached().NonLazy();
    }
}
