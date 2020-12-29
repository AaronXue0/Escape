using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Role.PlayerSpace.Game;
using TMPro;

namespace GameManagerSpace.Game
{
    public class View : MonoBehaviour, IGameView
    {
        public void UpdateCountingDownView(List<PlayerCharacter> roles, string msg)
        {
            foreach (PlayerCharacter role in roles)
            {
                role.transform.parent.GetComponentInChildren<TMP_Text>().text = msg;
            }
        }
    }
}