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

        public Text textPosition;

        #endregion

        void Awake()
        {
            drapDpwnEditorModel.onValueChanged.AddListener(OnValueChanged_EditorModel);
        }

        public void Initialize(EditorModel model)
        {
            drapDpwnEditorModel.value = (int)model;
        }

        void OnValueChanged_EditorModel(int value)
        {
            Runtime.HexMap.instance.SetEditorModel((EditorModel)value);
        }
    }
}