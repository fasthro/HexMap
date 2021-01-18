using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace HexMap
{
    public static class CheckMissScripts
    {
        public static void RemoveMissComponent()
        {
            string fullPath = Application.dataPath + "/Art";
            fullPath = fullPath.Replace("/", @"\");
            List<string> pathList = GetAssetsPathByRelativePath(new string[] { "Assets/Art" }, "t:Prefab", SearchOption.AllDirectories);
            int counter = 0;
            for (int i = 0, iMax = pathList.Count; i < iMax; i++)
            {
                EditorUtility.DisplayProgressBar("处理进度", string.Format("{0}/{1}", i + 1, iMax), (i + 1f) / iMax);
                if (CheckMissMonoBehavior(pathList[i]))
                    ++counter;
            }
            EditorUtility.ClearProgressBar();
            EditorUtility.DisplayDialog("处理结果", "完成修改，修改数量 : " + counter, "确定");
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 获取项目中某种资源的路径
        /// </summary>
        /// <param name="fullPath">win的路径格式，以 "\"为分隔符</param>
        /// <param name="filter">win的资源过滤模式 例如 : *.prefab</param>
        /// <param name="searchOption">目录的搜索方式</param>
        /// <returns></returns>
        static List<string> GetAssetsPathByFullPath(string fullPath, string filter, SearchOption searchOption)
        {
            List<string> pathList = new List<string>();
            string[] files = Directory.GetFiles(fullPath, filter, searchOption);
            for (int i = 0; i < files.Length; i++)
            {
                string path = files[i];
                path = "Assets" + path.Substring(Application.dataPath.Length, path.Length - Application.dataPath.Length);
                pathList.Add(path);
            }

            return pathList;
        }


        /// <summary>
        /// 获取项目中某种资源的路径
        /// </summary>
        /// <param name="relativePath">unity路径格式，以 "/" 为分隔符</param>
        /// <param name="filter">unity的资源过滤模式 https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html </param>
        /// <param name="searchOption"></param>
        /// <returns></returns>
        static List<string> GetAssetsPathByRelativePath(string[] relativePath, string filter, SearchOption searchOption)
        {
            List<string> pathList = new List<string>();
            string[] guids = AssetDatabase.FindAssets(filter, relativePath);
            for (int i = 0; i < guids.Length; i++)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[i]);
                pathList.Add(path);
            }

            return pathList;
        }

        /// <summary>
        /// 删除一个Prefab上的空脚本
        /// </summary>
        /// <param name="path">prefab路径 例Assets/Resources/FriendInfo.prefab</param>
        static bool CheckMissMonoBehavior(string path)
        {
            bool isNull = false;
            string textContent = File.ReadAllText(path);
            Regex regBlock = new Regex("MonoBehaviour");
            // 以"---"划分组件
            string[] strArray = textContent.Split(new string[] { "---" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < strArray.Length; i++)
            {
                string blockStr = strArray[i];
                if (regBlock.IsMatch(blockStr))
                {
                    // 模块是 MonoBehavior
                    //(?<名称>子表达式)  含义:将匹配的子表达式捕获到一个命名组中
                    Match guidMatch = Regex.Match(blockStr, "m_Script: {fileID: (.*), guid: (?<GuidValue>.*?), type: [0-9]}");
                    if (guidMatch.Success)
                    {
                        string guid = guidMatch.Groups["GuidValue"].Value;
                        if (!File.Exists(AssetDatabase.GUIDToAssetPath(guid)))
                        {
                            isNull = true;
                            textContent = DeleteContent(textContent, blockStr);
                        }
                    }

                    Match fileIdMatch = Regex.Match(blockStr, @"m_Script: {fileID: (?<IdValue>\d+)}");
                    if (fileIdMatch.Success)
                    {
                        string idValue = fileIdMatch.Groups["IdValue"].Value;
                        if (idValue.Equals("0"))
                        {
                            isNull = true;
                            textContent = DeleteContent(textContent, blockStr);
                        }
                    }
                }
            }
            if (isNull)
            {
                // 有空脚本 写回prefab
                File.WriteAllText(path, textContent);
            }
            return isNull;
        }

        // 删除操作
        static string DeleteContent(string input, string blockStr)
        {
            input = input.Replace("---" + blockStr, "");
            Match idMatch = Regex.Match(blockStr, "!u!(.*) &(?<idValue>.*?)\n");
            if (idMatch.Success)
            {
                // 获取 MonoBehavior的fileID
                string fileID = idMatch.Groups["idValue"].Value;
                Regex regex = new Regex("  - (.*): {fileID: " + fileID + "}\n");
                input = regex.Replace(input, "");
            }

            return input;
        }
    }
}