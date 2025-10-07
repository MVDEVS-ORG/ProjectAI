using FMOD.Studio;
using FMODUnity;
using Newtonsoft.Json;
using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SoundSettingsUI : MonoBehaviour
{

    private Bus _masterBus;
    private Bus _musicBus;
    private Bus _sfxBus;
    private Bus _ambienceBus;

    private DataSerializer _dataSerializer = new DataSerializer();

    private SoundVolumeData _volumeData;

    [SerializeField] private Slider _masterVolumeSlider;
    [SerializeField] private Slider _musicVolumeSlider;
    [SerializeField] private Slider _ambienceVolumeSlider;
    [SerializeField] private Slider _sfxVolumeSlider;

    [Header("Transitions To")]
    [SerializeField] private GameObject BackTo; 

    private void Start()
    {
        _masterBus = RuntimeManager.GetBus("bus:/");
        _musicBus = RuntimeManager.GetBus("bus:/Music");
        _sfxBus = RuntimeManager.GetBus("bus:/SFX");
        _ambienceBus = RuntimeManager.GetBus("bus:/Ambience");
        try
        {
            _volumeData = _dataSerializer.LoadData<SoundVolumeData>(AddressableIds.Sound_Volume_Data_Path);
        }
        catch(Exception ex)
        {
            Debug.LogWarning(ex);
            _volumeData = new SoundVolumeData(0.5f, 0.5f, 0.5f, 05f);
        }

        SetVolumeSliders();
        _masterVolumeSlider.onValueChanged.AddListener((value) => { _masterBus.setVolume(Mathf.Clamp(value,0f,1f)); _volumeData.MasterVolume = Mathf.Clamp(value, 0f, 1f); });
        _musicVolumeSlider.onValueChanged.AddListener((value) => { _musicBus.setVolume(Mathf.Clamp(value, 0f, 1f)); _volumeData.MusicVolume = Mathf.Clamp(value, 0f, 1f); });
        _ambienceVolumeSlider.onValueChanged.AddListener((value) => { _ambienceBus.setVolume(Mathf.Clamp(value, 0f, 1f)); _volumeData.AmbienceVolume = Mathf.Clamp(value, 0f, 1f); });
        _sfxVolumeSlider.onValueChanged.AddListener((value) => { _sfxBus.setVolume(Mathf.Clamp(value, 0f, 1f)); _volumeData.SFXVolume = Mathf.Clamp(value, 0f, 1f); });

        ApplyVolume();
    }

    private async void SetVolumeSliders()
    {
        while(_volumeData==null)
        {
            await Awaitable.EndOfFrameAsync();
        }
        _masterVolumeSlider.value = _volumeData.MasterVolume;
        _musicVolumeSlider.value = _volumeData.MusicVolume;
        _ambienceVolumeSlider.value = _volumeData.AmbienceVolume;
        _sfxVolumeSlider.value = _volumeData.SFXVolume;
    }

    public void OnEnable()
    {
        UIController.LookAtUI(true, gameObject);
        SetVolumeSliders ();
        ApplyVolume();
    }

    public void OnDisable()
    {
        UIController.LookAtUI(false, gameObject);
    }

    public async void ApplyVolume()
    {
        while (_volumeData == null)
        {
            await Awaitable.EndOfFrameAsync();
        }
        _volumeData.MasterVolume = Mathf.Clamp(_volumeData.MasterVolume, 0f, 1f);
        _volumeData.MusicVolume = Mathf.Clamp(_volumeData.MusicVolume, 0f, 1f);
        _volumeData.AmbienceVolume = Mathf.Clamp(_volumeData.AmbienceVolume, 0f, 1f);
        _volumeData.SFXVolume = Mathf.Clamp(_volumeData.SFXVolume, 0f, 1f);
        _dataSerializer.SaveData<SoundVolumeData>(AddressableIds.Sound_Volume_Data_Path, _volumeData);
    }

    public void BackButton()
    {
        BackTo.SetActive(true);
        gameObject.SetActive(false);
    }
}

[Serializable]
public class SoundVolumeData
{
    public float MasterVolume;
    public float MusicVolume;
    public float AmbienceVolume;
    public float SFXVolume;

    public SoundVolumeData(float masterVolume, float musicVolume, float ambienceVolume, float sFXVolume)
    {
        MasterVolume = masterVolume;
        MusicVolume = musicVolume;
        AmbienceVolume = ambienceVolume;
        SFXVolume = sFXVolume;
    }
}

