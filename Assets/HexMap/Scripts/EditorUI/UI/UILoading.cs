/*
 * @Author: fasthro
 * @Date: 2021-01-07 18:17:30
 * @Description: 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HexMap
{
    public class UILoading : MonoBehaviour
    {
        public Text contentText;

        public void Show(string content)
        {
            gameObject.SetActive(true);
            contentText.text = content;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}