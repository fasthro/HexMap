using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HexMap
{
    public class Helper
    {
        /// <summary>
        /// 同步美术资源
        /// </summary>
        [MenuItem("HexMap/同步美术资源")]
        public static void SyncArtAsset()
        {
            SyncAsset.Sync();
        }
        
        /// <summary>
        /// 生成资源索引
        /// </summary>
        [MenuItem("HexMap/生成美术资源索引")]
        public static void GenerateAssetIndexMap()
        {
            GenerateAssetIndex.Gentrate();
        }
        
        /// <summary>
        /// 移除丢失脚本
        /// </summary>
        [MenuItem("HexMap/Remove Missing-MonoBehavior Component")]
        public static void RemoveMissingMonoBehaviorComponent()
        {
            CheckMissScripts.RemoveMissComponent();
        }
    }
}