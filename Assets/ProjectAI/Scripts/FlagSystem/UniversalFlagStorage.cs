using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UniversalFlagStorage
{
    [SerializeField] List<Flag> _allFlags = new();
    [SerializeField] Dictionary<FlagType, List<Flag>> _flagsByType = new();

    DataSerializer _serializer = new DataSerializer();

    [Inject]
    public void Initialize()
    {
        SetUpFlags();
    }

    public void AddFlags(List<Flag> flags)
    {
        foreach(var flag in flags)
        {
            AddFlag(flag);
        }
    }

    public void AddFlag(Flag flag)
    {
        if(_allFlags==null)
        {
            _allFlags = new List<Flag>();
        }
        if(_flagsByType == null)
        {
            _flagsByType = new();
        }

        if (_allFlags.Contains(flag))
        {
            Debug.LogError($"{flag.name} is already present");
            return;
        }
        _allFlags.Add(flag);
        try
        {
            _flagsByType[flag.Type].Add(flag);
        }
        catch
        {
            _flagsByType[flag.Type] = new() { flag };
        }
        SaveFlags();
    }

    public bool ValidateFlag(Flag flag)
    {
        return _allFlags.Contains(flag);
    }

    public bool ChechIfType(Flag flag, FlagType type)
    {
        return _flagsByType[type].Contains(flag);
    }

    public void SetUpFlags()
    {
        _allFlags.Clear();
        _flagsByType.Clear();
        try
        {
            _flagsByType = _serializer.LoadData<Dictionary<FlagType, List<Flag>>>(AddressableIds.Player_Flags);
            foreach (var flag in _flagsByType)
            {
                _allFlags.AddRange(flag.Value);
            }
        }
        catch
        {
            SaveFlags();
        }
    }

    public void EraseFlagsByType(FlagType type)
    {
        for (int i = 0; i < _flagsByType[type].Count; i++)
        {
            _allFlags.Remove(_flagsByType[type][i]);
        }
        _flagsByType[type].Clear();
    }

    public void EraseOnRunEnd()
    {
        EraseFlagsByType(FlagType.Temporary);
        EraseFlagsByType(FlagType.PersistAcrossRun);
    }

    public void SaveFlags()
    {
        _serializer.SaveData<Dictionary<FlagType, List<Flag>>>(AddressableIds.Player_Flags, _flagsByType);
    }

    public void RemoveSpecificFlag(Flag flag)
    {
        _allFlags.Remove(flag);
        _flagsByType[flag.Type].Remove(flag);
    }

    public List<Flag> GetFlags() => _allFlags;

    public List<Flag> GetFlagsByType(FlagType type) => _flagsByType[type];
}
