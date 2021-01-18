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

        private Dictionary<string, GameObject> _prefabDict = new Dictionary<string, GameObject>();
        private Dictionary<string, Stack<PoolIdentity>> _goStackDict = new Dictionary<string, Stack<PoolIdentity>>();

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

        public PoolIdentity GetGameObject(int mapId)
        {
            var data = MapEditor.instance.mapParser.GetDataWithId(MapLayerType.Prefab, mapId);
            if (data == -1) return null;

            var mapPrefab = MapEditor.instance.prefabParser.GetWithId(data);
            if (mapPrefab == null) return null;
            
            var po = GetGameObject(mapPrefab.assetPath);
            po.index = mapId;
            
            return po;
        }

        public void RecycleGameObject(PoolIdentity po)
        {
            po.transform.SetParent(null);
            if (!_goStackDict.ContainsKey(po.assetPath))
            {
                var poolStack = new Stack<PoolIdentity>();
                poolStack.Push(po);
                _goStackDict.Add(po.assetPath, poolStack);
            }
            else
            {
                _goStackDict[po.assetPath].Push(po);
            }
        }

        private GameObject GetPrefab(string assetPath)
        {
            GameObject prefab;
            if (!_prefabDict.ContainsKey(assetPath))
            {
                var path = assetPath + ".prefab";
                prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            }
            else prefab = _prefabDict[assetPath];

            return prefab;
        }

        private PoolIdentity GetGameObject(string assetPath)
        {
            if (!_goStackDict.ContainsKey(assetPath))
            {
                return CreateGameObject(assetPath);
            }

            var poolStack = _goStackDict[assetPath];
            if (poolStack.Count <= 0)
            {
                return CreateGameObject(assetPath);
            }

            return poolStack.Pop();
        }

        private PoolIdentity CreateGameObject(string assetPath)
        {
            var go = GameObject.Instantiate<GameObject>(GetPrefab(assetPath));
            var po = go.AddComponent<PoolIdentity>();
            po.assetPath = assetPath;
            return po;
        }
    }
}