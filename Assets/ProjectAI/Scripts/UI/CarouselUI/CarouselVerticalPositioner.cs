using DG.Tweening;
using HasanSadikin.Carousel;
using UnityEngine;

public class CarouselVerticalPositioner : MonoBehaviour
{
    [SerializeField] private float _offSetY;
    [SerializeField] private float _gap;
    [SerializeField] private Ease _ease;

    public void SetPosition(RectTransform rectTransform, int index)
    {
        float endValue = index * _gap + _offSetY;
        float duration = 0.5f;
        this.CreateSequence(rectTransform)
        .Join(rectTransform.DOAnchorPosY(endValue, duration).SetEase(_ease));
    }
}
