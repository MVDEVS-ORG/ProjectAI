using Assets.ProjectAI.Scripts.MainMenu;
using UnityEngine;
using Zenject;

public class MainMenuInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<CharacterSelectionController>().AsSingle();
        Container.Bind<MainMenuController>().AsSingle().NonLazy();
    }
}
