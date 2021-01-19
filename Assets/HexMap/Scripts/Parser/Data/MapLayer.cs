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
        private int[] _original;

        public MapLayer(string name, int[] data)
        {
            this.name = name;

            _original = data;

            this.data = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
                this.data[i] = -1;
        }

        public int GetData(int id)
        {
            return id >= 0 && id < data.Length ? _GetData(id) : -1;
        }

        private int _GetData(int id)
        {
            return data[id] == -1 ? _original[id] : data[id];
        }

        public int GetOriginalData(int id)
        {
            return id >= 0 && id < _original.Length ? _original[id] : -1;
        }

        public void SetData(int id, int value)
        {
            if (id >= 0 && id < data.Length)
            {
                data[id] = value;
            }
        }

        public override string ToString()
        {
            return $"MapLayer: {name}";
        }
    }
}