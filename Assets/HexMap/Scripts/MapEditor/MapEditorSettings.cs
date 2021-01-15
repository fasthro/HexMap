using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace HexMap
{
    [CreateAssetMenu(fileName = "MapEditorSettings", menuName = "HexMap/MapEditorSettings")]
    public class MapEditorSettings : ScriptableObject
    {
        /// <summary>
        /// 同步美术资源目录
        /// </summary>
        [Header("同步美术资源目录")]
        public string syncAssetDirectory;
    }
}