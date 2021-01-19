/*
 * @Author: fasthro
 * @Date: 2021-01-11 13:32:44
 * @Description: 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace HexMap.Runtime
{
    public enum EditorModel : int
    {
        Hex = 0,
        Ground,
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
        
        [Tooltip("资源节点")]
        public Transform AssetsRoot;
        [Tooltip("资源加载速度（蛋帧加载数量）,默认为 chunkRowSize * chunkColumnSize")]
        public int assetLoadVelocity = 0;
        [Tooltip("资源加载间歇帧数")]
        public int assetLoadRestFrame = 5;
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
            mapCamera.MoveToCell(100, 100);

            Assets.VELOCITY = assetLoadVelocity <= 0 ? hexGrid.chunkRowSize * hexGrid.chunkColumnSize : assetLoadVelocity;
            Assets.REST_FRAME = assetLoadRestFrame;
        }

        public void SetEditorModel(EditorModel model)
        {
            editorModel = model;

            pickHexIndex = -1;
            pickHexEffect.gameObject.SetActive(false);

            pickGroundIndex = -1;
            pickGroundEffect.gameObject.SetActive(false);

            ground.SetEditorModel(model == EditorModel.Ground);
            hexGrid.SetEditorModel(model == EditorModel.Hex);

            var angles = mapCamera.transform.localEulerAngles;
            angles.x = model == EditorModel.Hex ? 45 : 90;
            mapCamera.transform.localEulerAngles = angles;

            mapCamera.cam.fieldOfView = model == EditorModel.Hex ? 16 : 30;
        }

        void Update()
        {
            Assets.Update();
            
#if UNITY_EDITOR
            Debug.DrawLine(mapCamera.transform.position, mapCamera.centerPosition, Color.red);
#endif

            var chunkXZ = hexGrid.PositionToChunkXZ(mapCamera.centerPosition);
            if (chunkXZ.x > 0 && chunkXZ.y > 0)
            {
                hexGrid.Refresh(chunkXZ.x, chunkXZ.y);
            }
        }

        public void RefreshChunks()
        {
            var chunkXZ = hexGrid.PositionToChunkXZ(mapCamera.centerPosition);
            if (chunkXZ.x > 0 && chunkXZ.y > 0)
            {
                hexGrid.ForeRefresh(chunkXZ.x, chunkXZ.y);
            }
        }

        public void RefreshChunk(int xChunk, int zChunk)
        {
            hexGrid.RefreshChunk(hexGrid.XZToChunkIndex(xChunk, zChunk));
        }
        
        public void RefreshChunk(int chunkIndex)
        {
            hexGrid.RefreshChunk(chunkIndex);
        }

        public void RefreshCell(int cellIndex)
        {
            var xz = hexGrid.IndexToCellXZ(cellIndex);
            hexGrid.RefreshCell(cellIndex);
        }
        
        public void RefreshCell(int xCell, int zCell)
        {
            hexGrid.RefreshCell(hexGrid.XZToCellIndex(xCell, zCell));
        }

        public void OnPickHexCell(HexCell cell)
        {
            pickHexIndex = cell.index;
            pickHexEffect.gameObject.SetActive(true);
            pickHexEffect.localPosition = cell.position;
            EditorUI.instance.main.SetSelected(true, cell.index);
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
