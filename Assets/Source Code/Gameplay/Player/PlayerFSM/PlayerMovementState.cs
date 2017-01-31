using UnityEngine;
using System.Collections;

public class PlayerMovementState : IPlayerState
{
    public PLAYERSTATE StateName { get; set; }

    public PlayerFSM Player { get; set; }

    bool b_touchEnd;
    bool b_moveForward;

    float m_startVelocity;
    float m_timeStamp;
    float m_maxVelocityModifier;

    Vector2 moveLeftCoords;
    Vector2 moveRightCoords;

    public PlayerMovementState(PlayerFSM player)
    {
        this.Player = player;
        StateName = PLAYERSTATE.MOVEMENT;
    }

    public void OnStateInit()
    {

    }

    public void OnStateEnter()
    {
        b_touchEnd = false;
        EventHandler.TriggerEvent(EEventID.EVENT_PLAYER_STATE_CHANGE, PLAYERSTATE.MOVEMENT);

#if UNITY_ANDROID
        EventHandler.AddListener(EEventID.EVENT_TOUCH_STATIONARY, OnEventTouchStationary);
        EventHandler.AddListener(EEventID.EVENT_TOUCH_END, OnTouchEnd);
        EventHandler.AddListener(EEventID.EVENT_TOUCH_TAP, OnEventTouchTap);
        EventHandler.AddListener(EEventID.EVENT_POINTER_DRAG, OnEventTouchMove);
        EventHandler.AddListener(EEventID.EVENT_TOUCH_START, OnEventTouchStart);
        EventHandler.AddListener(EEventID.EVENT_TOUCH_IMMEDIATE, OnEventTouchImmediate);
#elif UNITY_WEBGL
        EventHandler.AddListener(EEventID.EVENT_KEY_A, OnEventKeyA);
        EventHandler.AddListener(EEventID.EVENT_KEY_D, OnEventKeyD);
        EventHandler.AddListener(EEventID.EVENT_KEY_SPACE, OnEventSpace);
#endif

        m_startVelocity = Player.m_ptrRigidbody.velocity.x;
        m_timeStamp = Time.time;
        m_maxVelocityModifier = 1f;
        GameManager.Instance.m_ptrUIController.OnPlayerStateChange(PLAYERSTATE.MOVEMENT, !b_moveForward);
        GameManager.Instance.m_ptrUIController.ToggleJumpButton(false);

        moveLeftCoords = GameManager.Instance.moveLeftScreenCoords;
        moveRightCoords = GameManager.Instance.moveRightScreenCoords;
    }

    public void OnStateUpdate()
    {
        if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            b_touchEnd = true;

        Player.m_ptrRigidbody.velocity = Vector3.ClampMagnitude(Player.m_ptrRigidbody.velocity, 12);
        if(b_touchEnd && Player.b_Grounded)
        {
            Player.m_ptrRigidbody.velocity = new Vector2(0f, Player.m_ptrRigidbody.velocity.y);
            Player.ChangeState(Player.idleState);
        }

        if(Time.time - m_timeStamp > 0.5f)
        {
            float delta = Mathf.Abs(Player.m_ptrRigidbody.velocity.x - m_startVelocity);
            if (delta < 0.2f)
                m_maxVelocityModifier = 1f;
            else
                m_maxVelocityModifier = 1.5f;


            m_timeStamp = Time.time;
            m_startVelocity = Player.m_ptrRigidbody.velocity.x;
        }
    }

    void OnEventKeyA(System.Object data)
    {
        if (Player.b_Grounded)
            Player.PlayWalk();
        MovePlayer(false);
    }

    void OnEventKeyD(System.Object data)
    {
        if (Player.b_Grounded)
            Player.PlayWalk();
        MovePlayer(true);
    }

    void OnEventSpace(System.Object data)
    {
        Player.ChangeState(Player.jumpState);
    }

    void OnEventTouchStationary(System.Object data)
    {
        Hashtable table = (Hashtable)data;
        int fingerID = (int)table["fingerId"];

        if (Player.m_iPrimaryTouch == fingerID)
        {

            Vector2 touchPos = (Vector2)table["touchPoint"];
            Vector2 WorldPos = Camera.main.ScreenToWorldPoint(touchPos);

            if (Player.b_Grounded)
                Player.PlayWalk();
            Player.m_iPrimaryTouch = fingerID;
            b_touchEnd = false;

            if (WorldPos.x > Player.transform.position.x)
            {
                MovePlayer(true);
                b_moveForward = true;
            }
            else
            {
                MovePlayer(false);
                b_moveForward = false;
            }
        }
    }

