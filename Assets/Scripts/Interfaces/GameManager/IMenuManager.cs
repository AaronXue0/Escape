using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IMenuManager
{
    void Init(System.Action<string, bool>  callback);
    void AnimationEventCallback();
    void Play();
    void Settings();
    void Exit();
    void Confirm();
}
