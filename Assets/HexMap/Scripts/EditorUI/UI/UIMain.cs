/*
 * @Author: fasthro
 * @Date: 2021-01-13 11:43:00
 * @Description: 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using HexMap.Runtime;

namespace HexMap
{
    public class UIMain : MonoBehaviour
    {
        #region  top

        public Dropdown drapDpwnEditorModel;

        public InputField inputX;
        public InputField inputY;
        public Button btnGoto;

        public Button btnSave;

        #endregion

        #region bottom
        #endregion

        #region minimap
        public Transform pointer;
        public Text textPosition;

        #endregion
        MapCamera mapCamera;
        HexGrid hexGrid;
        void Awake()
        {
            drapDpwnEditorModel.onValueChanged.AddListener(OnValueChanged_EditorModel);
            btnGoto.onClick.AddListener(OnGotoButtonClick);
        }
        private void Start()
        {
            mapCamera = Runtime.HexMap.instance.mapCamera;
            hexGrid = Runtime.HexMap.instance.hexGrid;
        }

        public void Initialize(EditorModel model)
        {
            drapDpwnEditorModel.value = (int)model;
        }

        void OnValueChanged_EditorModel(int value)
        {
            Runtime.HexMap.instance.SetEditorModel((EditorModel)value);
        }
        void OnGotoButtonClick() 
        {
            int cellX;
            int cellY;
            if (int.TryParse(inputX.text,out cellX) && int.TryParse(inputY.text, out cellY))
            {
                Runtime.HexMap.instance.mapCamera.MoveToCell(cellX, cellY);
                var currPos = Runtime.HexMap.instance.mapCamera.GetCenterPosition();
                var chunkXZ = Runtime.HexMap.instance.hexGrid.PositionToCellXZ(currPos);
                Debug.Log(chunkXZ);
            }
        }
        private void Update()
        {
            RefreshMinmpaShow();
        }

        private void RefreshMinmpaShow() 
        {
            var currPos = mapCamera.GetCenterPosition();
            var chunkXZ = hexGrid.PositionToCellXZ(currPos);
            RectTransform rect = pointer.parent.GetComponent<RectTransform>();
            float posX = chunkXZ.x * rect.rect.width / hexGrid.gridRowCount - rect.rect.width / 2;
            float posY = chunkXZ.y * rect.rect.height / hexGrid.gridColumnCount - rect.rect.height / 2;
            pointer.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(posX, -posY, 0);
            textPosition.text = string.Format("X:{0} Y:{1}", chunkXZ.x, chunkXZ.y);
        }
    }
}