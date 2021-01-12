/*
 * @Author: fasthro
 * @Date: 2021-01-07 16:56:09
 * @Description: 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexMap.Runtime
{
    public class HexGrid : MonoBehaviour
    {
        [SerializeField] public int gridRowCount;
        [SerializeField] public int gridColumnCount;

        [SerializeField] public int chunkRowSize;
        [SerializeField] public int chunkColumnSize;

        [SerializeField] public HexChunk hexChunk;
        [SerializeField] public HexCell hexCell;
        [SerializeField] public Transform garbageRoot;

        public int chunkRowCount { get; private set; }
        public int chunkColumnCount { get; private set; }
        public HexCell[] cells { get; private set; }
        public HexChunk[] chunks { get; private set; }

        public int activeChunkX { get; private set; }
        public int activeChunkZ { get; private set; }

        private HashSet<int> _chunkActives = new HashSet<int>();
        private HashSet<int> _chunkActives2 = new HashSet<int>();

        private Stack<HexCell> _cellStack = new Stack<HexCell>();
        private Stack<HexChunk> _chunkStack = new Stack<HexChunk>();

        private bool _isEditor;

        public void Initialize(bool isEditor)
        {
            _isEditor = isEditor;
            garbageRoot.gameObject.SetActive(false);

            // chunks
            activeChunkX = activeChunkX = -1;
            chunkRowCount = gridRowCount / chunkRowSize;
            chunkColumnCount = gridColumnCount / chunkColumnSize;
            chunks = new HexChunk[chunkRowCount * chunkColumnCount];

            // cells
            cells = new HexCell[gridRowCount * gridColumnCount];
        }

        public void SetEditorModel(bool editor)
        {
            gameObject.SetActive(editor);
        }

        public void Refresh(int xChunk, int zChunk)
        {
            if (activeChunkX == xChunk && activeChunkZ == zChunk)
                return;

            _chunkActives2.Clear();

            activeChunkX = xChunk;
            activeChunkZ = zChunk;

            int x = xChunk - 1;
            int z = zChunk - 1;
            for (int tz = z; tz < z + 3; tz++)
            {
                for (int tx = x; tx < x + 3; tx++)
                {
                    if (tx >= 0 && tz >= 0)
                    {
                        var index = tx + tz * chunkRowCount;
                        if (index < chunks.Length)
                        {
                            // Debug.Log($"tx:{tx} tz:{tz} index:{index}");
                            PoolGetHexChunk(tx, tz, index);
                            WakeupChunk(index);
                            _chunkActives2.Add(index);
                        }
                    }
                }
            }
            foreach (var index in _chunkActives)
            {
                if (!_chunkActives2.Contains(index))
                {
                    PoolRecycleHexChunk(chunks[index]);
                }
            }

            _chunkActives.Clear();
            foreach (var index in _chunkActives2)
            {
                _chunkActives.Add(index);
            }
        }

        private void WakeupChunk(int chunkIndex)
        {
            var chunk = chunks[chunkIndex];
            if (chunk == null || chunk.isActive)
                return;

            chunk.isActive = true;

            var sx = chunk.x * chunkRowSize;
            var sz = chunk.z * chunkColumnSize;
            for (int tz = sz; tz < sz + chunkColumnSize; tz++)
            {
                for (int tx = sx; tx < sx + chunkRowSize; tx++)
                {
                    chunk.AddHexCell(PoolGetHexCell(tx, tz, chunkIndex));
                }
            }
        }

        private HexChunk PoolGetHexChunk(int x, int z, int index)
        {
            if (chunks[index] != null)
                return chunks[index];

            HexChunk chunk;
            if (_chunkStack.Count <= 0)
            {
                chunk = chunks[index] = GameObject.Instantiate<HexChunk>(hexChunk);
            }
            else
            {
                chunk = chunks[index] = _chunkStack.Pop();
            }
            chunk.index = index;
            chunk.x = x;
            chunk.z = z;

            chunk.gameObject.transform.SetParent(transform);
            chunk.gameObject.transform.localPosition = Vector3.zero;
            chunk.gameObject.name = "chunk_" + index;
            chunk.gameObject.SetActive(true);

            return chunk;
        }

        private void PoolRecycleHexChunk(HexChunk chunk)
        {
            // Debug.Log($"Recycle Chunk: {chunk.index}");
            chunks[chunk.index] = null;

            chunk.isActive = false;
            chunk.gameObject.transform.SetParent(garbageRoot);

            foreach (var cell in chunk.cells)
                PoolRecycleHexCell(cell);
            chunk.cells.Clear();

            _chunkStack.Push(chunk);
        }

        private HexCell PoolGetHexCell(int x, int z, int chunkIndex)
        {
            int index = x + z * gridRowCount;
            if (index >= cells.Length)
            {
                Debug.LogError($"HexCell Index > Cells3.Length x: {x} z:{z} index:{index} chunkIndex:{chunkIndex}");
                return null;
            }

            if (x >= 1500 || z > 1500)
            {
                Debug.LogError($"HexCell x/z >= 1500.");
                return null;
            }

            if (cells[index] != null)
                return cells[index];

            HexCell cell;
            if (_cellStack.Count <= 0)
            {
                cell = cells[index] = GameObject.Instantiate<HexCell>(hexCell);
            }
            else
            {
                cell = cells[index] = _cellStack.Pop();
            }

            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f) * -1f;

            cell.coordinates = HexCoord.AtOffset(x, z);
            cell.position = position;
            cell.index = index;
            cell.x = x;
            cell.z = z;
            cell.chunkIndex = chunkIndex;

            cell.transform.SetParent(chunks[chunkIndex].transform);
            cell.transform.localPosition = position;
            return cell;
        }

        private void PoolRecycleHexCell(HexCell cell)
        {
            cells[cell.index] = null;
            cell.transform.SetParent(garbageRoot);
            _cellStack.Push(cell);
        }

        public int PositionToCellIndex(Vector3 position)
        {
            var coord = HexCoord.AtPosition(new Vector2(position.x, Mathf.Abs(position.z)));
            return coord.q + coord.r * gridRowCount + coord.r / 2;
        }

        public Vector2Int PositionToCellXZ(Vector3 position)
        {
            var index = PositionToCellIndex(position);
            return new Vector2Int(index % gridRowCount, index / gridColumnCount);
        }

        public Vector2Int PositionToChunkXZ(Vector3 position)
        {
            var cellXZ = PositionToCellXZ(position);
            int chunkX = cellXZ.x / chunkRowSize;
            int chunkZ = cellXZ.y / chunkColumnSize;
            return new Vector2Int(chunkX, chunkZ);
        }

        public int PositionToChunkIndex(Vector3 position)
        {
            var chunkXZ = PositionToChunkXZ(position);
            return chunkXZ.x + chunkXZ.y * chunkRowCount;
        }
    }
}
