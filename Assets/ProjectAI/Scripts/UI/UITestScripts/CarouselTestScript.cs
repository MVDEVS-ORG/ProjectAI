using Assets.Services;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CarouselTestScript : MonoBehaviour
{
    [Inject] IAssetService _assetService;

    [SerializeField] Carousel Carousel;
    [SerializeField] List<Sprite> _list;
    Dictionary<string, Sprite> _sprites = new();
    List<string> Ref=new List<string>();
    int j = 0;
    [SerializeField] Sprite _addSprite;
    [SerializeField] Sprite _replaceSprite;

    private async void Start()
    {
        int i = 0;
        foreach (var sprite in _list)
        {
            _sprites[sprite.name + i] = sprite;
            Ref.Add(sprite.name+i);
        }
        await Carousel.Initialize(_sprites, _assetService);
    }

    public async void AddItem()
    {
        await Carousel.AddItem(_addSprite.name + j, _addSprite, true);
        j++;
    }

    public void Replace()
    {
        Carousel.ReplaceItem(Carousel.GetCurrentIndexName(), "asd" + j, _replaceSprite);
        j++;
    }
}
