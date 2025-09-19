using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElementalistAbilityUI : MonoBehaviour
{
    [SerializeField] private List<Sprite> _elementSprites;
    [SerializeField] private Image _spriteImage;

    public void SetImage(int index)
    {
        _spriteImage.sprite = _elementSprites[index];
    }
}
