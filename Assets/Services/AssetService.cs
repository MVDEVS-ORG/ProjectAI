using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Services
{
    public class AssetService : IAssetService
    {
        public async Awaitable<GameObject> InstantiateAsync(string address, bool active = true)
        {
            try
            {
                GameObject go = await Addressables.InstantiateAsync(address).Task;
                if (!active)
                    go.SetActive(false);
                return go;
            }
            catch 
            {
                Debug.LogError($"Error while trying to instantiate {address} object");
            }
            return null;
        }

        public async Awaitable<GameObject> InstantiateWithPRAsync(string address, Vector3 position, Quaternion rotation, bool active = true)
        {
            try
            {
                GameObject go = await Addressables.InstantiateAsync(address, position, rotation).Task;
                if (!active)
                    go.SetActive(false);
                return go;
            }
            catch
            {
                Debug.LogError($"Error while trying to instantiate {address} object");
            }
            return null;
        }

        public async Awaitable<GameObject> InstantiateWithParentAsync(string address, Transform parent, bool active = true)
        {
            try
            {
                GameObject go = await Addressables.InstantiateAsync(address, parent).Task;
                if (!active)
                    go.SetActive(false);
                return go;
            }
            catch
            {
                Debug.LogError($"Error while trying to instantiate {address} object");
            }
            return null;
        }

        public async Awaitable<T> LoadAssetAsync<T>(string address)
        {
            var asset = await Addressables.LoadAssetAsync<T>(address).Task;
            return asset;
        }

        public void UnloadAsset<T>(T asset)
        {
            Addressables.Release(asset);
        }
    }
}