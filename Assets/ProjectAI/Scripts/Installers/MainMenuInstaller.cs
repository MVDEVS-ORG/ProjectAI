using Zenject;

public class MainMenuInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IUpgradeController>().To<UpgradeController>().AsSingle();
        Container.Bind<MainMenuController>().AsSingle().NonLazy();
    }
}
