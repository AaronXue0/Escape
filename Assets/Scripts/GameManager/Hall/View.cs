using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GameManagerSpace.Hall
{
    public class View : MonoBehaviour, IHallView
    {
        [SerializeField] TMP_Text tmp = null;

        public void UpdateTMP_Text(string msg)
        {
            tmp.text = msg;
        }
    }

    [System.Serializable]
    public class UIContent
    {
        public int id;
        public TMP_Text text;
        public Image image;
    }
}