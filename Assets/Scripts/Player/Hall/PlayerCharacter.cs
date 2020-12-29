using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace Role.PlayerSpace.Hall
{
    public class PlayerCharacter : MonoBehaviour
    {
        [SerializeField] float speed = 5;
        private int playerId = 0;
        private Player player = null;
        private Vector3 moveVector = Vector3.zero;
        private Camera selfCamera = null;

        List<System.Action<int, GameObject>> gameActions = new List<System.Action<int, GameObject>>();

        Rigidbody2D rb = null;

        public void AssignController(int id, List<System.Action<int, GameObject>> callbacks)
        {
            playerId = id;
            player = ReInput.players.GetPlayer(playerId);
            gameActions = callbacks;
        }

        private void Update()
        {
            Move();
        }

        void Move()
        {
            moveVector.x = player.GetAxis("CursorMoveX");
            moveVector.y = player.GetAxis("CursorMoveY");
            rb.velocity = moveVector * speed;
        }

        private void OnTriggerStay2D(Collider2D other)
        {
            switch (other.tag)
            {
                case "EscaperModel":
                    if (player.GetButtonDown("Choose"))
                    {
                        gameActions[0](playerId, other.gameObject);
                    }
                    break;
                case "HunterModel":
                    if (player.GetButtonDown("Choose"))
                    {
                        gameActions[1](playerId, other.gameObject);
                    }
                    break;
                case "MapModel":
                    if (player.GetButtonDown("Choose"))
                    {
                        gameActions[2](playerId, other.gameObject);
                    }
                    break;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            switch (other.tag)
            {
                case "EscaperModel":
                case "HunterModel":
                case "MapModel":
                    break;
            }
        }

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            player = ReInput.players.GetPlayer(0);
            selfCamera = transform.parent.GetComponentInChildren<Camera>();
        }
    }
}