using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using System;
using HexMap;
using UFramework;

namespace HexMap
{
    public static class SyncAsset
    {
        public static void Sync()
        {
            var settings = AssetDatabase.LoadAssetAtPath<MapEditorSettings>("Assets/HexMap/Settings/MapEditorSettings.asset");

            foreach (var assetDir in settings.assetDirectorys)
            {
                var sourceDir = IOPath.PathCombine(settings.projectDirectory, assetDir);
                var sourceFiles = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories);

                foreach (var sourceFile in sourceFiles)
                {
                    var destFile = IOPath.PathReplace(sourceFile, settings.projectDirectory, Environment.CurrentDirectory);
                    if (IsNewFile(sourceFile, destFile))
                    {
                        IOPath.FileCopy(sourceFile, destFile);
                    }
                }
            }
            
            AssetDatabase.Refresh();
            
            Debug.Log("资源同步完成");
        }

        static bool IsNewFile(string sourceFile, string destFile)
        {
            if (!IOPath.FileExists(destFile))
                return true;

            var sLen = FileSize(sourceFile);
            var dLen = FileSize(destFile);
            if (sLen != dLen)
                return true;

            var sHash = IOPath.FileMD5(sourceFile);
            var dHash = IOPath.FileMD5(destFile);
            return sHash.Equals(dHash);
        }

        static long FileSize(string path)
        {
            return (new FileInfo(path)).Length;
        }
    }
}