/*
 * @Author: fasthro
 * @Date: 2021-01-11 13:32:44
 * @Description: 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexMap.Runtime
{
    public class HexMap : MonoBehaviour
    {
        public Ground ground;
        public HexGrid hexGrid;
        public MapCamera mapCamera;

        void Start()
        {
            hexGrid.Initialize();
            mapCamera.MoveToCell(750, 750);
        }

        void Update()
        {

#if UNITY_EDITOR
            Debug.DrawLine(mapCamera.transform.position, mapCamera.centerPosition, Color.red);
#endif

            var chunkXZ = hexGrid.PositionToChunkXZ(mapCamera.centerPosition);
            if (chunkXZ.x > 0 && chunkXZ.y > 0)
            {
                hexGrid.Refresh(chunkXZ.x, chunkXZ.y);
            }
        }

        public static Vector3 CellPosition(int x, int z)
        {
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f) * -1f;
            return position;
        }
    }
}
