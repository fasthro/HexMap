/*
 * @Author: fasthro
 * @Date: 2021-01-07 11:52:13
 * @Description: 
 */
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HexMap.Runtime
{
    public class Ground : MonoBehaviour
    {
        [SerializeField] public int row;
        [SerializeField] public int column;
        [SerializeField] public float size = 1f;
        [SerializeField] public Vector2 offset = Vector2.one;
        [SerializeField] public TextAsset tileFile;
        [SerializeField] public Material[] materials;

        public int count { get; private set; }
        public MeshFilter meshFilter { get; private set; }
        public Mesh mesh { get; private set; }
        public MeshRenderer meshRenderer { get; private set; }

        private List<Vector3> _vertices;
        private int[] _triangles;
        private Vector2[] _uv;

        private int[] _tiles;
        private Dictionary<int, List<int>> _subMeshTriangleDict = new Dictionary<int, List<int>>();

        void Awake()
        {
            mesh = new Mesh();
            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.materials = new Material[materials.Length];
            transform.localPosition = new Vector3(offset.x, 0, offset.y);
        }

        void Start()
        {
            Initialize();
            DrawMesh();
        }

        public void Initialize()
        {
            count = row * column;

            // tile
            _tiles = new int[count];
            var tds = tileFile.text.Split(',');
            for (int i = 0; i < count; i++)
            {
                if (i < tds.Length)
                {
                    var tile = int.Parse(tds[i]);
                    if (tile < materials.Length)
                        _tiles[i] = tile;
                    else
                        _tiles[i] = 0;
                }
                else _tiles[i] = 0;
            }

            // submesh
            _subMeshTriangleDict.Clear();
            for (int i = 0; i < count; i++)
            {
                var key = _tiles[i];
                if (!_subMeshTriangleDict.ContainsKey(key))
                {
                    _subMeshTriangleDict.Add(key, new List<int>());
                }
            }

            // tiling
            Vector2 textureScale = new Vector2(row, column);
            foreach (var mat in materials)
            {
                mat.SetTextureScale("_MaskTex", textureScale);
            }
        }

        public void DrawMesh()
        {
            _uv = new Vector2[(row + 1) * (column + 1)];
            _vertices = new List<Vector3>(_uv.Length);
            for (int index = 0, y = 0; y <= column; y++)
            {
                for (int x = 0; x <= row; x++, index++)
                {
                    // x:z
                    // _vertices.Add(new Vector3(x * scale, 0, y * scale * -1f));

                    // x:-z
                    _vertices.Add(new Vector3(x * size, 0, y * size * -1f));

                    _uv[index] = new Vector2((float)x / (float)row, (float)y / (float)column);
                }
            }

            mesh.SetVertices(_vertices);
            mesh.uv = _uv;

            _triangles = new int[row * column * 6];
            for (int ti = 0, vi = 0, y = 0; y < column; y++, vi++)
            {
                for (int x = 0; x < row; x++, ti += 6, vi++)
                {
                    // x:z
                    // _triangles[ti] = vi;
                    // _triangles[ti + 3] = _triangles[ti + 2] = vi + 1;
                    // _triangles[ti + 4] = _triangles[ti + 1] = vi + width + 1;
                    // _triangles[ti + 5] = vi + width + 2;

                    // x:-z
                    _triangles[ti + 1] = vi;
                    _triangles[ti + 2] = _triangles[ti + 3] = vi + 1;
                    _triangles[ti + 4] = vi + row + 2;
                    _triangles[ti] = _triangles[ti + 5] = vi + row + 1;
                }
            }

            mesh.subMeshCount = materials.Length;
            for (int i = 0; i < count; i++)
            {
                _subMeshTriangleDict[_tiles[i]].AddRange(GetTriangleArray(i));
            }

            int subIndex = 0;
            foreach (var item in _subMeshTriangleDict)
            {
                var key = subIndex++;
                mesh.SetIndices(item.Value.ToArray(), MeshTopology.Triangles, key);
            }
            meshRenderer.materials = materials;
            mesh.RecalculateNormals();
        }

        private int[] GetTriangleArray(int index)
        {
            int start = index * 6;
            int end = start + 6;
            int[] triangle = new int[6];
            for (int ti = 0, i = start; i < end; i++, ti++)
                triangle[ti] = _triangles[i];
            return triangle;
        }
    }
}