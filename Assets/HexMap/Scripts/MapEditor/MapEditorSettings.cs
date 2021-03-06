﻿using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace HexMap
{
    [CreateAssetMenu(fileName = "MapEditorSettings", menuName = "HexMap/MapEditorSettings")]
    public class MapEditorSettings : ScriptableObject
    {
        /// <summary>
        /// 同步美术资源目录
        /// </summary>
        [Tooltip("资源项目目录")]
        public string projectDirectory;
        
        [Tooltip("资源目录列表")]
        public string[] assetDirectorys;
        
        [Tooltip("地图XML数据文件路径")]
        public string mapXmlPath;
        
        [Tooltip("地图资源XML数据文件路径")]
        public string prefabXmlPath;
    }
}