/*
 * @Author: fasthro
 * @Date: 2021-01-07 11:14:19
 * @Description: 
 */
namespace HexMap
{
    public class MapPrefab
    {
        public int index { get; private set; }
        public string prefab { get; private set; }
        public int terrain { get; private set; }

        public MapPrefab(int index, string prefab, int terrain)
        {
            this.index = index;
            this.prefab = prefab;
            this.terrain = terrain;
        }

        public override string ToString()
        {
            return $"MapPrefab: index = {index} prefab = {prefab} terrain = {terrain}";
        }
    }
}