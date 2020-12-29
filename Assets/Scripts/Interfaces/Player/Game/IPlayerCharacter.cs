using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Role.PlayerSpace.Game
{
    interface IPlayerCharacter
    {
        void AssignController(int id);
        void AssignTeam(int id, List<System.Action<PlayerCharacter>> callbacks);
        void GetItemSuccess();
        void GetCaught();
    }
}