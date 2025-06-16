using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class GameController : IGameController
{
    [Inject] private IPlayerController playerController;
    [Inject] private PlayerPicker playerPicker;

    [Inject]
    private void Initialize()
    {
        _ = (this as IGameController).StartGame();
    }
    async Task IGameController.StartGame()
    {
        await playerPicker.SetPlayer();
        await playerController.SpawnPlayer(Vector3.zero,playerPicker.PickPlayer());
    }
}
