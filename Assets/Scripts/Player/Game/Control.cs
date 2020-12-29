using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Role.PlayerSpace.Game
{
    public class Control : MonoBehaviour
    {
        public void GameStartSetup(int id)
        {
            // model.TeamID = id;
            // switch (id)
            // {
            //     case 0:
            //         PlayerState = PlayerState.escaper;
            //         model.Hp = 3;
            //         break;
            //     case 1:
            //         PlayerState = PlayerState.hunter;
            //         model.Hp = 50;
            //         model.StateSpeedGain = 1.05f;
            //         model.StateJumpGain = 1f;
            //         break;
            //     default:
            //         Debug.Log("Player teamID errors");
            //         break;
            // }
        }
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███  Public Function  ███████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        public void DODash(Vector2 value)
        {
            move.DOAddforceImpulse(value * model.DashPower);
        }

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███  Input Function  ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        public void Move(float value)
        {
            horizontalInput = value;
            anime.DOAnimation("movement", Mathf.Abs(horizontalInput));
            transform.localScale = new Vector2(
                transform.localScale.x >= 0 ?
                horizontalInput >= 0 ? 1 : -1 :
                horizontalInput <= 0 ? -1 : 1
            , 1);
        }
        public void Jump(bool isJumping)
        {
            switch (isJumping)
            {
                case true:
                    FrontControl();
                    GroundControl();
                    /*if (OnGround && OnWall)
                    {
                        Debug.Log("WallJump");
                        isWallJumping = true;
                        JumpState = JumpState.preWallJumping;
                    }*/
                    if (OnGround)
                    {
                        JumpState = JumpState.preJumping;
                    }
                    else if (OnWall /*&& (JumpState == JumpState.isWallSliding || JumpState == JumpState.preWallSliding)*/)
                    {
                        isWallJumping = true;
                        JumpState = JumpState.preWallJumping;
                    }
                    else if (!OnGround && !OnWall && model.RocketShoe && !model.RocketJump)
                    {
                        model.RocketJump = true;
                        Debug.Log("rocket");
                        jumpTimeCounter = 0f;
                        model.GroundJumpGain = 0.5f;
                        JumpState = JumpState.preJumping;
                    }
                    break;
                case false:
                    if (JumpState == JumpState.isJumping)
                    {
                        JumpState = JumpState.preFalling;
                    }
                    break;
            }
        }
        public void Run(bool value)
        {
            switch (value)
            {
                case true:
                    isRunning = true;
                    isAbleToRecoveryEndurance = false;
                    model.AddSpeedGain = 0.2f;
                    model.AddJumpGain = 0.1f;
                    break;
                case false:
                    isRunning = false;
                    this.AbleToDo(() => isAbleToRecoveryEndurance = true, 1f);
                    model.SpeedGain = 1f;
                    model.JumpGain = 1f;
                    break;
            }
        }
        public void Attack()
        {
            if (PlayerState.StateCompare(AsHunter) == false) return;
            if (isAttacking) return;
            isAttacking = true;
            this.AbleToDo(() => isAttacking = false, 1f);
            anime.DOAnimation("attack");
        }
        public void Hurt(System.Action callback)
        {
            if (PlayerState == PlayerState.spectator) return;
            if (isHurting) return;
            isHurting = true;
            this.AbleToDo(() => isHurting = false, 1f);
            if (model.Shielding == false)
            {
                model.Hp--;
                if (model.Hp <= 0)
                {
                    Mutate();
                    callback();
                    return;
                }
                anime.DOAnimation("hurt");
            }
            else model.Shielding = false;
        }
        public void Mutate()
        {
            int playerLayer = 27;
            int specatorLayer = 30;
            Camera cam = transform.parent.GetChild(1).GetComponent<Camera>();
            cam.cullingMask = -1;
            PlayerState = PlayerState.spectator;
            view.UpdaetShaderRenderer("GHOST_ON");
            gameObject.layer = specatorLayer;
            foreach (Transform child in GetComponentInChildren<Transform>())
            {
                if (child.gameObject.layer == playerLayer)
                {
                    child.gameObject.layer = specatorLayer;
                }
            }
        }
        public void Squat(bool value)
        {
            isSquating = value;
            if (isSquating) transform.rotation = Quaternion.Euler(50, 0, 0);
            else transform.rotation = new Quaternion(0, 0, 0, 0);
        }
        public void DeadForSec()
        {

        }
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███  MoveHandling Function  █████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        void GroundControl()
        {
            // if (GroundState == GroundState.controled) return;
            model.Grounded = GroundCheck(model.GetGroundLayer) || GroundCheck(model.GetBoxLayer);
            model.IceGrounded = GroundCheck(model.GetIceGroundLayer);
            model.SlimeGrounded = GroundCheck(model.GetSlimeGroundLayer);

            if (rb.velocity.magnitude > 40)
            {
                GroundState = GroundState.controled;
            }
            else if (model.Grounded)
            {
                model.GroundSpeedGain = 1;
                model.GroundJumpGain = 1f;
                model.GroundState = GroundState.normal;
            }
            else if (model.IceGrounded)
            {
                model.GroundSpeedGain = 1;
                model.GroundJumpGain = 1;
                if (model.IceSkate == false) model.GroundState = GroundState.ice;
                else model.GroundState = GroundState.normal;
            }
            else if (model.SlimeGrounded)
            {
                if (model.SlimeShoe == false)
                {
                    model.GroundSpeedGain = 0.5f;
                    model.GroundJumpGain = 0.2f;
                    model.GroundState = GroundState.slime;
                }
                else model.GroundState = GroundState.normal;
            }
            else
            {
                model.GroundState = GroundState.air;
            }
        }
        void FrontControl()
        {
            // if (FrontState == FrontState.controled) return;

            model.Fronted = FrontCheck(model.GetGroundLayer);
            model.IceFronted = FrontCheck(model.GetIceGroundLayer);
            model.SlimeFronted = FrontCheck(model.GetSlimeGroundLayer);

            if (model.Fronted)
            {
                model.WallSpeedGain = 1f;
                model.WallJumpGain = 1f;
                model.WallSlideGain = 1f;
            }
            else if (model.IceFronted && OnWall)
            {
                if (model.IceSkate == false)
                {
                    model.WallSpeedGain = 1.3f;
                    model.WallJumpGain = 0f;
                    model.WallSlideGain = 10f;
                }
                else
                {
                    model.WallSpeedGain = 1f;
                    model.WallJumpGain = 1f;
                    model.WallSlideGain = 1f;
                }
            }
            else if (model.SlimeFronted && OnWall)
            {
                if (model.SlimeShoe == false)
                {
                    model.WallSpeedGain = 0.7f;
                    model.WallJumpGain = 0.7f;
                    model.WallSlideGain = 1f;
                }
                else
                {
                    model.WallSpeedGain = 1f;
                    model.WallJumpGain = 1f;
                    model.WallSlideGain = 1f;
                }
            }

            if (OnGround == false && isWallJumping == false && JumpState != JumpState.isJumping && JumpState != JumpState.preJumping) JumpState = JumpState.preFalling;
            if (isWallJumping == false && OnWall && rb.velocity.y < 0 && JumpState != JumpState.preWallJumping) JumpState = JumpState.preWallSliding;
        }
        bool GroundCheck(LayerMask mask)
        {
            bool detect = false;
            foreach (var ground in model.GetGroundCheck)
            {
                if (Physics2D.Raycast(
                    ground.position,
                    Vector2.down,
                    model.GroundCheckDistance,
                    mask
                ))
                {
                    detect = true;
                    break;
                }
            };
            return detect ? true : false;
        }
        bool FrontCheck(LayerMask mask)
        {
            bool detect = false;
            foreach (var front in model.GetFrontCheck)
            {
                if (Physics2D.Raycast(
                    front.position,
                    Vector2.right * horizontalInput,
                    model.GroundCheckDistance,
                    mask
                ))
                {
                    detect = true;
                    break;
                }
            };
            if (detect) wallJumpPos = -horizontalInput;
            return detect ? true : false;
        }
        bool OnGround
        {
            get
            {
                return model.Grounded || model.IceGrounded || model.SlimeGrounded;
            }
        }
        bool OnWall
        {
            get
            {
                if (OnGround)
                    return false;
                else
                    return model.Fronted || model.IceFronted || model.SlimeFronted;
            }
        }
        void DOMove()
        {
            switch (GroundState)
            {
                case GroundState.controled:
                    move.DOControlMove(horizontalInput);
                    break;
                case GroundState.air:
                case GroundState.normal:
                case GroundState.slime:
                    if (horizontalInput != 0)
                    {
                        MoveGainHandle();
                    }
                    else move.DOStopMoveX();
                    break;
                case GroundState.ice:

                    if (horizontalInput != 0)
                    {
                        IceMoveGainHandle();
                        move.DOSlowDown();
                    }
                    else move.DOSlowDown();
                    break;
                default:
                    break;
            }
        }
        void MoveGainHandle()
        {
            if (isSquating && rb.velocity.x == 0) model.SlideSpeedGain = 0;
            else if (isSquating && model.SlideSpeedGain > 0) model.SlideSpeedGain -= 0.015f;
            else if (isSquating && model.SlideSpeedGain <= 0) model.SlideSpeedGain = 0;
            else model.SlideSpeedGain = 1f;
            FrontControl();
            if (OnWall)
            {
                move.DOMoveX(0);
            }
            else
            {
                move.DOMoveX(horizontalInput
                    * model.WalkSpeed
                    * model.ItemSpeedGain
                    * model.GroundSpeedGain
                    * model.SpeedGain
                    * model.StateSpeedGain
                    * model.SlideSpeedGain);
            }

        }
        void IceMoveGainHandle()
        {
            FrontControl();
            if (rb.velocity.x < 15)
            {
                if (OnWall)
                {
                    move.DOAddforceX(0);
                }
                else
                {
                    move.DOAddforceX(horizontalInput * 3
                        * model.WalkSpeed
                        * model.ItemSpeedGain
                        * model.GroundSpeedGain
                        * model.SpeedGain
                        * model.StateSpeedGain
                        * model.SlideSpeedGain);
                }
            }
        }
        void DOJump()
        {
            /* Jump Animation */
            switch (JumpState)
            {
                case JumpState.preWallSliding:
                    anime.DOAnimation("fall");
                    JumpState = JumpState.isWallSliding;
                    break;
                case JumpState.preWallJumping:
                    anime.DOAnimation("wallJump");
                    JumpState = JumpState.isWallJumping;
                    break;
                case JumpState.preJumping:
                    anime.DOAnimation("jump");
                    JumpState = JumpState.isJumping;
                    break;
                case JumpState.preFalling:
                    anime.DOAnimation("fall");
                    JumpState = JumpState.isFalling;
                    break;
                case JumpState.preGrounded:
                    anime.DOAnimation("exit");
                    JumpState = JumpState.isGrounded;
                    break;
            }
            /* Jump Feature */
            switch (JumpState)
            {
                case JumpState.isWallSliding:
                    model.RocketJump = false;
                    //if (wallJumpTimeCounter > 0) wallJumpTimeCounter = 0;
                    move.DOMove(Vector2.down
                        * model.ItemJumpGain
                        * (2 - model.GroundJumpGain)
                        * model.JumpGain
                        * model.WallSlideGain);
                    if (OnWall == false || OnGround) JumpState = JumpState.preFalling;
                    break;
                case JumpState.isWallJumping:
                    if (wallJumpTimeCounter < model.WallJumpTime)
                    {
                        wallJumpTimeCounter += Time.deltaTime;
                        move.DOMove(new Vector2(
                            model.WallJumpForce.x * wallJumpPos
                            * model.ItemJumpGain
                            * model.JumpGain
                            * model.StateJumpGain
                            * model.WallJumpGain
                        , model.WallJumpForce.y
                            * model.ItemJumpGain
                            * model.JumpGain
                            * model.StateJumpGain
                            * model.WallJumpGain));
                    }
                    else
                    {
                        isWallJumping = false;
                        JumpState = JumpState.preFalling;
                    }
                    break;
                case JumpState.isJumping:
                    if (jumpTimeCounter < model.JumpTime)
                    {
                        jumpTimeCounter += Time.deltaTime;
                        move.DOMoveY(Vector2.up.y * model.JumpForce
                        * model.ItemJumpGain
                        * model.GroundJumpGain
                        * model.JumpGain
                        * model.StateJumpGain);
                    }
                    else JumpState = JumpState.preFalling;
                    break;
                case JumpState.isFalling:
                    if (OnGround) JumpState = JumpState.preGrounded;
                    break;
                case JumpState.isGrounded:
                    jumpTimeCounter = 0;
                    model.RocketJump = false;
                    break;
            }
        }
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███  Other Function  ████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        private void Update()
        {
            if (model.Shield) UpdateShield();
            UpdateViews();
            EnduranceSystem();
            GroundControl();
            if (OnWall) if (wallJumpTimeCounter > 0) wallJumpTimeCounter = 0;
            if (OnGround)
            {
                return;
            }
            FrontControl();
        }
        private void UpdateShield()
        {
            Debug.Log(model.Shielding);
            if (model.Shielding == false)
            {
                this.AbleToDo(() => model.Shielding = true, 10f);
            }
        }
        private void UpdateViews()
        {
            view.UpdateHealthBar(GetEnduranceAmount);
            view.UpdateBalloon(model.Balloon);
        }
        private void EnduranceSystem()
        {
            if (endurance <= 0)
                Run(false);
            if (isRunning)
                endurance -= Time.deltaTime;
            if (isRunning == false && isAbleToRecoveryEndurance && endurance < model.Endurance)
            {
                if (model.EnergyDrink) endurance += Time.deltaTime;
                endurance += Time.deltaTime;
            }
        }
        private void FixedUpdate()
        {
            DOMove();
            DOJump();
        }
        private void Awake()
        {
            SetGetcomponents();
            Init();
        }
        void SetGetcomponents()
        {
            model = GetComponent<Model>();
            view = GetComponent<View>();
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            anime = new Anime(animator);
            move = new Move(rb);
        }
        void Init()
        {
            endurance = model.Endurance;
            if (OnGround) JumpState = JumpState.preGrounded;
            else JumpState = JumpState.preFalling;
            FrontState = FrontState.air;
            GroundState = GroundState.air;
        }
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███  Item  ███████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████

        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███  Item  ███████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        public void GetStartItem(string tag, System.Action callback)
        {
            if (model.IsGetStartItem == false)
            {
                model.IsGetStartItem = true;

                switch (tag)
                {
                    case "IceSkate":
                        model.IceSkate = true;
                        break;
                    case "SlimeShoe":
                        model.SlimeShoe = true;
                        break;
                    case "Shield":
                        model.Shielding = true;
                        model.Shield = true;
                        break;
                    case "EnergyDrink":
                        model.EnergyDrink = true;
                        break;
                    case "Crucifixion":

                        model.Crucifixion = true;
                        break;
                    case "Armor":
                        model.AddItemSpeedGain = -0.1f;
                        model.AddItemJumpGain = -0.1f;
                        model.AddHp(2);
                        model.Armor = true;
                        break;
                    case "LightnessShoe":
                        model.LightnessShoe = true;
                        model.AddItemSpeedGain = 0.1f;
                        model.AddItemJumpGain = 0.1f;
                        break;
                    case "RocketShoe":

                        model.RocketShoe = true;
                        break;
                    case "DeveloperObsession":

                        model.DeveloperObsession = true;
                        break;
                    case "Immortal":

                        model.Immortal = true;
                        break;
                    case "Balloon":
                        model.Balloon = true;
                        break;
                    case "Trophy":

                        model.Trophy = true;
                        break;
                    case "Detector":
                        model.Detector = true;
                        break;
                    default:
                        break;
                }
                callback();
            }
        }
        public void DestroyObject(GameObject gameObject)
        {
            if (model.IsGetStartItem == false)
                gameObject.SetActive(false);
        }
        public void CancelItem()
        {
            model.IsGetStartItem = false;
            model.IceSkate = false;
            model.SlimeShoe = false;
            model.Shield = false;
            model.EnergyDrink = false;
            model.Balloon = false;
            model.Armor = false;
            model.LightnessShoe = false;
            model.Crucifixion = false;
            model.RocketShoe = false;
            model.DeveloperObsession = false;
            model.Immortal = false;
            model.Trophy = false;
            model.Detector = false;
            model.Shielding = false;
            if (model.LightnessShoe)
            {
                model.AddItemSpeedGain = -0.1f;
                model.AddItemJumpGain = -0.1f;
            }
            if (model.Armor)
            {
                model.AddItemSpeedGain = 0.1f;
                model.AddItemJumpGain = 0.1f;
                model.AddHp(-2);
            }
        }
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███  Get/Setters  ███████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        public PlayerState PlayerState { get { return model.PlayerState; } set { model.PlayerState = value; } }
        public PlayerState AsWaiting { get { return PlayerState.waiting; } }
        public PlayerState AsEscaper { get { return PlayerState.escaper; } }
        public PlayerState AsHunter { get { return PlayerState.hunter; } }
        public PlayerState AsSpectator { get { return PlayerState.spectator; } }
        GroundState GroundState { get { return model.GroundState; } set { model.GroundState = value; } }
        FrontState FrontState { get { return model.FrontState; } set { model.FrontState = value; } }
        JumpState JumpState { get { return model.JumpState; } set { model.JumpState = value; } }
        private float GetEnduranceAmount { get { return endurance / model.Endurance; } }
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███  Components  ████████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        Model model = null;
        View view = null;
        Move move = null;
        Anime anime = null;
        Rigidbody2D rb = null;
        Animator animator = null;
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        // ███  Variables  █████████████████████████████████████████████████████████████████████████████████████████████████████
        // █████████████████████████████████████████████████████████████████████████████████████████████████████████████████████
        float horizontalInput = 0;
        float jumpTimeCounter = 0;
        float wallJumpTimeCounter = 0;
        float wallJumpPos = 0;
        float endurance = 0;
        bool isWallJumping = false;
        bool isRunning = false;
        bool isAbleToRecoveryEndurance = false;
        bool isAttacking = false;
        bool isHurting = false;
        bool isSquating = false;
    }

    public class Move
    {
        Rigidbody2D rb;

        public Move(Rigidbody2D rb)
        {
            this.rb = rb;
        }
        public void DOMove(Vector2 force)
        {
            rb.velocity = force;
        }
        public void DOAddMove(Vector2 force) // DOAddvelocity
        {
            rb.velocity += force;
        }
        public void DOMoveX(float force)
        {
            rb.velocity = new Vector2(force, rb.velocity.y);
        }
        public void DOMoveY(float force)
        {
            rb.velocity = new Vector2(rb.velocity.x, force);
        }
        public void DOControlMove(float force)
        {
            // if((rb.velocity.x > 0 && force < 0)
            // || (rb.velocity.x < 0 && force > 0)) DOMoveX(-rb.velocity.x);
            // DOAddforceX(rb.velocity.x * 0.9f);
            // DOMoveX();
        }
        public void DOAddforce(Vector2 force)
        {
            rb.AddForce(force);
        }
        public void DOAddforceImpulse(Vector2 force)
        {
            rb.AddForce(force, ForceMode2D.Impulse);
        }
        public void DOAddforceX(float force)
        {
            rb.AddForce(new Vector2(force, 0));
        }
        public void DOAddforceY(float force)
        {
            rb.AddForce(new Vector2(0, force));
        }
        public void DOStopMoveX()
        {
            DOMoveX(0);
        }
        public void DOSlowDown()
        {
            DOAddMove(rb.velocity * -0.05f);
        }
        public void DOIceSlide(float posNForce)
        {
            DOAddforceImpulse(Vector2.right * posNForce);
        }
        public void DOSlide()
        {
            DOAddMove(rb.velocity * -0.1f);
        }
    }

    public class Anime
    {
        Animator animator;
        public Anime(Animator aniamtor)
        {
            this.animator = aniamtor;
        }
        public void DOAnimation(string name)
        {
            animator.SetTrigger(name);
        }
        public void DOAnimation(string name, float value)
        {
            animator.SetFloat(name, value);
        }
    }

    public static class PlayerExtension
    {
        public static bool StateCompare(this PlayerState state, PlayerState compareState)
        {
            if (state != compareState)
                return false;
            return true;
        }
        public static T AbleToDo<T>(this T source, System.Action action, float sec) where T : MonoBehaviour
        {
            T t = source;
            t.StartCoroutine(DelaySec(sec, action));
            return t;
        }
        public static IEnumerator DelaySec(float sec, System.Action callback)
        {
            yield return new WaitForSeconds(sec);
            callback();
        }
    }
}