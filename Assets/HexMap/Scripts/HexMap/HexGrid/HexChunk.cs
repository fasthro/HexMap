using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexMap.Runtime
{
    public class HexChunk : MonoBehaviour
    {
        public int row;
        public int column;

        public int x { get; set; }
        public int z { get; set; }
        public int index { get; set; }
        public bool isActive { get; set; }
        public List<HexCell> cells { get; private set; }

        void Awake()
        {
            cells = new List<HexCell>();
        }

        public void AddHexCell(HexCell cell)
        {
            if (cell == null) return;
            cells.Add(cell);
            cell.hexObject.transform.SetParent(transform, false);
            cell.chunkIndex = index;
        }
    }
}
