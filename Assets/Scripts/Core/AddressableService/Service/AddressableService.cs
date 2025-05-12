using System.Collections.Generic;
using System.Threading.Tasks;
using Core.AddressableService.Interface;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Core.AddressableService.Service
{
    public class AddressableService : IAddressableService
    {
        private Dictionary<string, GameObject> _addressableObject = new Dictionary<string, GameObject>();
        private Dictionary<string, ScriptableObject> _addressableScriptableObject = new Dictionary<string, ScriptableObject>();
        private Dictionary<string, AudioClip> _addressableAudioClips = new Dictionary<string, AudioClip>();

        public Task Inject()
        {
            return Task.CompletedTask;
        }
        public async Task<GameObject> LoadObject(string key)
        {
            GameObject instantiatedObject;
            if (_addressableObject.ContainsKey(key))
            {
                instantiatedObject = Object.Instantiate(_addressableObject[key]);
                return instantiatedObject;
            }
            var opHandle = Addressables.LoadAssetAsync<GameObject>(key);
            await opHandle.Task;
            _addressableObject[key] = opHandle.Result;
            instantiatedObject = Object.Instantiate(_addressableObject[key]);
            return instantiatedObject;
        }
        
        public async Task<ScriptableObject> LoadScriptableObject(string key)
        {
            ScriptableObject scriptableObject;
            if (_addressableScriptableObject.ContainsKey(key))
            {
                scriptableObject = _addressableScriptableObject[key];
                return scriptableObject;
            }
            var opHandle = Addressables.LoadAssetAsync<ScriptableObject>(key);
            await opHandle.Task;
            _addressableScriptableObject[key] = opHandle.Result;
            scriptableObject = _addressableScriptableObject[key];
            return scriptableObject;
        }

        public async Task<AudioClip> LoadAudioClip(string key)
        {
            AudioClip audioClip;
            if (_addressableAudioClips.ContainsKey(key))
            {
                audioClip = _addressableAudioClips[key];
                return audioClip;
            }
            var opHandle = Addressables.LoadAssetAsync<AudioClip>(key);
            await opHandle.Task;
            _addressableAudioClips[key] = opHandle.Result;
            audioClip = _addressableAudioClips[key];
            return audioClip;
        }
    }
}