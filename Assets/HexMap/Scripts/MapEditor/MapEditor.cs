/*
 * @Author: fasthro
 * @Date: 2021-01-12 12:14:37
 * @Description: 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using HexMap.Runtime;

namespace HexMap
{
    public class MapEditor : MonoBehaviour
    {
        public string mapXmlPath;
        public string prefabXmlPath;
        public EditorModel defaultEditorModel = EditorModel.Hex;
        public Runtime.HexMap hexMap;

        public UnityEvent onCompleted;

        public MapParser mapParser { get; private set; }
        public PrefabParser prefabParser { get; private set; }

        private bool _isLoaded;
        private bool _isParsed;

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
            // EditorUI.instance.loading.Show("正在加载地图配置文件...");
            mapParser = new MapParser(mapXmlPath);
            mapParser.LoadXml();

            prefabParser = new PrefabParser(prefabXmlPath);
            // prefabParser.LoadXml();
            // prefabParser.LoadXml();
            #endregion
        }

        void Update()
        {
            if (!_isLoaded && mapParser.isLoaded && prefabParser.isLoaded)
            {
                // EditorUI.instance.loading.Show("正在解析地图配置文件...");
                _isLoaded = true;
                // mapParser.ParseXml();
                // prefabParser.ParseXml();
            }

            if (!_isParsed && mapParser.isParsed && prefabParser.isParsed)
            {
                EditorUI.instance.loading.Hide();
                _isParsed = true;
                onCompleted.Invoke();
            }
        }
    }
}
