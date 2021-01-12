/*
 * @Author: fasthro
 * @Date: 2021-01-08 14:19:07
 * @Description: 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexMap.Runtime
{
    public class HexCell : MonoBehaviour
    {
        public int index;
        public int chunkIndex;
        public int x;
        public int z;
        public HexCoord coordinates;
        public Vector3 position;
        public readonly HexCell[] neighbors = new HexCell[6];

        public HexCell GetNeighbor(HexDirection direction)
        {
            return neighbors[(int)direction];
        }

        public void SetNeighbor(HexDirection direction, HexCell cell)
        {
            neighbors[(int)direction] = cell;
            cell.neighbors[(int)direction.Opposite()] = this;
        }
    }
}