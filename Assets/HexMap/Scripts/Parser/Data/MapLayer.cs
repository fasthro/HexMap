/*
 * @Author: fasthro
 * @Date: 2021-01-07 11:05:32
 * @Description: 
 */

namespace HexMap
{
    public enum MapLayerType
    {
        Prefab,
        Terrain,
    }

    public class MapLayer
    {
        public string name { get; private set; }
        public int[] data { get; private set; }

        public MapLayer(string name, int[] data)
        {
            this.name = name;
            this.data = data;
        }

        public int GetData(int id)
        {
            return id >= 0 && id < data.Length ? data[id] : -1;
        }

        public override string ToString()
        {
            return $"MapLayer: {name}";
        }
    }
}