    void MovePlayer(bool b_MoveFront)
    {
        int modifier = (b_MoveFront) ? 1 : -1;
        if((modifier == 1) && !(Player.b_flipscale))
        {
            GameManager.Instance.m_ptrUIController.OnPlayerStateChange(PLAYERSTATE.MOVEMENT, !b_MoveFront);
            Player.transform.localScale = new Vector3(Mathf.Abs(Player.playerScale.x), Mathf.Abs(Player.playerScale.y), Mathf.Abs(Player.playerScale.z));
            Player.b_flipscale = !Player.b_flipscale;
        }
        else if((modifier == -1) && (Player.b_flipscale))
        {
            GameManager.Instance.m_ptrUIController.OnPlayerStateChange(PLAYERSTATE.MOVEMENT, !b_MoveFront);
            Player.transform.localScale = new Vector3(-1f * Mathf.Abs(Player.playerScale.x), Player.playerScale.y, Player.playerScale.z);
            Player.b_flipscale = !Player.b_flipscale;
        }
        float x = Mathf.MoveTowards(Player.m_ptrRigidbody.velocity.x, Player.MaxVelocity * modifier, Player.Acceleration * Time.fixedDeltaTime * m_maxVelocityModifier);
        Player.m_ptrRigidbody.velocity = new Vector3(x, Player.m_ptrRigidbody.velocity.y);
        //Debug.LogError(x+ " "+ Player.MaxVelocity * modifier+ " "+ Player.Acceleration * Time.fixedDeltaTime * m_maxVelocityModifier);
    }

    void OnTouchEnd(System.Object data)
    {
        Hashtable table = (Hashtable)data;
        int fingerIndex = (int)table["fingerId"];
        if (fingerIndex == Player.m_iPrimaryTouch)
        {
            Player.m_iPrimaryTouch = -1;
            b_touchEnd = true;
        }
    }

    void OnEventTouchTap(System.Object data)
    {
        /*Hashtable table = (Hashtable)data;
        int touchIndex = (int)table["fingerId"];
        //if (touchIndex != Player.m_iPrimaryTouch)
        {
            Player.ChangeState(Player.jumpState);
        }*/
    }

    void OnEventTouchStart(System.Object data)
    {
        Hashtable table = (Hashtable)data;
        int touchIndex = (int)table["fingerIndex"];
        if (Player.m_iPrimaryTouch == -1)
            Player.m_iPrimaryTouch = touchIndex;
        else
            Player.ChangeState(Player.jumpState);
    }

    void OnEventTouchMove(System.Object data)
    {
        Hashtable table = (Hashtable)data;
        int touchIndex = (int)table["fingerIndex"];
        if (Player.m_iPrimaryTouch == -1)
            return;
        if (touchIndex == Player.m_iPrimaryTouch)
        {
            Vector2 touchPos = (Vector2)table["touchPos"];
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(touchPos);

            if (worldPos.x > Player.transform.position.x)
            {
                b_moveForward = true;
                MovePlayer(true);
            }
            else
            {
                b_moveForward = false;
                MovePlayer(false);
            }
        }
    }

    void OnEventTouchImmediate(System.Object data)
    {
        Player.m_iPrimaryTouch = 0;
        Hashtable table = (Hashtable)data;
        Vector2 touchPos = (Vector2)table["touchPoint"];
        
        if (touchPos.x > moveLeftCoords.x && touchPos.x < moveLeftCoords.y)
        {
            MovePlayer(false);
        }
        if (touchPos.x > moveRightCoords.x && touchPos.x < moveRightCoords.y)
        {
            MovePlayer(true);
        }
        if (Player.b_Grounded)
            Player.PlayWalk();
    }

    public void OnStateExit()
    {
        GameManager.Instance.m_ptrUIController.ToggleJumpButton(true);
        Player.m_iPrimaryTouch = -1;
#if UNITY_ANDROID
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_STATIONARY, OnEventTouchStationary);
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_END, OnTouchEnd);
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_TAP, OnEventTouchTap);
        EventHandler.RemoveListener(EEventID.EVENT_POINTER_DRAG, OnEventTouchMove);
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_START, OnEventTouchStart);
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_IMMEDIATE, OnEventTouchImmediate);
#elif UNITY_WEBGL
        EventHandler.RemoveListener(EEventID.EVENT_KEY_A, OnEventKeyA);
        EventHandler.RemoveListener(EEventID.EVENT_KEY_D, OnEventKeyD);
        EventHandler.RemoveListener(EEventID.EVENT_KEY_SPACE, OnEventSpace);
#endif
    }

    ~PlayerMovementState()
    {
#if UNITY_ANDROID
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_STATIONARY, OnEventTouchStationary);
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_END, OnTouchEnd);
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_TAP, OnEventTouchTap);
        EventHandler.RemoveListener(EEventID.EVENT_POINTER_DRAG, OnEventTouchMove);
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_START, OnEventTouchStart);
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_IMMEDIATE, OnEventTouchImmediate);
#elif UNITY_WEBGL
        EventHandler.RemoveListener(EEventID.EVENT_KEY_A, OnEventKeyA);
        EventHandler.RemoveListener(EEventID.EVENT_KEY_D, OnEventKeyD);
        EventHandler.RemoveListener(EEventID.EVENT_KEY_SPACE, OnEventSpace);
#endif
    }
}
