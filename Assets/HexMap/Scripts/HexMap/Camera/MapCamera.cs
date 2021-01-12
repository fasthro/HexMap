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

            rts.onClick.AddListener(OnClick);
            rts.onPick.AddListener(OnPick);
            rts.onFreeOver.AddListener(OnOver);

            _raycast = new Plane(Vector3.up, Vector3.zero);
        }

        public void Initialize(bool isEditor)
        {

        }

        private void OnClick(Vector3 clickPosition, bool isDoubleClick, bool isLongTap)
        {

        }

        private void OnPick(RaycastHit hitInfo)
        {
            var hexcell = hitInfo.transform.gameObject.GetComponent<HexCell>();
            if (hexcell != null)
            {
                HexMap.instance.OnPickHexCell(hexcell);
            }

            var groundcell = hitInfo.transform.gameObject.GetComponent<GroundCell>();
            if (groundcell != null)
            {
                HexMap.instance.OnPickGroundCell(groundcell);
            }
        }

        private void OnOver(Vector3 overPosition)
        {

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

