using System;
using System.Collections.Generic;
using Assets.Services;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GunUI : MonoBehaviour, IGunUI
{
    [Inject] IAssetService _assetService;

    private GunsModel _currentGun;

    Dictionary<GunsModel, GunUIHolder> GunUIs = new();

    private bool _initialized = false;
    private Transform _playerTransform;

    public bool Initialized => _initialized;

    async Awaitable IGunUI.Initialize(GunsModel model, Transform playerTransform)
    {
        _currentGun = model;
        _playerTransform = playerTransform;
        GameObject obj = await _assetService.InstantiateWithParentAsync(model.GunUIAddressable, transform);
        GunUIHolder gunUI = obj.GetComponent<GunUIHolder>();
        GunUIs.Add(model, gunUI);
        _initialized = true;
    }

    void IGunUI.UpdateCoolDown()
    {
        GunUIs[_currentGun].GunOverHeatFill.fillAmount = _currentGun.OverHeatValue / _currentGun.OverHeatLimit;
    }

    void Update()
    {
        try
        {
            if (_initialized)
            {
                if (_currentGun != null && GunUIs.ContainsKey(_currentGun) && GunUIs[_currentGun].gameObject.activeSelf)
                {
                    GunUIs[_currentGun].transform.position = _playerTransform.position + new Vector3(GunUIs[_currentGun].Offset.x, GunUIs[_currentGun].Offset.y, GunUIs[_currentGun].transform.position.z);
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex);
        }
    }

    async Awaitable IGunUI.AddGun(GunsModel model)
    {
        GunUIs[_currentGun].gameObject.SetActive(false);
        _currentGun = model;
        GameObject obj = await _assetService.InstantiateWithParentAsync(model.GunUIAddressable, transform);
        GunUIHolder gunUI = obj.GetComponent<GunUIHolder>();
        GunUIs.Add(model, gunUI);
    }

    void IGunUI.RemoveGun(GunsModel model, GunsModel switchTo)
    {
        _currentGun = switchTo;
        GameObject.Destroy(GunUIs[model].gameObject);
        GunUIs.Remove(model);
    }

    void IGunUI.SwapGun(GunsModel model)
    {
        GunUIs[_currentGun].gameObject.SetActive(false);
        _currentGun = model;
        GunUIs[model].gameObject.SetActive(true);
    }
}

