/*
 * @Author: fasthro
 * @Date: 2021-01-11 13:32:44
 * @Description: 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexMap.Runtime
{
    public enum EditorModel
    {
        Ground,
        Hex,
    }

    public class HexMap : MonoBehaviour
    {
        public static HexMap instance { get; private set; }

        public Ground ground;
        public HexGrid hexGrid;
        public MapCamera mapCamera;

        public Transform pickHexEffect;
        public int pickHexIndex { get; private set; }

        public Transform pickGroundEffect;
        public int pickGroundIndex { get; private set; }

        public EditorModel editorModel { get; private set; }

        void Awake()
        {
            instance = this;
            pickHexEffect.gameObject.SetActive(false);
            pickGroundEffect.gameObject.SetActive(false);
        }

        public void Initialize(bool isEditor)
        {
            ground.Initialize(isEditor);
            hexGrid.Initialize(isEditor);
            mapCamera.Initialize(isEditor);
            mapCamera.MoveToCell(750, 750);
        }

        public void SetEditorModel(EditorModel model)
        {
            editorModel = model;
            ground.SetEditorModel(model == EditorModel.Ground);
            hexGrid.SetEditorModel(model == EditorModel.Hex);
        }

        void Update()
        {

#if UNITY_EDITOR
            Debug.DrawLine(mapCamera.transform.position, mapCamera.centerPosition, Color.red);
#endif

            var chunkXZ = hexGrid.PositionToChunkXZ(mapCamera.centerPosition);
            if (chunkXZ.x > 0 && chunkXZ.y > 0)
            {
                hexGrid.Refresh(chunkXZ.x, chunkXZ.y);
            }
        }

        public void OnPickHexCell(HexCell cell)
        {
            pickHexIndex = cell.index;
            pickHexEffect.gameObject.SetActive(true);
            pickHexEffect.localPosition = cell.position;
        }

        public void OnPickGroundCell(GroundCell cell)
        {
            pickGroundIndex = cell.index;
            pickGroundEffect.gameObject.SetActive(true);
            pickGroundEffect.localPosition = cell.position;
        }

        public static Vector3 CellPosition(int x, int z)
        {
            Vector3 position;
            position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
            position.y = 0f;
            position.z = z * (HexMetrics.outerRadius * 1.5f) * -1f;
            return position;
        }
    }
}
