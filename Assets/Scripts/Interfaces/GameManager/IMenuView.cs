using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

interface IMenuView
{
    void InitButton();
    void SelectCallback(Button btn);
}
