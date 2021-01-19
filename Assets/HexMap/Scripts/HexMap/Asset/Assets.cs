using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexMap.Runtime
{
    public class Assets
    {
        public static int VELOCITY = 400;
        public static int REST_FRAME = 5;

        private static Dictionary<string, AssetUnit> _unitDict = new Dictionary<string, AssetUnit>();

        private static Dictionary<string, List<IAssetsLoader>> _loaderDict = new Dictionary<string, List<IAssetsLoader>>();
        private static Dictionary<string, AssetRequest> _requestDict = new Dictionary<string, AssetRequest>();

        private static Queue<IAssetsLoader> _loaderQueue = new Queue<IAssetsLoader>();
        private static Queue<AssetUnit> _unitQueue = new Queue<AssetUnit>();

        private static int _maxPassCount;
        private static int _passCount;
        private static int _frameCount;

        #region loader

        public static void Add(string id, IAssetsLoader loader)
        {
            OnAdd(id);
            _loaderDict[id].Add(loader);
            LoadAsset(id);
        }

        public static void RemoveAll()
        {
            var loaderToRemove = _loaderDict.Select(pair => pair.Key).ToList();
            foreach (var id in loaderToRemove)
            {
                _loaderDict.Remove(id);
            }
        }

        public static void Remove(string id, IAssetsLoader loader)
        {
            OnRemove(id);
            _loaderDict[id].Remove(loader);
        }

        static void Remove(string id)
        {
            OnRemove(id);
            _loaderDict.Remove(id);
        }

        static void Broadcast(AssetUnit unit)
        {
            if (_loaderDict.TryGetValue(unit.id, out var loaders))
            {
                foreach (var loader in loaders)
                {
                    _loaderQueue.Enqueue(loader);
                    _unitQueue.Enqueue(unit);
                }

                Remove(unit.id);
            }
        }

        public static void Update()
        {
            if (_maxPassCount <= 0)
                _maxPassCount = _loaderQueue.Count > VELOCITY ? VELOCITY : _loaderQueue.Count;
            
            while (_passCount <= _maxPassCount && _frameCount == 0 && _loaderQueue.Count > 0)
            {
                _passCount++;
                _loaderQueue.Dequeue().OnLoadAsset(_unitQueue.Dequeue().Allocate());
            }

            if (_passCount > _maxPassCount)
            {
                _frameCount++;
                if (_frameCount >= REST_FRAME)
                {
                    _frameCount = 0;
                    _passCount = 0;
                    _maxPassCount = 0;
                }
            }
        }

        static void OnAdd(string id)
        {
            if (!_loaderDict.ContainsKey(id))
            {
                _loaderDict.Add(id, new List<IAssetsLoader>());
            }
        }

        static void OnRemove(string id)
        {
            OnAdd(id);
        }

        #endregion

        #region loader

        static void LoadAsset(string id)
        {
            if (!_requestDict.ContainsKey(id))
            {
                var request = new AssetRequest(id);
                request.Load(OnLoadAsset);
                _requestDict.Add(id, request);
                return;
            }

            OnLoadAsset(_requestDict[id]);
        }

        static void OnLoadAsset(AssetRequest request)
        {
            if (!request.isDone) return;

            AssetUnit unit = null;
            if (!_unitDict.ContainsKey(request.id))
            {
                unit = new AssetUnit(request.id, request.gameObject);
                _unitDict.Add(request.id, unit);
            }
            else
            {
                unit = _unitDict[request.id];
            }

            Broadcast(unit);
        }

        #endregion

        #region asset

        public static void Recycle(AssetIdentity identity)
        {
            if (_unitDict.ContainsKey(identity.id))
            {
                _unitDict[identity.id].Recycle(identity);
            }
        }

        #endregion
    }
}