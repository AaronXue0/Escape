using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Role.PlayerSpace.Game;

interface IGameControl
{
    void Init();

    /// <param name="Callback">Callback assign to players.</param>
    void GetStartItemCallback(PlayerCharacter role);
    void GetCaught(PlayerCharacter role);
    void GetGoal(PlayerCharacter role);

    /// <param name="Setting">Game is setting.</param>
    IEnumerator RandomRoom();
    IEnumerator RandomStartItems();
    IEnumerator SpawnPlayers();
    /// <param name="Starting">Game is starting.</param>
    IEnumerator EscaperStart();
    IEnumerator HunterStart();
    /// <param name="Playing">Game is playing.</param>
    IEnumerator Playing();
    IEnumerator PlayingCoroutine(int mode);
    IEnumerator PlayFinished();
    IEnumerator DelayAndBroadcast(int counter, bool doCast);
    /// <param name="Game over">This round is over.</param>
    IEnumerator Scoring();
}
