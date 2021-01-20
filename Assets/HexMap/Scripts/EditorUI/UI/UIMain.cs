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
        #region top

        public Dropdown drapDpwnEditorModel;

        public InputField inputX;
        public InputField inputY;
        public Button btnGoto;
        public Button btnSave;

        #endregion top

        #region minimap

        public RectTransform planeRect;
        public Transform pointer;
        public Text textPosition;

        #endregion minimap

        #region assets

        public Dropdown drapDpwnAssetType;
        public RectTransform gridRect;
        public GameObject gridrecord;
        private List<GameObject> gridList = new List<GameObject>();
        public GameObject preView;
        public Transform roadRoot;
        public Transform terrainRoot;
        private List<Asset> showAssets = new List<Asset>();
        private Toggle selectedToggle;

        #endregion assets

        #region info

        public Transform selectInfoRoot;
        public Text textSelectInfo;

        #endregion info

        #region opt

        public Transform optRoot;
        public Button btnReplace;
        public Button btnRestore;

        #endregion opt

        private MapCamera mapCamera;
        private HexGrid hexGrid;

        private bool _isSelectedHexCell;
        private bool _isSelectedAsset;
        private Asset _selectedAsset;
        private int _selectedCellIndex;

        private void Awake()
        {
            drapDpwnEditorModel.onValueChanged.AddListener(OnValueChanged_EditorModel);
            btnGoto.onClick.AddListener(OnGotoButtonClick);
            btnSave.onClick.AddListener(OnSaveButtonClick);
            drapDpwnAssetType.onValueChanged.AddListener(OnValueChanged_DrapDpwnAssetType);
            selectInfoRoot.gameObject.SetActive(false);

            btnReplace.onClick.AddListener(OnReplaceButtonClick);
            btnRestore.onClick.AddListener(OnRestoreButtonClick);
            optRoot.gameObject.SetActive(false);
        }

        private void Start()
        {
            mapCamera = Runtime.HexMap.instance.mapCamera;
            hexGrid = Runtime.HexMap.instance.hexGrid;
        }

        public void Initialize(EditorModel model)
        {
            _isSelectedHexCell = false;
            _isSelectedAsset = false;
            drapDpwnAssetType.options = MapEditor.instance.assetsSettings.GetDropdownAssetTypes();
            drapDpwnAssetType.SetValueWithoutNotify(0);
            OnValueChanged_DrapDpwnAssetType(0);
            drapDpwnEditorModel.value = (int)model;
        }

        private void OnValueChanged_EditorModel(int value)
        {
            Runtime.HexMap.instance.SetEditorModel((EditorModel)value);
        }

        private void OnValueChanged_DrapDpwnAssetType(int value)
        {
            if (selectedToggle)
            {
                selectedToggle.SetIsOnWithoutNotify(false);
            }
            _isSelectedAsset = false;
            SetOpt();
            preView.SetActive(false);
            ShowAssetList(drapDpwnAssetType.captionText.text);
        }

        private void OnGotoButtonClick()
        {
            int cellX;
            int cellY;
            if (int.TryParse(inputX.text, out cellX) && int.TryParse(inputY.text, out cellY))
            {
                Runtime.HexMap.instance.mapCamera.MoveToCell(cellX, cellY);
                var currPos = Runtime.HexMap.instance.mapCamera.centerPosition;
                var chunkXZ = Runtime.HexMap.instance.hexGrid.PositionToCellXZ(currPos);
            }
        }

        private void Update()
        {
            RefreshMinmpaShow();

            if (Input.GetMouseButtonDown(0))
            {
                Vector2 pos;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(planeRect, Input.mousePosition, null, out pos))
                {
                    UIPositionToCellXZAndGoto(pos);
                }
            }
        }

        private void RefreshMinmpaShow()
        {
            var currPos = mapCamera.centerPosition;
            var chunkXZ = hexGrid.PositionToCellXZ(currPos);
            RectTransform rect = pointer.parent.GetComponent<RectTransform>();
            float posX = chunkXZ.x * rect.rect.width / hexGrid.cellRowCount - rect.rect.width / 2;
            float posY = chunkXZ.y * rect.rect.height / hexGrid.cellColumnCount - rect.rect.height / 2;
            pointer.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(posX, -posY, 0);
            textPosition.text = string.Format("行:{0} 列:{1}", chunkXZ.x, chunkXZ.y);
        }

        private void UIPositionToCellXZAndGoto(Vector2 pos)
        {
            if (pos.x < planeRect.rect.width / 2 && pos.x > -planeRect.rect.width / 2)
            {
                if (pos.y < planeRect.rect.height / 2 && pos.y > -planeRect.rect.height / 2)
                {
                    int cellX = (int)((pos.x + planeRect.rect.width / 2) / planeRect.rect.width * hexGrid.cellRowCount);
                    int cellY = (int)(-(pos.y - planeRect.rect.height / 2) / planeRect.rect.height * hexGrid.cellColumnCount);

                    Runtime.HexMap.instance.mapCamera.MoveToCell(cellX, cellY);
                }
            }
        }

        public void ShowAssetList(string typeName)
        {
            showAssets = MapEditor.instance.assetsSettings.GetAssetListByTypeName(typeName);
            for (int i = 0; i < gridList.Count; i++)
            {
                gridList[i].SetActive(false);
            }

            for (int i = 0; i < showAssets.Count; i++)
            {
                if (i < gridList.Count)
                {
                    gridList[i].SetActive(true);
                    gridList[i].GetComponentInChildren<Text>().text = showAssets[i].name;
                }
                else
                {
                    GenerateGrids(i, showAssets[i].name);
                }
            }
        }

        public void GenerateGrids(int index, string showName)
        {
            GameObject go = Instantiate(gridrecord, gridRect);
            go.GetComponent<Toggle>().isOn = false;
            gridList.Add(go);
            go.name = index.ToString();
            go.GetComponentInChildren<Text>().text = showName;
            go.GetComponent<Toggle>().onValueChanged.AddListener((isOn) =>
            {
                OnToggleChanged(isOn, int.Parse(go.name));
                selectedToggle = go.GetComponent<Toggle>();
            });
            go.SetActive(true);
        }

        private void OnToggleChanged(bool isOn, int index)
        {
            _isSelectedAsset = isOn;
            preView.SetActive(isOn);
            if (isOn)
            {
                for (int i = 0; i < roadRoot.childCount; i++)
                {
                    Destroy(roadRoot.GetChild(i).gameObject);
                }

                for (int i = 0; i < terrainRoot.childCount; i++)
                {
                    Destroy(terrainRoot.GetChild(i).gameObject);
                }

                _selectedAsset = showAssets[index];
                if (drapDpwnAssetType.captionText.text == "Road")
                {
                    Instantiate(_selectedAsset.gameObject, roadRoot);
                }
                else if (drapDpwnAssetType.captionText.text == "Terrain")
                {
                    Instantiate(_selectedAsset.gameObject, terrainRoot);
                }
            }
            else _selectedAsset = null;

            SetOpt();
        }

        public void SetSelected(bool isSelected, int cellIndex)
        {
            _isSelectedHexCell = isSelected;
            selectInfoRoot.gameObject.SetActive(isSelected);
            if (isSelected)
            {
                _selectedCellIndex = cellIndex;
                var xz = Runtime.HexMap.instance.hexGrid.IndexToCellXZ(cellIndex);
                textSelectInfo.text = $"焦点坐标 ({xz.x}, {xz.y}) - 地形（山地）-  等级（5）- 无覆盖物";
            }
            else _selectedCellIndex = -1;

            SetOpt();
        }

        private void SetOpt()
        {
            var isOn = _isSelectedHexCell && _isSelectedAsset;
            optRoot.gameObject.SetActive(isOn);
        }

        private void OnReplaceButtonClick()
        {
            if (_selectedAsset != null && _selectedCellIndex > -1)
            {
                MapEditor.instance.mapParser.SetDataWithId(MapLayerType.Prefab, _selectedCellIndex, _selectedAsset.index);
                Runtime.HexMap.instance.RefreshCell(_selectedCellIndex);
            }
        }

        private void OnRestoreButtonClick()
        {
            if (_selectedAsset != null && _selectedCellIndex > -1)
            {
                var value = MapEditor.instance.mapParser.GetOriginalDataWithId(MapLayerType.Prefab, _selectedCellIndex);
                MapEditor.instance.mapParser.SetDataWithId(MapLayerType.Prefab, _selectedCellIndex, value);
                Debug.Log("----------------> " + value);
                Runtime.HexMap.instance.RefreshCell(_selectedCellIndex);
            }
        }

        private void OnSaveButtonClick()
        {
            MapEditor.instance.SaveMapDataToXml();
        }
    }
}