using Assets.ProjectAI.Scripts.GameEvent;
using Assets.Services;
using UnityEngine;
using Zenject;

public class ChestInteraction : MonoBehaviour, IInteractable
{
    [Inject] IUpgradeController _controller;
    [Inject] IAssetService _assetService;

    [SerializeField] string _textOnOpening;
    [SerializeField] Animator _chestAnimator;

    private bool _opened = false;

    void IInteractable.Interact(Transform interactor)
    {
        // TODO add a probablility system which gives multiple rewards like guns and cards
        // also add an intermediary that will get the rewards list and then dispense them accordingly
        if (!_opened)
        {
            _opened = true;
            _ = HandleChestSequence();
        }
    }

    private async Awaitable HandleChestSequence()
    {
        _chestAnimator?.Play("Opened");
        await Awaitable.WaitForSecondsAsync(0.2f); // wait for chest to visually open

        // Step 2: Show message
        _controller.DisplayUpgrades();
        await Awaitable.WaitForSecondsAsync(0.3f);

        _= DisplayMessage();
        await Awaitable.WaitForSecondsAsync(0.3f);

        await DropWeapon();
    }

    private async Awaitable DisplayMessage()
    {
        GameObject obj = await _assetService.InstantiateAsync(AddressableIds.PopUp_UI);
        obj.GetComponent<PopUpUI>().DisplayTextWithDuration(_textOnOpening, 3f);

        GameEvents.PortalkeyAcquired();
    }

    private async Awaitable DropWeapon()
    {
        int rng = Random.Range(0, 10);
        string address = rng > 5 ? AddressableIds.Simple_Gun : AddressableIds.Shot_Gun;
        GameObject obj = await _assetService.InstantiateAsync(address);
        obj.transform.position = new Vector2(transform.position.x, transform.position.y - 1f);
    }
}
