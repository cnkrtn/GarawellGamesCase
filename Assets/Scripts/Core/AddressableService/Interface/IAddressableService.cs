using System.Threading.Tasks;
using UnityEngine;

namespace Core.AddressableService.Interface
{
    public interface IAddressableService
    {
        Task Inject();
        Task<GameObject> LoadObject(string key);
        Task<ScriptableObject> LoadScriptableObject(string key);
        Task<AudioClip> LoadAudioClip(string key);

    }
}