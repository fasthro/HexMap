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

        private MapLayer _prefabLayer => _layerDict[MapLayerType.Prefab];
        private MapLayer _terrainLayer => _layerDict[MapLayerType.Terrain];

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
                var layerElement = (SecurityElement) layer;
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

        protected override void OnSave()
        {
            var xml = new XmlDocument();
            xml.Load(_xmlPath);

            var xmlNodeList = xml.SelectSingleNode("Map")?.SelectSingleNode("Layers")?.ChildNodes;

            if (xmlNodeList != null)
                foreach (XmlElement tempNode in xmlNodeList)
                {
                    var layerName = tempNode.GetAttribute("name") ?? string.Empty;
                    if (layerName.Equals("prefab"))
                    {
                        var datas = _prefabLayer.GetDatas();
                        var sb = new StringBuilder();
                        for (var i = 0; i < datas.Length; i++)
                        {
                            sb.Append(datas[i]);
                            sb.Append(",");
                        }

                        var dataStr = sb.ToString();
                        dataStr = dataStr.Substring(0, dataStr.Length - 1);
                        tempNode.SetAttribute("data", dataStr);
                    }
                    else if (layerName.Equals("terrainType"))
                    {
                        var datas = _terrainLayer.GetDatas();
                        var sb = new StringBuilder();
                        for (var i = 0; i < datas.Length; i++)
                        {
                            sb.Append(datas[i]);
                            sb.Append(",");
                        }

                        var dataStr = sb.ToString();
                        dataStr = dataStr.Substring(0, dataStr.Length - 1);
                        tempNode.SetAttribute("data", dataStr);
                    }
                }

            xml.Save(_xmlPath);

            _prefabLayer.SaveData();
            _terrainLayer.SaveData();
        }

        public override bool CheckSave()
        {
            return _prefabLayer.IsChanged() || _terrainLayer.IsChanged();
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
    }
}