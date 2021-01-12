using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexMap.Runtime
{
    public class MapCamera : MonoBehaviour
    {
        readonly static Vector3 VP = new Vector3(0.5f, 0.5f, 0);

        public RTSCamera rts { get; private set; }
        public Camera cam { get; private set; }
        public Vector3 centerPosition => GetCenterPosition();

        private Plane _raycast;
        private float _raycastEnter;

        void Awake()
        {
            rts = GetComponent<RTSCamera>();
            cam = GetComponent<Camera>();

            _raycast = new Plane(Vector3.up, Vector3.zero);
        }

        public void MoveToCell(int x, int z)
        {
            var tp = HexMap.CellPosition(x, z);
            transform.position = tp + rts.targetOffset;
        }

        public Vector3 GetCenterPosition()
        {
            Ray worldRay = cam.ViewportPointToRay(VP);
            _raycast.Raycast(worldRay, out _raycastEnter);
            return worldRay.GetPoint(_raycastEnter);
        }
    }
}

