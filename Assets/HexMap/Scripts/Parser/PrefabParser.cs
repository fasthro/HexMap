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
        private List<MapPrefab> _prefabs;

        public readonly Dictionary<int, int> idDict = new Dictionary<int, int>();
        public readonly Dictionary<string, int> nameDict = new Dictionary<string, int>();

        public PrefabParser(string xmlPath) : base(xmlPath)
        {
            _prefabs = new List<MapPrefab>();
        }

        protected override void OnParseXml()
        {
            _prefabs.Clear();
            idDict.Clear();
            nameDict.Clear();

            var prefabsRoot = _rootElement.SearchForChildByTag("Prefabs");
            if (prefabsRoot == null) return;
            var prefabs = prefabsRoot.Children;
            foreach (var prefab in prefabs)
            {
                var layerElement = (SecurityElement) prefab;
                var index = int.Parse(layerElement.Attribute("index") ?? string.Empty);
                var name = layerElement.Attribute("name") ?? string.Empty;
                var terrain = int.Parse(layerElement.Attribute("terrain") ?? string.Empty);
                var radius = int.Parse(layerElement.Attribute("radius") ?? string.Empty);

                idDict.Add(index, _prefabs.Count);
                nameDict.Add(name, _prefabs.Count);
                _prefabs.Add(new MapPrefab(index, name, terrain, radius));
            }
        }

        public MapPrefab GetWithId(int id)
        {
            return idDict.ContainsKey(id) ? _prefabs[idDict[id]] : null;
        }

        public MapPrefab GetWithName(string name)
        {
            return nameDict.ContainsKey(name) ? _prefabs[nameDict[name]] : null;
        }
    }
}