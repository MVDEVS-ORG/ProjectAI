using HasanSadikin.Carousel;
using System;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class GunData
{
    public Sprite Sprite;
}



public class GunCarousel : CarouselController<GunData>
{
    private void OnEnable()
    {
        OnItemSelected.AddListener(LogItem);
    }

    private void LogItem(GunData data)
    {
        Debug.Log(data.Sprite);
    }

    public void UpdateGuns()
    {
        UpdateData();
    }
}
