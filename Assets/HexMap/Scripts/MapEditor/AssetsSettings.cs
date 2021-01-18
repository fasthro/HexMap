using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;

#endif

namespace HexMap
{
    [System.Serializable]
    public class Asset
    {
        public int index;
        public int terrain;
        public int radius;

        public string name => gameObject.name;
        public GameObject gameObject;
    }

    [System.Serializable]
    public class AssetType
    {
        public string typeName;
        public string[] assetsDirectorys;

        public Asset[] assets;
    }

    [CreateAssetMenu(fileName = "AssetsSettings", menuName = "HexMap/AssetsSettings")]
    public class AssetsSettings : ScriptableObject
    {
        public AssetType[] assetsTypes;

        public List<Dropdown.OptionData> GetDropdownAssetTypes()
        {
            return assetsTypes.Select(assetType => new Dropdown.OptionData(assetType.typeName)).ToList();
        }

        public List<Asset> GetAssetListByTypeName(string typeName)
        {
            var list = new List<Asset>();
            foreach (var assetsType in assetsTypes)
            {
                if (assetsType.typeName == typeName)
                {
                    list.AddRange(assetsType.assets);
                }
            }
            return list;
        }
    }
}