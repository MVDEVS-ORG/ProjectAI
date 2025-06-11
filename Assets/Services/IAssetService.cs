using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Services
{
    public interface IAssetService
    {
        Task<T> LoadAssetAsync<T>(string address);
        Task<GameObject> InstantiateAsync(string address);
        void UnloadAsset<T>(T asset);
    }
}