using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Services
{
    public interface IAssetService
    {
        Awaitable<T> LoadAssetAsync<T>(string address);
        Awaitable<GameObject> InstantiateAsync(string address, bool active = true);
        void UnloadAsset<T>(T asset);
        Awaitable<GameObject> InstantiateWithPRAsync(string address, Vector3 position, Quaternion rotation, bool active = true);
        Awaitable<GameObject> InstantiateWithParentAsync(string address, Transform parent, bool active = true);
    }
}