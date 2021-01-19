/*
 * @Author: fasthro
 * @Date: 2021-01-12 12:14:37
 * @Description: 
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using HexMap.Runtime;
using UnityEditor;

namespace HexMap
{
    public class MapEditor : MonoBehaviour
    {
        public static MapEditor instance { get; private set; }

        public string mapXmlPath;
        public string prefabXmlPath;
        public EditorModel defaultEditorModel = EditorModel.Hex;
        public Runtime.HexMap hexMap;

        public MapEditorSettings editorSettings;
        public AssetsSettings assetsSettings;

        public UnityEvent onCompleted;

        public MapParser mapParser { get; private set; }
        public PrefabParser prefabParser { get; private set; }

        private bool _isLoaded;
        private bool _isParsed;

        private void Awake()
        {
            instance = this;
        }

        void Start()
        {
            #region initialize

            hexMap.Initialize(true);
            hexMap.SetEditorModel(defaultEditorModel);

            EditorUI.instance.Initialize(defaultEditorModel);

            #endregion

            #region parse

            _isLoaded = false;
            _isParsed = false;
            EditorUI.instance.loading.Show("正在加载地图配置文件...");
            mapParser = new MapParser(editorSettings.mapXmlPath);
            mapParser.LoadXml();

            prefabParser = new PrefabParser(editorSettings.prefabXmlPath);
            prefabParser.LoadXml();
            prefabParser.LoadXml();

            #endregion
        }

        void Update()
        {
            if (!_isLoaded && mapParser.isLoaded && prefabParser.isLoaded)
            {
                EditorUI.instance.loading.Show("正在解析地图配置文件...");
                _isLoaded = true;
                mapParser.ParseXml();
                prefabParser.ParseXml();
            }

            if (!_isParsed && mapParser.isParsed && prefabParser.isParsed)
            {
                EditorUI.instance.loading.Hide();
                _isParsed = true;
                onCompleted.Invoke();
            }
        }

        public string GetPrefabAssetPath(int mapId)
        {
            var data = MapEditor.instance.mapParser.GetDataWithId(MapLayerType.Prefab, mapId);
            if (data == -1) return null;
            var mapPrefab = MapEditor.instance.prefabParser.GetWithId(data);
            return mapPrefab == null ? null : mapPrefab.assetPath;
        }
    }
}