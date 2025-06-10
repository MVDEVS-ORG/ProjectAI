using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolManager : MonoBehaviour
{
    [SerializeField] private bool _addToDontDestroyOnLoad = false;

    private GameObject _emptyHolder;

    private static GameObject _particleSystemsEmpty;
    private static GameObject _gameObjectsEmpty;
    private static GameObject _soundFXEmpty;

    private static Dictionary<GameObject, ObjectPool<GameObject>> _objectPools;
    private static Dictionary<GameObject, GameObject> _cloneToPrefab;

    public enum PoolType
    {
        ParticleSystems,
        GameObjects,
        SoundFX
    }

    public static PoolType PoolingType;

    private void Awake()
    {
        _objectPools = new();
        _cloneToPrefab = new();
        SetUpEmpties();
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

        if (_addToDontDestroyOnLoad)
        {
            DontDestroyOnLoad(_emptyHolder);
        }
    }

    private static void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc: () => CreateObject(prefab, pos, rot, poolType),
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleasedObject,
            actionOnDestroy: OnDestroyObject
            );
        _objectPools.Add(prefab, pool);


    }

    private static void CreatePool(GameObject prefab, Transform parent, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
            createFunc: () => CreateObject(prefab, parent, rot, poolType),
            actionOnGet: OnGetObject,
            actionOnRelease: OnReleasedObject,
            actionOnDestroy: OnDestroyObject
            );
        _objectPools.Add(prefab, pool);


    }

    private static GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        prefab.SetActive(false);
        GameObject obj = Instantiate(prefab, pos, rot);
        prefab.SetActive(true);
        GameObject parentObject = SetParentObject(poolType);
        obj.transform.SetParent(parentObject.transform);
        return obj;
    }

    private static GameObject CreateObject(GameObject prefab, Transform parent, Quaternion rot, PoolType poolType = PoolType.GameObjects)
    {
        prefab.SetActive(false);
        GameObject obj = Instantiate(prefab, parent);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.rotation = rot;
        obj.transform.localScale = Vector3.one;
        prefab.SetActive(true);
        return obj;
    }

    private static void OnGetObject(GameObject obj)
    {

    }

    private static void OnReleasedObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    private static void OnDestroyObject(GameObject obj)
    {
        if (_cloneToPrefab.ContainsKey(obj))
        {
            _cloneToPrefab.Remove(obj);
        }
    }

    private static GameObject SetParentObject(PoolType poolType)
    {
        switch (poolType)
        {
            case PoolType.GameObjects:
                return _gameObjectsEmpty;
            case PoolType.ParticleSystems:
                return _particleSystemsEmpty;
            case PoolType.SoundFX:
                return _soundFXEmpty;
            default:
                return null;
        }
    }

    private static T SpawnObjects<T>(GameObject objectsToSpawn, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : Object
    {
        if (!_objectPools.ContainsKey(objectsToSpawn))
        {
            CreatePool(objectsToSpawn, spawnPos, spawnRotation, poolType);
        }

        GameObject obj = _objectPools[objectsToSpawn].Get();

        if (obj != null)
        {
            if (!_cloneToPrefab.ContainsKey(obj))
            {
                _cloneToPrefab.Add(obj, objectsToSpawn);
            }
            obj.transform.position = spawnPos;
            obj.transform.rotation = spawnRotation;
            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject))
            {
                return obj as T;
            }

            T component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"Objects {objectsToSpawn.name} doesn't have component of type {typeof(T)}");
                return null;
            }
            return component;
        }
        return null;
    }

    public static T SpawnObject<T>(T prefab, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : Component
    {
        return SpawnObjects<T>(prefab.gameObject, spawnPos, spawnRotation, poolType);
    }

    public static GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects)
    {
        return SpawnObjects<GameObject>(objectToSpawn, spawnPos, spawnRotation, poolType);
    }

    public static T SpawnObject<T>(T prefab, Transform parent, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : Component
    {
        return SpawnObjects<T>(prefab.gameObject, parent, spawnRotation, poolType);
    }

    public static GameObject SpawnObject(GameObject objectToSpawn, Transform parent, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects)
    {
        return SpawnObjects<GameObject>(objectToSpawn, parent, spawnRotation, poolType);
    }

    public static void ReturnObjectToPool(GameObject obj, PoolType poolType = PoolType.GameObjects)
    {
        if (_cloneToPrefab.TryGetValue(obj, out GameObject prefab))
        {
            GameObject parentObject = SetParentObject(poolType);

            if (obj.transform.parent != parentObject.transform)
            {
                obj.transform.SetParent(parentObject.transform);
            }
            if (_objectPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
            {
                pool.Release(obj);
            }
        }
        else
        {
            Debug.LogWarning("Trying to return an object that is not pooled: " + obj.name);
        }
    }

    private static T SpawnObjects<T>(GameObject objectsToSpawn, Transform parent, Quaternion spawnRotation, PoolType poolType = PoolType.GameObjects) where T : Object
    {
        if (!_objectPools.ContainsKey(objectsToSpawn))
        {
            CreatePool(objectsToSpawn, parent, spawnRotation, poolType);
        }

        GameObject obj = _objectPools[objectsToSpawn].Get();

        if (obj != null)
        {
            if (!_cloneToPrefab.ContainsKey(obj))
            {
                _cloneToPrefab.Add(obj, objectsToSpawn);
            }
            obj.transform.SetParent(parent);
            obj.transform.localPosition = Vector3.zero;
            obj.transform.rotation = spawnRotation;
            obj.SetActive(true);

            if (typeof(T) == typeof(GameObject))
            {
                return obj as T;
            }

            T component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"Objects {objectsToSpawn.name} doesn't have component of type {typeof(T)}");
                return null;
            }
            return component;
        }
        return null;
    }
}
