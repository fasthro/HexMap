using HexMap.Runtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GroundCell), true)]
public class ChangeAreaEditor : Editor
{
    private GameObject selectArea;
    private string[] areaNames;
    private int window_w = 300;
    private int window_h = 300;

    private void OnEnable()
    {
        Debug.Log("-------------------");
        //包含该组件的物体被选中的时候调用
        areaNames = Directory.GetFiles("Assets/Art/Map/Area/Material", "*.mat");
        if (areaNames != null)
        {
            for (int i = 0; i < areaNames.Length; i++)
            {
                areaNames[i] = areaNames[i].Replace("Assets/Art/Map/Area/Material\\", "");
            }
        }
        window_w = 200;
        window_h = 280 + (areaNames.Length - 3) * 50;
    }

    public void OnSceneGUI()
    {
        selectArea = Selection.activeGameObject;
        if (selectArea == null)
        {
            return;
        }
        if (selectArea.name == "Ground_")
        {
            selectArea = selectArea.transform.parent.gameObject;
            Debug.Log(selectArea.name);
        }
        GroundCell groundCell = selectArea.GetComponent<GroundCell>();
        if (groundCell == null)
        {
            return;
        }

        GUI.Window(0, new Rect(Screen.width - window_w, Screen.height - window_h, window_w, window_h), ShowWindow, "Change Area");
    }

    private void ShowWindow(int id)
    {
        Handles.BeginGUI();
        GUILayout.BeginArea(new Rect(0, 20, window_w, window_h));

        GUILayout.Label("替换选中地表");

        for (int i = 0; i < areaNames.Length; i++)
        {
            if (GUILayout.Button(string.Format("替换成 {0}", areaNames[i]), GUILayout.Height(50)))
            {
                Change(i);
            }
        }

        GUILayout.EndArea();
        Handles.EndGUI();
    }

    private void Change(int m_index)
    {
        GroundCell groundCell = selectArea.GetComponent<GroundCell>();
        groundCell.materialIndex = m_index;
        HexMap.Runtime.HexMap.instance.ground.SetTileData(groundCell.index, m_index);
        HexMap.Runtime.HexMap.instance.ground.AfreshDrawMesh();
    }
}