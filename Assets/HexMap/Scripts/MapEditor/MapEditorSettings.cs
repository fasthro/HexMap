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
        [Tooltip("资源项目目录")]
        public string projectDirectory;
        
        [Tooltip("资源目录列表")]
        public string[] assetDirectorys;
    }
}