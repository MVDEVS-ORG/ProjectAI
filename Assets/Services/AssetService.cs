using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Assets.Services
{
    public class AssetService : IAssetService
    {
        public async Awaitable<GameObject> InstantiateAsync(string address)
        {
            GameObject go = await Addressables.InstantiateAsync(address).Task;
            return go;
        }

        public async Awaitable<GameObject> InstantiateWithPRAsync(string address, Vector3 position, Quaternion rotation)
        {
            GameObject go = await Addressables.InstantiateAsync(address,position, rotation).Task;
            return go;
        }

        public async Awaitable<GameObject> InstantiateWithParentAsync(string address, Transform parent)
        {
            GameObject go = await Addressables.InstantiateAsync(address,parent).Task;
            return go;
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