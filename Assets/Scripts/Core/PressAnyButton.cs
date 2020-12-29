using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PressAnyButton : MonoBehaviour
{
    [SerializeField] string nextScene = "MenuScene";
    [SerializeField] GameObject text = null;
    public System.Action<string> SceneCallbackAction { get; set; }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (SceneCallbackAction != null)
                StartCoroutine(WaitForAllAnimationFinish());
        }
    }

    IEnumerator WaitForAllAnimationFinish()
    {
        text.SetActive(false);
        foreach (AnimationCore _a in FindObjectsOfType<AnimationCore>())
        {
            if (_a.enabled == false) continue;
            var wait = _a.WaitForFinish();
            yield return wait;
        }
        SceneCallbackAction(nextScene);
    }
}
