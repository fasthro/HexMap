/*
 * @Author: fasthro
 * @Date: 2021-01-06 17:38:03
 * @Description: 
 */
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

namespace HexMap
{
    public class MapParser : XmlParser
    {
        public Dictionary<string, MapLayer> mapLayerDict = new Dictionary<string, MapLayer>();
        public MapParser(string xmlPath) : base(xmlPath)
        {
        }

        protected override void OnParseXml()
        {
            mapLayerDict.Clear();

            var layersRoot = _rootElement.SearchForChildByTag("Layers");
            var layers = layersRoot.Children;
            foreach (var layer in layers)
            {
                SecurityElement layerElement = (SecurityElement)layer;
                string layerName = layerElement.Attribute("name");
                string data = layerElement.Attribute("data");
                string[] datas = data.Split(',');
                int[] value = new int[data.Length];
                for (int i = 0; i < datas.Length; i++)
                {
                    value[i] = int.Parse(datas[i]);
                }
                mapLayerDict.Add(layerName, new MapLayer(layerName, value));
            }
        }
    }
}
