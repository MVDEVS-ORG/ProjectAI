using HasanSadikin.Carousel;
using UnityEngine;
using UnityEngine.UI;

public class GunCarouselItem : CarouselItem<GunData>
{
    [SerializeField] private Image _image;
    protected override void OnDataUpdated(GunData data)
    {
        base.OnDataUpdated(data);
        _image.sprite = data.Sprite;
    }
}
