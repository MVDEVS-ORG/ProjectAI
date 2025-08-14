using Assets.Services;
using UnityEngine;
using Zenject;

public class ChestInteraction : MonoBehaviour, IInteractable
{
    [Inject] IUpgradeController _controller;
    [Inject] IAssetService _assetService;

    [SerializeField] string _TextOnOpening;
    [SerializeField] Animator _chestAnimator;

    private bool _opened = false;

    void IInteractable.Interact(Transform Interactor)
    {
        // TODO add a probablility system which gives multiple rewards like guns and cards
        // also add an intermediary that will get the rewards list and then dispense them accordingly
        if (!_opened)
        {
            _controller.DisplayUpgrades();
            _ = DisplayMessage();
            _chestAnimator?.Play("Opened");
            _opened = true;
        }
    }

    private async Awaitable DisplayMessage()
    {
        GameObject obj = await _assetService.InstantiateAsync(AddressableIds.PopUp_UI);
        obj.GetComponent<PopUpUI>().DisplayTextWithDuration(_TextOnOpening, 5f);
    }
}
