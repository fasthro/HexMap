/*
 * @Author: fasthro
 * @Date: 2021-01-08 14:29:11
 * @Description: 
 */
using UnityEngine;
using System.Collections.Generic;
using System;

namespace HexMap.Runtime
{
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class HexMesh : MonoBehaviour
    {
        public MeshFilter meshFilter { get; private set; }
        public Mesh mesh { get; private set; }
        public MeshRenderer meshRenderer { get; private set; }

        private List<Vector3> _vertices = new List<Vector3>();
        private List<int> _triangles = new List<int>();
        private List<Vector2> _uvs = new List<Vector2>();

        void Awake()
        {
            mesh = new Mesh();
            meshFilter = gameObject.GetComponent<MeshFilter>();
            meshFilter.mesh = mesh;
            meshRenderer = gameObject.GetComponent<MeshRenderer>();
        }

        public void AddSingleVertice(Vector3 v)
        {
            _vertices.Add(v);
        }

        public void AddSingleTriangle(int index)
        {
            _triangles.Add(index);
        }

        public void AddSingleUV(Vector2 uv)
        {
            _uvs.Add(uv);
        }

        public void AddTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int index = _vertices.Count;
            _vertices.Add(v1);
            _vertices.Add(v2);
            _vertices.Add(v3);
            _triangles.Add(index);
            _triangles.Add(index + 1);
            _triangles.Add(index + 2);
        }

        public void AddPerturbTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
        {
            int index = _vertices.Count;
            _vertices.Add(HexMetrics.Perturb(v1));
            _vertices.Add(HexMetrics.Perturb(v2));
            _vertices.Add(HexMetrics.Perturb(v3));
            _triangles.Add(index);
            _triangles.Add(index + 1);
            _triangles.Add(index + 2);
        }

        public void AddUV(Vector2 uv1, Vector2 uv2, Vector3 uv3)
        {
            _uvs.Add(uv1);
            _uvs.Add(uv2);
            _uvs.Add(uv3);
        }

        public void DrawMesh()
        {
            mesh.SetVertices(_vertices);
            mesh.SetUVs(0, _uvs);
            mesh.SetTriangles(_triangles, 0);
            mesh.RecalculateNormals();
        }
    }
}