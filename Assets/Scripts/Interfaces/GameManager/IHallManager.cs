using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

interface IHallManager
{
    
    void Init(System.Action<string, bool> _loadSceneCallback, System.Action<int> _joinCallback, System.Action<int> _removeAction);

    /// <param name="GameStartFeature">Feature about game start.</param>
    IEnumerator UpdateCoreModel();
    IEnumerator StartCountDown();
    IEnumerator MapPollResults();
    void GameStart();
    /// <summary>
    /// Update camera rect of all players.
    /// </summary>
    void UpdateCameras();
    /// <summary>
    /// Player change selection of their avatars so re-active old choosen avatar againg.
    /// </summary>
    void UndoModelActive(GameObject model);
    /// <summary>
    /// Remove controller from certain player.
    /// </summary>



    /// <param name="PlayerCallback">All actions that players pressed their controller.</param>
    /// <summary>
    /// Press state back.
    /// </summary>
    void StateBack(int id);
    /// <summary>
    /// Press confirm and state equals to SelectEscaper.
    /// </summary>
    void SelectEscapser(int id, GameObject model);
    /// <summary>
    /// Press confirm and state equals to SelectHunter.
    /// </summary>
    void SelectHunter(int id, GameObject model);
    /// <summary>
    /// Press confirm and state equals to SelectMap.
    /// </summary>
    void SelectMap(int id, GameObject map);
    /// <summary>
    /// Auto called by SelectMap after finishing SelectMap.
    /// </summary>
    void AllStepFinished(int id);



    /// <param name="PlayerControllerAbout">Features about player controller and active.</param>
    void RemoveController(int id);
    /// <summary>
    /// Assign controller to certain player.
    /// </summary>
    void AssignController(Player player, InputActionSourceData source);
    /// <summary>
    /// Instant a player object.
    /// </summary>
    void GetPlayerActive(int id);
}
