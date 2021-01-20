/*
 * @Author: fasthro
 * @Date: 2021-01-06 17:38:03
 * @Description:
 */

using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Xml;
using UnityEngine;

namespace HexMap
{
    public class MapParser : XmlParser
    {
        private Dictionary<MapLayerType, MapLayer> _layerDict;
        public bool isSaved = false;

        public MapParser(string xmlPath) : base(xmlPath)
        {
            _layerDict = new Dictionary<MapLayerType, MapLayer>();
        }

        protected override void OnParseXml()
        {
            _layerDict.Clear();

            var layersRoot = _rootElement.SearchForChildByTag("Layers");
            if (layersRoot == null) return;
            var layers = layersRoot.Children;
            foreach (var layer in layers)
            {
                var layerElement = (SecurityElement)layer;
                var layerName = layerElement.Attribute("name") ?? string.Empty;
                var data = layerElement.Attribute("data") ?? string.Empty;
                var datas = data.Split(',');
                var value = new int[datas.Length];
                for (var i = 0; i < datas.Length; i++)
                {
                    value[i] = int.Parse(datas[i]);
                }

                if (layerName.Equals("prefab"))
                {
                    _layerDict.Add(MapLayerType.Prefab, new MapLayer(layerName, value));
                }
                else if (layerName.Equals("terrainType"))
                {
                    _layerDict.Add(MapLayerType.Terrain, new MapLayer(layerName, value));
                }
            }
        }

        public int GetDataWithId(MapLayerType lt, int id)
        {
            return _layerDict.ContainsKey(lt) ? _layerDict[lt].GetData(id) : -1;
        }

        public int GetOriginalDataWithId(MapLayerType lt, int id)
        {
            return _layerDict.ContainsKey(lt) ? _layerDict[lt].GetOriginalData(id) : -1;
        }

        public void SetDataWithId(MapLayerType lt, int id, int value)
        {
            if (_layerDict.ContainsKey(lt))
                _layerDict[lt].SetData(id, value);
        }

        public MapLayer GetLaterDataByType(MapLayerType lt)
        {
            return _layerDict[lt];
        }

        public void SaveMapDataToXml()
        {
            if (GetLaterDataByType(MapLayerType.Prefab).IsChanged() || GetLaterDataByType(MapLayerType.Terrain).IsChanged())
            {
                isSaved = false;
                ThreadUtils.Run(_SaveMapDataToXml, OnSaveMapDataToXml);
            }
            else
            {
                OnSaveMapDataToXml();
            }
        }

        private void OnSaveMapDataToXml()
        {
            isSaved = true;
        }

        private void _SaveMapDataToXml()
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(_xmlPath);

            XmlNodeList xmlNodeList = xml.SelectSingleNode("Map").SelectSingleNode("Layers").ChildNodes;

            foreach (XmlElement tempNode in xmlNodeList)
            {
                var layerName = tempNode.GetAttribute("name") ?? string.Empty;
                if (layerName.Equals("prefab"))
                {
                    if (GetLaterDataByType(MapLayerType.Prefab).IsChanged())
                    {
                        int[] datas = GetLaterDataByType(MapLayerType.Prefab).GetDatas();
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < datas.Length; i++)
                        {
                            sb.Append(datas[i]);
                            sb.Append(",");
                        }
                        string dataStr = sb.ToString();
                        dataStr = dataStr.Substring(0, dataStr.Length - 1);
                        tempNode.SetAttribute("data", dataStr);
                    }
                }
                else if (layerName.Equals("terrainType"))
                {
                    if (GetLaterDataByType(MapLayerType.Terrain).IsChanged())
                    {
                        int[] datas = GetLaterDataByType(MapLayerType.Terrain).GetDatas();
                        StringBuilder sb = new StringBuilder();
                        for (int i = 0; i < datas.Length; i++)
                        {
                            sb.Append(datas[i]);
                            sb.Append(",");
                        }
                        string dataStr = sb.ToString();
                        dataStr = dataStr.Substring(0, dataStr.Length - 1);
                        tempNode.SetAttribute("data", dataStr);
                    }
                }
            }
            xml.Save(_xmlPath);
            RefreshData();
        }

        public void RefreshData()
        {
            GetLaterDataByType(MapLayerType.Prefab).RefreshData();
            GetLaterDataByType(MapLayerType.Terrain).RefreshData();
        }
    }
}