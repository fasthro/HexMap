using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UFramework;
using UnityEditor;
using UnityEngine;

namespace HexMap
{
    public static class GenerateAssetIndex
    {
        public static void Gentrate()
        {
            var editorSetting = AssetDatabase.LoadAssetAtPath<MapEditorSettings>("Assets/HexMap/Settings/MapEditorSettings.asset");
            var assetsSettings = AssetDatabase.LoadAssetAtPath<AssetsSettings>("Assets/HexMap/Settings/AssetsSettings.asset");

            var parser = new PrefabParser(editorSetting.prefabXmlPath);
            parser.LoadXml(false);
            parser.ParseXml(false);

            foreach (var assetType in assetsSettings.assetsTypes)
            {
                var list = new List<Asset>();
                foreach (var dir in assetType.assetsDirectorys)
                {
                    var files = Directory.GetFiles(dir, "*.prefab", SearchOption.AllDirectories);
                    foreach (var item in files)
                    {
                        var file = IOPath.PathUnitySeparator(item);
                        var parserKey = IOPath.PathReplace(file, ".prefab", "");
                        parserKey = IOPath.PathReplace(parserKey, Environment.CurrentDirectory, "");
                        var prefab = parser.GetWithName(parserKey);

                        if (prefab == null)
                        {
                            Debug.LogError($"请确认资源是否继续使用 file: {parserKey}");
                        }
                        else
                        {
                            var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(file);
                            var asset = new Asset()
                            {
                                index = prefab.index,
                                terrain = prefab.terrain,
                                radius = prefab.radius,
                                gameObject = gameObject,
                            };
                            list.Add(asset);
                        }
                    }
                }

                assetType.assets = list.ToArray();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("美术资源索引生成完成");
        }
    }
}