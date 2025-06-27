using Assets.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

public class ObjectPoolManager
{
    [Inject] private IAssetService _assetService;
    [Inject] private ISceneManager _sceneManager;
    private GameObject _emptyHolder;

    private GameObject _particleSystemsEmpty;
    private GameObject _gameObjectsEmpty;
    private GameObject _soundFXEmpty;
    private GameObject _enemies;

    private Dictionary<string, List<GameObject>> _objectPools;
    private List<GameObject> _poolObject;
    private Dictionary<GameObject, string> _objectPrefabMap;

    public enum PoolType
    {
        ParticleSystems,
        GameObjects,
        SoundFX,
        Enemies
    }

    public PoolType PoolingType;

    [Inject]
    private void Initialize()
    {
        _objectPools = new();
        _poolObject = new();
        _objectPrefabMap = new();
        SetUpEmpties();
        _sceneManager.BeforeChangeScene += RemoveAllPoolObjects;
    }

    private void SetUpEmpties()
    {
        _emptyHolder = new GameObject("Object Pools");
        _particleSystemsEmpty = new GameObject("Particle Effects");
        _particleSystemsEmpty.transform.SetParent(_emptyHolder.transform);
        _gameObjectsEmpty = new GameObject("Game Objects");
        _gameObjectsEmpty.transform.SetParent(_emptyHolder.transform);
        _soundFXEmpty = new GameObject("Sound FX");
        _soundFXEmpty.transform.SetParent(_emptyHolder.transform);
        _enemies = new GameObject("Enemies");
        _enemies.transform.SetParent(_emptyHolder.transform);
        GameObject.DontDestroyOnLoad(_emptyHolder);
    }

    private void CreatePool(string prefabAddress)
    {
        List<GameObject> pool = new();
        _objectPools.Add(prefabAddress, pool);
    }

    private async Awaitable<GameObject> CreateObject(string prefabAddress, PoolType poolType = PoolType.GameObjects)
    {
        GameObject obj = await _assetService.InstantiateWithParentAsync(prefabAddress,SetParentObject(poolType).transform);
        obj.SetActive(false);
        return obj;
    }

    private GameObject SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.GameObjects:
                return _gameObjectsEmpty;
            case PoolType.ParticleSystems:
                return _particleSystemsEmpty;
            case PoolType.SoundFX:
                return _soundFXEmpty;
            case PoolType.Enemies:
                return _enemies;
            default:
                return null;
        }
    }

    private async Awaitable<GameObject> GetObjectFromPool(string prefabAddress,PoolType poolType)
    {
        if(_objectPools.ContainsKey(prefabAddress))
        {
            if (_objectPools[prefabAddress].Count!=0)
            {
                return _objectPools[prefabAddress][0];
            }
            else
            {
                GameObject obj = await CreateObject(prefabAddress, poolType);
                _objectPrefabMap.Add(obj, prefabAddress);
                return obj;
            }
        }
        else
        {
            return null;
        }
    }

    private async Awaitable<GameObject> SpawnObjects(string prefabAddress, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) 
    {
        if (!_objectPools.ContainsKey(prefabAddress))
        {
            CreatePool(prefabAddress);
        }

        GameObject obj = await GetObjectFromPool(prefabAddress,poolType);
        _objectPools[prefabAddress].Remove(obj);

        if (obj != null)
        {
            _poolObject.Add(obj);
            obj.transform.parent = null;
            obj.transform.position = spawnPos;
            obj.transform.rotation = spawnRotation;
            obj.SetActive(true);
            return obj;
        }
        return obj;
    }

    public async Awaitable<GameObject> SpawnObjectAsync(string prefabAddress, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects)
    {
        return await SpawnObjects(prefabAddress, spawnPos, spawnRotation, poolType);
    }

    public void ReleaseGameObject(GameObject gameObject,PoolType poolType)
    {
        try
        {
            gameObject.SetActive(false);
            gameObject.transform.parent = SetParentObject(poolType).transform;
            _objectPools[_objectPrefabMap[gameObject]].Add(gameObject);
        }
        catch(Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    public void DestroyObject(GameObject gameobject)
    {
        string address = _objectPrefabMap[gameobject];
        _objectPrefabMap.Remove(gameobject);
        _objectPools[address].Remove(gameobject);
        _poolObject.Remove(gameobject);
        GameObject.Destroy(gameobject);
    }

    public void RemoveAllPoolObjects()
    {
        foreach (GameObject gameObject in _poolObject)
        {
            DestroyObject(gameObject);
        }
    }
}
