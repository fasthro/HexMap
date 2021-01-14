using System.Collections;
using System.Collections.Generic;
using BitBenderGames;
using UnityEngine;

namespace HexMap.Runtime
{
    public class MapCamera : MonoBehaviour
    {
        public float keyboardMovementSpeed = 5f;

        readonly static Vector3 VIEW_CENTER_POINT = new Vector3(0.5f, 0.5f, 0);

        public Camera cam { get; private set; }
        public TouchInputController touchInputController { get; private set; }
        public MobileTouchCamera mobileTouchCamera { get; private set; }
        public MobilePickingController mobilePickingController { get; private set; }

        public Vector3 centerPosition => GetViewPosition(VIEW_CENTER_POINT);

        public Vector2 keyboardInput
        {
            get { return new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")); }
        }

        private Plane _raycast;
        private float _raycastEnter;

        void Awake()
        {
            cam = GetComponent<Camera>();
            mobileTouchCamera = cam.GetComponent<MobileTouchCamera>();
            touchInputController = cam.GetComponent<TouchInputController>();
            mobilePickingController = cam.GetComponent<MobilePickingController>();

            mobilePickingController.SnapUnitSize = 0.1f;
            _raycast = new Plane(Vector3.up, Vector3.zero);
        }

        public void Initialize(bool isEditor)
        {

        }

        void Update()
        {
            UpdateKeyboardInput();
        }

        #region touch camera

        private void UpdateKeyboardInput()
        {
#if UNITY_EDITOR
            if (keyboardInput.x != 0 || keyboardInput.y != 0)
            {
                Vector3 desiredMove = new Vector3(keyboardInput.x, 0, keyboardInput.y);

                desiredMove *= keyboardMovementSpeed;
                desiredMove *= Time.deltaTime;
                desiredMove = Quaternion.Euler(new Vector3(0f, transform.eulerAngles.y, 0f)) * desiredMove;
                desiredMove = transform.InverseTransformDirection(desiredMove);

                transform.Translate(desiredMove, Space.Self);
            }
#endif
        }

        public void OnPickItem(RaycastHit hitInfo)
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

        public void OnPickableTransformSelected(Transform pickableTransform)
        {
        }

        public void OnPickableTransformSelectedExtended(PickableSelectedData data)
        {
        }

        public void OnPickableTransformDeselected(Transform pickableTransform)
        {
        }

        public void OnPickableTransformMoveStarted(Transform pickableTransform)
        {
        }

        public void OnPickableTransformMoved(Transform pickableTransform)
        {
        }

        public void OnPickableTransformMoveEnded(Vector3 startPos, Transform pickableTransform)
        {

        }
        #endregion

        public void MoveToCell(int x, int z)
        {
            // 45°角
            // var tp = HexMap.CellPosition(x, z);
            // tp.y = transform.position.y;
            // tp.z -= Mathf.Sqrt((_raycastEnter * _raycastEnter) / 2f);
            // transform.position = tp;

            // 任意角度
            var tp = HexMap.CellPosition(x, z);
            var dir = (transform.position - GetViewPosition(VIEW_CENTER_POINT)).normalized;
            var offset = dir * _raycastEnter;
            transform.position = tp + offset;
        }

        private Vector3 GetViewPosition(Vector3 point)
        {
            Ray worldRay = cam.ViewportPointToRay(point);
            _raycast.Raycast(worldRay, out _raycastEnter);
            return worldRay.GetPoint(_raycastEnter);
        }
    }
}

