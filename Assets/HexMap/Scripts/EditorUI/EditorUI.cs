/*
 * @Author: fasthro
 * @Date: 2021-01-12 11:50:30
 * @Description: 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HexMap.Runtime
{
    public class EditorUI : MonoBehaviour
    {
        public static EditorUI instance { get; private set; }

        public UILoading loading;
        public UIMain main;

        void Awake()
        {
            instance = this;
        }

        public void Initialize(EditorModel model)
        {
            loading.Hide();
            main.Initialize(model);
        }
    }
}
