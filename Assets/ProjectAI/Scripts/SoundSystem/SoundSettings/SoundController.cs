using FMOD.Studio;
using FMODUnity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
public class SoundController
{
    Dictionary<SoundType, Bus> _busDictionary = new Dictionary<SoundType, Bus>();
    Dictionary<SoundType, float> _volumeData = new();

    public Dictionary<SoundType, float> VolumeData => GetVolumeData();

    private Bus _masterBus;
    private Bus _musicBus;
    private Bus _sfxBus;
    private Bus _ambienceBus;

    private DataSerializer _dataSerializer = new DataSerializer();

    [Inject]
    public void Initialize()
    {
        _busDictionary[SoundType.Master] = RuntimeManager.GetBus("bus:/");
        _busDictionary[SoundType.Music] = RuntimeManager.GetBus("bus:/Music");
        _busDictionary[SoundType.SFX] = RuntimeManager.GetBus("bus:/SFX");
        _busDictionary[SoundType.Ambience] = RuntimeManager.GetBus("bus:/Ambience");
        try
        {
            _volumeData = _dataSerializer.LoadData<Dictionary<SoundType,float>>(AddressableIds.Sound_Volume_Data_Path);
            if(_volumeData.Count < _busDictionary.Count)
            {
                throw new Exception("Volume data is incomplete, resetting to default values.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogWarning(ex);
            _volumeData = new Dictionary<SoundType, float> { {SoundType.Master , 0.5f }, { SoundType.Music, 0.5f },{ SoundType.Ambience, 0.5f },{ SoundType.SFX, 0.5f } };
        }
        SetVolume();
    }

    private void SetVolume()
    {
        foreach(var bus in _busDictionary)
        {
            bus.Value.setVolume(Mathf.Clamp(_volumeData[bus.Key], 0f, 1f));
        }
    }

    public void SetVolume(SoundType type, float value)
    {
        float clampedValue = Mathf.Clamp(value, 0f, 1f);
        _busDictionary[type].setVolume(clampedValue);
        _volumeData[type] = clampedValue;
    }

    public void SaveData()
    {
        _dataSerializer.SaveData<Dictionary<SoundType, float>>(AddressableIds.Sound_Volume_Data_Path, _volumeData);
    }

    private Dictionary<SoundType, float> GetVolumeData()
    {
        return _volumeData;
    }
}

public enum SoundType
{
    Master,
    Music,
    Ambience,
    SFX
}

