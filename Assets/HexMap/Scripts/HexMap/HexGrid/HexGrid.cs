/*
 * @Author: fasthro
 * @Date: 2021-01-07 16:56:09
 * @Description: 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HexMap.Runtime
{
    public class HexGrid : MonoBehaviour
    {
        [SerializeField] public int cellRowCount;
        [SerializeField] public int cellColumnCount;

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
            chunkRowCount = cellRowCount / chunkRowSize;
            chunkColumnCount = cellColumnCount / chunkColumnSize;
            chunks = new HexChunk[chunkRowCount * chunkColumnCount];

            // cells
            cells = new HexCell[cellRowCount * cellColumnCount];
        }

        public void SetEditorModel(bool editor)
        {
            gameObject.SetActive(editor);
        }

        public void RefreshCell(int cellIndex)
        {
            if (cells[cellIndex] != null)
                cells[cellIndex].SetActive(true);
        }

        public void RefreshChunk(int chunkIndex)
        {
            var chunk = chunks[chunkIndex];
            if (chunk.isActive)
            {
                foreach (var cell in chunk.cells)
                    cell.SetActive(true);
            }
        }

        public void ForeRefresh(int xChunk, int zChunk)
        {
            activeChunkX = -1;
            activeChunkZ = -1;

            foreach (var index in _chunkActives)
            {
                PoolRecycleHexChunk(chunks[index]);
            }

            _chunkActives.Clear();

            Refresh(xChunk, zChunk);
        }

        public void Refresh(int xChunk, int zChunk)
        {
            if (activeChunkX == xChunk && activeChunkZ == zChunk)
                return;

            _chunkActives2.Clear();

            activeChunkX = xChunk;
            activeChunkZ = zChunk;

            var x = xChunk - 1;
            var z = zChunk - 1;
            var mx = x + 3 > chunkRowCount ? chunkRowCount : x + 3;
            var mz = z + 3 > chunkColumnCount ? chunkColumnCount : z + 3;

            // 优先处理当前视野Chunk
            var priorityChunkIndex = -1;
            if (xChunk < mx && zChunk < mz)
            {
                priorityChunkIndex = xChunk + zChunk * chunkRowCount;
                if (priorityChunkIndex < chunks.Length)
                {
                    PoolGetHexChunk(xChunk, zChunk, priorityChunkIndex);
                    WakeupChunk(priorityChunkIndex);
                    _chunkActives2.Add(priorityChunkIndex);
                }
            }

            for (var tz = z; tz < mz; tz++)
            {
                for (var tx = x; tx < mx; tx++)
                {
                    if (tx >= 0 && tz >= 0)
                    {
                        var index = tx + tz * chunkRowCount;
                        if (index < chunks.Length)
                        {
                            if (priorityChunkIndex != index)
                            {
                                PoolGetHexChunk(tx, tz, index);
                                WakeupChunk(index);
                                _chunkActives2.Add(index);
                            }
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
            chunk.cells.Clear();

            var sx = chunk.x * chunkRowSize;
            var sz = chunk.z * chunkColumnSize;
            var msx = sx + chunkColumnSize > cellRowCount ? cellRowCount : sx + chunkColumnSize;
            var msz = sz + chunkColumnSize > cellRowCount ? cellRowCount : sz + chunkColumnSize;
            for (int tz = sz; tz < msz; tz++)
            {
                for (int tx = sx; tx < msx; tx++)
                {
                    chunk.AddHexCell(PoolGetHexCell(tx, tz, chunkIndex));
                }
            }

            // Debug.Log($"Active Chunk: {chunkIndex}");
        }

        private HexChunk PoolGetHexChunk(int x, int z, int index)
        {
            HexChunk chunk = null;
            if (chunks[index] != null)
            {
                chunk = chunks[index];
            }

            if (chunk == null)
            {
                if (_chunkStack.Count <= 0)
                {
                    chunk = chunks[index] = GameObject.Instantiate<HexChunk>(hexChunk);
                }
                else
                {
                    chunk = chunks[index] = _chunkStack.Pop();
                }
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
            int index = x + z * cellRowCount;
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

            HexCell cell = null;
            if (cells[index] != null)
            {
                cell = cells[index];
            }

            if (cell == null)
            {
                if (_cellStack.Count <= 0)
                {
                    cell = cells[index] = GameObject.Instantiate<HexCell>(hexCell);
                }
                else
                {
                    cell = cells[index] = _cellStack.Pop();
                }
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

            cell.SetActive(true);

            return cell;
        }

        private void PoolRecycleHexCell(HexCell cell)
        {
            cells[cell.index] = null;
            cell.SetActive(false);
            cell.transform.SetParent(garbageRoot);
            _cellStack.Push(cell);
        }
        
        public int CoordToCellIndex(HexCoord coord)
        {
            return coord.q + coord.r * cellRowCount + coord.r / 2;
        }

        public int PositionToCellIndex(Vector3 position)
        {
            var coord = HexCoord.AtPosition(new Vector2(position.x, Mathf.Abs(position.z)));
            return coord.q + coord.r * cellRowCount + coord.r / 2;
        }

        public Vector2Int PositionToCellXZ(Vector3 position)
        {
            var index = PositionToCellIndex(position);
            return new Vector2Int(index % cellRowCount, index / cellColumnCount);
        }

        public Vector2Int IndexToCellXZ(int index)
        {
            return new Vector2Int(index % cellRowCount, index / cellColumnCount);
        }

        public int XZToCellIndex(int xCell, int zCell)
        {
            return xCell + zCell * cellRowCount;
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

        public Vector2Int IndexToChunkXZ(int index)
        {
            return new Vector2Int(index % chunkRowCount, index / chunkColumnCount);
        }

        public int XZToChunkIndex(int xChunk, int zChunk)
        {
            return xChunk + zChunk * chunkRowCount;
        }
    }
}