using System.Collections;
using System.Collections.Generic;
using UnityEngine;


interface IGameManager
{
    void Init(System.Action<string, bool> loadSceneActionCallback, System.Action loadedCallback);
    
    void GameFlow();

    /// <param name="GameFlow">Function controlled by Game Flow.</param>
    void SettingCallback();
    IEnumerator Setting();
    void StartingCallback();
    IEnumerator Starting();
    void PlayingCallback();
    IEnumerator Playing();
    void ScoringCallback();
    IEnumerator Scoring();
}
