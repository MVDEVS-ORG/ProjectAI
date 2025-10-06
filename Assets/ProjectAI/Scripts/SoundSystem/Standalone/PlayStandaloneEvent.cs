using FMODUnity;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PlayStandaloneEvent : MonoBehaviour, IPointerEnterHandler, ISelectHandler
{
    [SerializeField] private EventReference _selectedSoundReference;
    [SerializeField] private EventReference _clickedSoundReference;
    [SerializeField] private bool _objInWorldSpace;
    private Button _targetButton;

    private void Start()
    {
        _targetButton = gameObject.GetComponent<Button>();
        _targetButton.onClick.AddListener(() =>
        {
            PlaySound(_clickedSoundReference);
        });
    }
    public void PlaySound(EventReference reference)
    {
        if(_objInWorldSpace)
        {
            SoundUtils.PlayStandaloneSFX(reference, transform.position);
        }
        else
        {
            SoundUtils.PlayStandaloneSFX(reference, null);
        }
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        PlaySound(_selectedSoundReference);
    }

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        if(!(eventData is PointerEventData))
        {
            PlaySound(_selectedSoundReference);
        }
    }
}
