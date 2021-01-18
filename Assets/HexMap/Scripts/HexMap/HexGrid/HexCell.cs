/*
 * @Author: fasthro
 * @Date: 2021-01-08 14:19:07
 * @Description: 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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

        private readonly HexCell[] neighbors = new HexCell[6];

        private PoolIdentity _displayObject;

        public HexCell GetNeighbor(HexDirection direction)
        {
            return neighbors[(int) direction];
        }

        public void SetNeighbor(HexDirection direction, HexCell cell)
        {
            neighbors[(int) direction] = cell;
            cell.neighbors[(int) direction.Opposite()] = this;
        }

        public void SetActive(bool active)
        {
            if (_displayObject != null)
                MapEditor.instance.RecycleGameObject(_displayObject);

            if (active)
            {
                _displayObject = MapEditor.instance.GetGameObject(index);
                if (_displayObject != null)
                {
                    _displayObject.transform.SetParent(transform);
                    _displayObject.transform.localPosition = Vector3.zero;
                }
            }
        }
    }
}