/*
 * @Author: fasthro
 * @Date: 2021-01-06 18:12:50
 * @Description: 
 */
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

namespace HexMap
{
    public class PrefabParser : XmlParser
    {
        public Dictionary<int, MapPrefab> mapPrefabDict = new Dictionary<int, MapPrefab>();
        public PrefabParser(string xmlPath) : base(xmlPath)
        {
        }

        protected override void OnParseXml()
        {
            mapPrefabDict.Clear();

            var prefabsRoot = _rootElement.SearchForChildByTag("Prefabs");
            var prefabs = prefabsRoot.Children;
            foreach (var prefab in prefabs)
            {
                SecurityElement layerElement = (SecurityElement)prefab;
                int index = int.Parse(layerElement.Attribute("index"));
                string name = layerElement.Attribute("name");
                int terrain = int.Parse(layerElement.Attribute("terrain"));

                mapPrefabDict.Add(index, new MapPrefab(index, name, terrain));
            }
        }
    }
}
