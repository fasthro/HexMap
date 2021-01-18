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
        public string name => gameObject.name;
        public int index;
        public int terrain;
        public int radius;
        public GameObject gameObject;
    }

    [System.Serializable]
    public class AssetType
    {
        public string typeName;
        public string[] assetsDirectorys;
        public List<Asset> assets = new List<Asset>();
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
            List<Asset> assets = new List<Asset>();
            for (int i = 0; i < assetsTypes.Length; i++)
            {
                if (assetsTypes[i].typeName == typeName)
                {
                    assets = new List<Asset>(assetsTypes[i].assets);
                }
            }
            return assets;
        }
    }
}