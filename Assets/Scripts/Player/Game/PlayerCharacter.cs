using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

namespace Role.PlayerSpace.Game
{
    public class PlayerCharacter : MonoBehaviour
    {
        private Control control = null;
        private Player input = null;

        List<System.Action<PlayerCharacter>> gameActions = null;

        public int GetId { get { return playerId; } }
        private int playerId = 0;
        public int GetTeamID { get { return teamId; } }
        private int teamId = 0;

        public void AssignController(int id)
        {
            playerId = id;
            input = ReInput.players.GetPlayer(playerId);
        }
        public void AssignTeam(int id, List<System.Action<PlayerCharacter>> callbacks)
        {
            teamId = id;
            gameActions = callbacks;
            control.GameStartSetup(id);
        }

        private void Awake()
        {
            control = GetComponent<Control>();
            AssignController(0);
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.L)) control.Hurt(control.Mutate);
            if (Input.GetKeyDown(KeyCode.C)) control.CancelItem();
            if (input == null) return;
            float movement = input.GetAxis("Move Horizontal");
            control.Move(movement);

            if (input.GetButtonDown("Jump")) control.Jump(true);
            else if (input.GetButtonUp("Jump")) control.Jump(false);

            if (input.GetButtonDown("Run")) control.Run(true);
            else if (input.GetButtonUp("Run")) control.Run(false);

            if (input.GetButtonDown("Attack")) control.Attack();
            
            if (input.GetButtonDown("Squat")) control.Squat(true);
            else if (input.GetButtonUp("Squat")) control.Squat(false);
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            switch (other.tag)
            {
                case "PlayerWeapon":
                    control.Hurt(GetCaught);
                    break;
                case "Flag":
                    // if (control.PlayerState != control.AsEscaper || control.PlayerState != control.AsHunter)
                    //     return;
                    gameActions[2](this);
                    break;
                case "DashItem":
                    string[] handler = other.name.Split(',');
                    float forceX = 0;
                    float forceY = 0;
                    forceX = (string.Compare("n", handler[0]) == 0) ? transform.localScale.x : float.Parse(handler[0]);
                    forceY = (string.Compare("n", handler[1]) == 0) ? transform.localScale.y : float.Parse(handler[1]);
                    control.DODash(new Vector2(
                        forceX,
                        forceY
                    ));
                    break;
            }
        }
        void GetItemSuccess()
        {
            gameActions[0](this);
        }

        void GetCaught()
        {
            gameActions[1](this);
        }
        private void OnTriggerStay2D(Collider2D other)
        {
            switch (other.tag)
            {
                case "IceSkate":
                case "SlimeShoe":
                case "Shield":
                case "EnergyDrink":
                case "Crucifixion":
                case "Armor":
                case "LightnessShoe":
                case "RocketShoe":
                case "DeveloperObsession":
                case "Immortal":
                case "Balloon":
                case "Trophy":
                case "Detector":
                    if (input.GetButtonDown("Item"))
                    {
                        control.DestroyObject(other.gameObject);
                        control.GetStartItem(other.tag, GetItemSuccess);

                    }

                    break;

                default:
                    break;
            }
        }
    }
}