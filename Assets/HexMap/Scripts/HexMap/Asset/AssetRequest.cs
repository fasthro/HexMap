using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace HexMap.Runtime
{
    public enum AssetLoadState
    {
        Init,
        Loading,
        Loaded,
        Unload,
    }

    public class AssetRequest : ICoroutineWork
    {
        public string id;
        public GameObject gameObject { get; private set; }
        public string assetPath => $"{id}.prefab";
        
        public bool isDone => _state == AssetLoadState.Loaded;

        private AssetLoadState _state;
        private Action<AssetRequest> _callback;
        

        public AssetRequest(string id)
        {
            this.id = id;
            _state = AssetLoadState.Init;
        }

        public IEnumerator DoCoroutineWork()
        {
            _state = AssetLoadState.Loading;
            gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            yield return null;
            _state = AssetLoadState.Loaded;
            _callback?.Invoke(this);
        }

        public void Load(Action<AssetRequest> callback)
        {
            if (_state == AssetLoadState.Init)
            {
                _callback = callback;
                CoroutineWorker.Push(this);
            }
        }
    }
}