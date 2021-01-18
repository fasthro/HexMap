using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace HexMap
{
    public static class GenerateAssetIndex
    {
        public static void Gentrate()
        {
            var settings = AssetDatabase.LoadAssetAtPath<AssetsSettings>("Assets/HexMap/Settings/AssetsSettings.asset");

            foreach (var assetType in settings.assetsTypes)
            {
                assetType.assets.Clear();
                foreach (var dir in assetType.assetsDirectorys)
                {
                   var files = Directory.GetFiles(dir, "*.prefab", SearchOption.AllDirectories);
                   foreach (var file in files)
                   {
                       var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(file);
                       var asset = new Asset()
                       {
                           gameObject = gameObject,
                       };
                       
                       assetType.assets.Add(asset);
                   }
                }
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            Debug.Log("美术资源索引生成完成");
        }
    }
}