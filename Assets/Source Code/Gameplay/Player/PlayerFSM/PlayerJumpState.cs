using UnityEngine;
using System.Collections;

public class PlayerJumpState : IPlayerState
{

    public PLAYERSTATE StateName { get; set; }

    public PlayerFSM Player { get; set; }

    Vector2 jumpForce;
    float jumpStartTime;

    public PlayerJumpState(PlayerFSM player)
    {
        Player = player;
        StateName = PLAYERSTATE.JUMP;

        jumpForce = new Vector2(0f, Player.JumpMagnitude);
    }

    public void OnStateInit()
    {

    }

    public void OnStateEnter()
    {
        if (Player.b_Grounded)
        {
            Player.PlayJump();
            Player.m_ptrRigidbody.AddForce(jumpForce);
            jumpStartTime = Time.time;
        }
        GameManager.Instance.m_ptrUIController.OnPlayerStateChange(PLAYERSTATE.JUMP, false);
#if UNITY_ANDROID
        EventHandler.AddListener(EEventID.EVENT_TOUCH_STATIONARY, OnEventTouchStationary);
#elif UNITY_WEBGL
        EventHandler.AddListener(EEventID.EVENT_KEY_A, OnEventKeyA);
        EventHandler.AddListener(EEventID.EVENT_KEY_D, OnEventKeyD);
#endif
    }

    public void OnStateUpdate()
    {
        if ((Time.time - jumpStartTime) > 3f * Time.fixedDeltaTime)
        {
            if (Player.b_Grounded)
                Player.ChangeState(Player.idleState);
        }
    }

    void OnEventTouchStationary(System.Object data)
    {
        Hashtable table = (Hashtable)data;
        int fingerID = (int)table["fingerId"];

        //if (Player.m_iPrimaryTouch == fingerID)
        {

            Vector2 touchPos = (Vector2)table["touchPoint"];
            Vector2 WorldPos = Camera.main.ScreenToWorldPoint(touchPos);

            //if (Player.b_Grounded)
                //Player.PlayWalk();
            Player.m_iPrimaryTouch = fingerID;
            //b_touchEnd = false;

            if (WorldPos.x > Player.transform.position.x)
                MovePlayer(true);
            else
                MovePlayer(false);
        }
    }

    void OnEventKeyA(System.Object data)
    {
        MovePlayer(false);
    }

    void OnEventKeyD(System.Object data)
    {
        MovePlayer(true);
    }

    void MovePlayer(bool b_MoveFront)
    {
        int modifier = (b_MoveFront) ? 1 : -1;
        if ((modifier == 1) && !(Player.b_flipscale))
        {
            Player.transform.localScale = new Vector3(Mathf.Abs(Player.playerScale.x), Mathf.Abs(Player.playerScale.y), Mathf.Abs(Player.playerScale.z));
            Player.b_flipscale = !Player.b_flipscale;
        }
        else if ((modifier == -1) && (Player.b_flipscale))
        {
            Player.transform.localScale = new Vector3(-1f * Mathf.Abs(Player.playerScale.x), Player.playerScale.y, Player.playerScale.z);
            Player.b_flipscale = !Player.b_flipscale;
        }
        float x = Mathf.MoveTowards(Player.m_ptrRigidbody.velocity.x, Player.MaxVelocity * modifier, (Player.Acceleration/3.5f) * Time.fixedDeltaTime);
        Player.m_ptrRigidbody.velocity = new Vector3(x, Player.m_ptrRigidbody.velocity.y);
    }

    public void OnStateExit()
    {
#if UNITY_ANDROID
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_STATIONARY, OnEventTouchStationary);
#elif UNITY_WEBGL
        EventHandler.RemoveListener(EEventID.EVENT_KEY_A, OnEventKeyA);
        EventHandler.RemoveListener(EEventID.EVENT_KEY_D, OnEventKeyD);
#endif
    }

    ~PlayerJumpState()
    {
#if UNITY_ANDROID
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_STATIONARY, OnEventTouchStationary);
#elif UNITY_WEBGL
        EventHandler.RemoveListener(EEventID.EVENT_KEY_A, OnEventKeyA);
        EventHandler.RemoveListener(EEventID.EVENT_KEY_D, OnEventKeyD);
#endif
    }
}
