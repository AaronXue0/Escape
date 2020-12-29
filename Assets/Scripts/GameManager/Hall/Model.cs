using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameManagerSpace.Hall
{
    public class Model : MonoBehaviour
    {
        public GameObject GetPlayerAvatar { get { return playerAvatar; } }
        [SerializeField] GameObject playerAvatar = null;
        public Vector2 GetStartPos { get { return startPos.position; } }
        [SerializeField] Transform startPos = null;
    }
}