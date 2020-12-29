using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Role.PlayerSpace.Game;

interface IGameView
{
    void UpdateCountingDownView(List<PlayerCharacter> roles, string msg);
}
