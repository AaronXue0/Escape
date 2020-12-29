using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHighlighted : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    System.Action<Button> selectionAction = null;
    Button selfBtn = null;

    public void Init(System.Action<Button> select)
    {
        selectionAction = select;
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (selectionAction == null) return;
        selectionAction(GetComponent<Button>());
    }

    private void Awake()
    {
        selfBtn = GetComponent<Button>();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (selectionAction == null) return;
        selectionAction(null);
    }
}
