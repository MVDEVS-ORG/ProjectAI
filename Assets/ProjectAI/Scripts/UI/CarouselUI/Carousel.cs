using Assets.Services;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using HasanSadikin.Carousel;

public class Carousel : MonoBehaviour
{
    [SerializeField] string CarouselPrefabAddress;
    [SerializeField] Transform _parentObject;
    List<Image> _carouselItems =  new List<Image>();
    Dictionary<string,Image> _keyValuePairs = new Dictionary<string,Image>();
    int _index = 0;
    int signedIndex;
    IAssetService _assetService;
    CarouselVerticalPositioner _verticalPositioner;

    public async Awaitable Initialize(Dictionary<string,Sprite> namePrefabAddress, IAssetService assetService)
    {
        _assetService = assetService;
        ClearCarousel();
        foreach(var address in namePrefabAddress)
        {
            await AddItem(address.Key, address.Value, false);
        }
        _verticalPositioner = GetComponent<CarouselVerticalPositioner>();
        UpdatePositions();
    }

    private void ClearCarousel()
    {
        for (int i = 0; i < _carouselItems.Count; i++)
        {
            Image obj = _carouselItems[i];
            _carouselItems.Remove(obj);
            Destroy(obj.gameObject);
        }
        _keyValuePairs.Clear();
    }

    private void UpdatePositions()
    {
        for(int i=0;i<_carouselItems.Count;i++)
        {
            _verticalPositioner.SetPosition((_carouselItems[i].transform as RectTransform), i - _index);
        }
    }

    public void Next()
    {
        EffectOutOfFocus(_carouselItems[_index]);
        _index = (_index + 1) % _carouselItems.Count;
        EffectOnFocus(_carouselItems[_index]);
        UpdatePositions();
    }

    public void Previous()
    {
        EffectOutOfFocus(_carouselItems[_index]);
        _index = (_index - 1) < 0 ? _carouselItems.Count - 1 : _index - 1;
        EffectOnFocus(_carouselItems[_index]);
        UpdatePositions();
    }

    public void MoveToIndex(string name)
    {
        EffectOutOfFocus(_carouselItems[_index]);
        Image img = _keyValuePairs[name];
        _index = _carouselItems.IndexOf(img);
        EffectOnFocus(_carouselItems[_index]);
        UpdatePositions();
    }

    public async Awaitable AddItem(string name, Sprite sprite, bool UpdateItemPositions)
    {
        if (_carouselItems != null && _carouselItems.Count > 0)
        {
            EffectOutOfFocus(_carouselItems[_index]);
        }
        GameObject obj = await _assetService.InstantiateWithParentAsync(CarouselPrefabAddress, _parentObject, false);
        Image img = obj.GetComponent<Image>();
        img.sprite = sprite;
        var rect = obj.transform as RectTransform;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = new Vector3(0, 0);
        _carouselItems.Add(img);
        _keyValuePairs.Add(name, img);
        _index = _carouselItems.IndexOf(img);
        obj.SetActive(true);
        EffectOnFocus(_carouselItems[_index]);
        if (UpdateItemPositions)
        {
            UpdatePositions();
        }
    }

    public void RemoveItem(string name)
    {
        Image img = _keyValuePairs[name];
        _keyValuePairs.Remove(name);
        _carouselItems.Remove(img);
        Destroy(img);
    }

    public void ReplaceItem(string name, string newName, Sprite sprite)
    {
        Image img = _keyValuePairs[name];
        img.sprite = sprite;
        _keyValuePairs.Remove(name);
        _keyValuePairs.Add(newName, img);
    }

    public string GetCurrentIndexName()
    {
        Image img = _carouselItems[_index];
        foreach(var data in _keyValuePairs)
        {
            if(img == data.Value)
            {
                return data.Key;
            }
        }
        return null;
    }

    private void EffectOnFocus(Image img)
    {
        this.CreateSequence(img.gameObject).Join(img.DOFade(1,0.25f)).Join((img.transform as RectTransform).DOScale(1.15f,0.25f));
    }

    private void EffectOutOfFocus(Image img)
    {
        this.CreateSequence(img.gameObject).Join(img.DOFade(0.5f, 0.25f)).Join((img.transform as RectTransform).DOScale(0.6f, 0.25f));
    }

}
