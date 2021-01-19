using System.Collections.Generic;
using UnityEngine;

namespace HexMap.Runtime
{
    public class AssetUnit
    {
        public string id { get; private set; }

        private GameObject _prefab;
        private Stack<AssetIdentity> _stacks = new Stack<AssetIdentity>();

        public AssetUnit(string id, GameObject prefab)
        {
            this.id = id;
            this._prefab = prefab;
        }

        /// <summary>
        /// 分配对象
        /// </summary>
        /// <returns></returns>
        public AssetIdentity Allocate()
        {
            AssetIdentity poolIdentity = null;
            if (_stacks.Count > 0)
            {
                poolIdentity = _stacks.Pop();
            }
            else
            {
                var newGo = Object.Instantiate<GameObject>(_prefab, null, true);
                poolIdentity = newGo.AddComponent<AssetIdentity>();
                poolIdentity.id = id;
            }

            poolIdentity.gameObject.SetActive(true);
            return poolIdentity;
        }
        
        /// <summary>
        ///  回收对象
        /// </summary>
        /// <param name="assetIdentity"></param>
        /// <returns></returns>
        public bool Recycle(AssetIdentity assetIdentity)
        {
            if (assetIdentity == null)
            {
                GameObject gameObject;
                (gameObject = assetIdentity.gameObject).transform.SetParent(null);
                Object.Destroy(gameObject);
                return false;
            }
            
            assetIdentity.gameObject.SetActive(false);
            assetIdentity.gameObject.transform.SetParent(HexMap.instance.AssetsRoot);
            _stacks.Push(assetIdentity);
            return true;
        }
    }
}