using System.Collections.Generic;
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
        public string name;
        public int index;
        public int terrain;
        public int radius;
        public GameObject gameObject;
    }

    [System.Serializable]
    public class AssetType
    {
        public string typeName;
        public string[] assetsDirectory;

        public Asset[] assets;
    }

    [CreateAssetMenu(fileName = "AssetsSettings", menuName = "HexMap/AssetsSettings")]
    public class AssetsSettings : ScriptableObject
    {
        public AssetType[] assetsTypes;

        public List<Dropdown.OptionData> GetDropdownAssetTypes()
        {
            List<Dropdown.OptionData> datas = new List<Dropdown.OptionData>();
            for (int i = 0; i < assetsTypes.Length; i++)
            {
                datas.Add(new Dropdown.OptionData(assetsTypes[i].typeName));
            }

            return datas;
        }
    }
}