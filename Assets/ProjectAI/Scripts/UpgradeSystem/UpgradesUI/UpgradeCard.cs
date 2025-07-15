using Assets.Services;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeCard : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _descriptionText;
    [SerializeField] private Image _cardImage;
    public Button CardButton;
    [HideInInspector] public List<UpgradeSO> UpgradeCardInfo;
    private UpgradesPopUp _popup;
    private IAssetService _assetService;
    public void LoadCard(List<UpgradeSO> card, UpgradesPopUp popup)
    {
        try
        {
            if (card.Count != 0)
            {
                UpgradeCardInfo = card;
                _titleText.text = card[0].Header;
                _descriptionText.text = card[0].Description;
                _ = LoadSprite(card[0].SpriteAddressable);
                CardButton.onClick.AddListener(() => { popup.OnSelected(UpgradeCardInfo); });
            }
        }
        catch (Exception exception)
        {
            Debug.LogError(exception);
            popup.gameObject.SetActive(false);
        }
    }

    public void Initialize(UpgradesPopUp popup, IAssetService service)
    {
        _assetService = service;
        _popup = popup;
    }

    public async Awaitable LoadSprite(string sprite)
    {
        _cardImage.sprite = await _assetService.LoadAssetAsync<Sprite>(sprite);
    }

    public void Clear()
    {
        /*UpgradeCardInfo = null;
        _titleText.text = "test";
        _descriptionText.text = "test";*/
    }
}
