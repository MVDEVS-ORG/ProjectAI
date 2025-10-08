using FMOD.Studio;
using FMODUnity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SoundSettingsUI : MonoBehaviour
{

    [Inject]
    private SoundController _soundController;

    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _ambienceVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

    [Header("Transitions To")]
    [SerializeField] private GameObject BackTo; 

    private Dictionary<SoundType,Slider> _sliderVolumePairs = new Dictionary<SoundType,Slider>();

    private void Start()
    {
        _sliderVolumePairs[SoundType.Master] = _masterVolumeSlider;
        _sliderVolumePairs[SoundType.Music] = _musicVolumeSlider;
        _sliderVolumePairs[SoundType.Ambience] = _ambienceVolumeSlider;
        _sliderVolumePairs[SoundType.SFX] = _sfxVolumeSlider;

        _ = SetVolumeSliders();
        foreach(var sound in _soundController.VolumeData)
        {
            _sliderVolumePairs[sound.Key].onValueChanged.AddListener((value) => { _soundController.SetVolume(sound.Key, value); });
        }
        ApplyVolume();
    }

    private async Awaitable SetVolumeSliders()
    {
        while(_soundController==null)
        {
            await Awaitable.EndOfFrameAsync();
        }
        foreach(var sound in _soundController.VolumeData)
        {
            _sliderVolumePairs[sound.Key].value = sound.Value;
        }
    }

    public void OnEnable()
    {
        UIController.LookAtUI(true, gameObject);
        _ = SetVolumeSliders ();
    }

    public void OnDisable()
    {
        UIController.LookAtUI(false, gameObject);
    }

    public void ApplyVolume()
    {
        _soundController.SaveData();
    }

    public void BackButton()
    {
        BackTo.SetActive(true);
        gameObject.SetActive(false);
    }
}



