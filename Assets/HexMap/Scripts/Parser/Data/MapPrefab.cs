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
        public string assetPath { get; private set; }
        public int terrain { get; private set; }
        public int radius { get; private set; }
        

        public MapPrefab(int index, string assetPath, int terrain, int radius)
        {
            this.index = index;
            this.assetPath = assetPath;
            this.terrain = terrain;
            this.radius = radius;
        }

        public override string ToString()
        {
            return $"MapPrefab: index = {index} prefab = {assetPath} terrain = {terrain} radius = {radius}";
        }
    }
}