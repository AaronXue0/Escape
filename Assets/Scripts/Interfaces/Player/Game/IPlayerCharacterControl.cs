using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Role.PlayerSpace.Game
{
    interface IPlayerCharacterControl
    {
        void GameStartSetup(int id);


        /// <param name="UIs">Update UI View.</param>

        void UpdateViews();


        /// <param name="StartItems">About start items.</param>
        void GetStartItem(Collider2D other, System.Action callback);
        void UpdateShield();

        /// <param name="State">Player state.</param>
        void Mutate();
        void Dead();
        void Hurt(System.Action callback);
        
        /// <param name="InputAndControl">Input and control feature.</param>
        void EnduranceSystem();
        void Attack();
        void Move(float value);
        void Jump(bool isJumping);
        void GroundControl();
        void FrontControl();
        bool GroundCheck(LayerMask mask);
        bool FrontCheck(LayerMask mask);
        void DOMove();
        void MoveGainHandle();
        void IceMoveGainHandle();
        void DOJump();
    }
}