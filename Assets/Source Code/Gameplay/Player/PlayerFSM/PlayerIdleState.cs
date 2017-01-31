using UnityEngine;
using System.Collections;

public class PlayerIdleState : IPlayerState
{
    public PLAYERSTATE StateName { get; set; }

    public PlayerFSM Player { get; set; }

    Vector2 moveLeftCoords;
    Vector2 moveRightCoords;

    public PlayerIdleState(PlayerFSM player)
    {
        this.Player = player;
        StateName = PLAYERSTATE.IDLE;
    }

    public void OnStateInit()
    {

    }

    public void OnStateEnter()
    {
        EventHandler.TriggerEvent(EEventID.EVENT_PLAYER_STATE_CHANGE, PLAYERSTATE.IDLE);

#if UNTIY_ANDROID
        EventHandler.AddListener(EEventID.EVENT_TOUCH_STATIONARY, OnEventTouchStationary);
        EventHandler.AddListener(EEventID.EVENT_TOUCH_IMMEDIATE, OnEventTouchImmediate);
        EventHandler.AddListener(EEventID.EVENT_POINTER_DRAG, OnEventTouchDrag);
        EventHandler.AddListener(EEventID.EVENT_JUMP, OnEventTouchTap);
        EventHandler.AddListener(EEventID.EVENT_TOUCH_START, OnEventTouchStart);
#elif UNITY_WEBGL
        EventHandler.AddListener(EEventID.EVENT_KEY_A, OnEventKeyInput);
        EventHandler.AddListener(EEventID.EVENT_KEY_D, OnEventKeyInput);
        EventHandler.AddListener(EEventID.EVENT_KEY_SPACE, OnEventTouchTap);
        EventHandler.AddListener(EEventID.EVENT_MOUSE_START, OnEventTouchStart);
        EventHandler.AddListener(EEventID.EVENT_POINTER_DRAG, OnEventTouchDrag);
#endif
        Player.PlayIdle();

        GameManager.Instance.m_ptrUIController.OnPlayerStateChange(PLAYERSTATE.IDLE, false);

        moveLeftCoords = GameManager.Instance.moveLeftScreenCoords;
        moveRightCoords = GameManager.Instance.moveRightScreenCoords;
    }

    public void OnStateUpdate()
    {

    }

    void OnEventTouchStationary(System.Object data)
    {
        Hashtable table = (Hashtable)data;
        int touchIndex = (int)table["fingerId"];
        Player.m_iPrimaryTouch = touchIndex;
        Player.ChangeState(Player.movementState);
    }

    void OnEventTouchTap(System.Object data)
    {
        /*Hashtable table = (Hashtable)data;
        Vector2 TouchPos = (Vector2)table["endPoint"];
        Vector2 WorldPos = Camera.main.ScreenToWorldPoint(TouchPos);
        RaycastHit2D[] hits = Physics2D.RaycastAll(WorldPos, Vector2.zero);
        bool b_changeToJump = true;

        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.collider.tag.Contains("GesturePrefab"))
                {
                    b_changeToJump = false;
                    EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_OBJET_DESTROY, hit.collider.gameObject);
                    break;
                }
            }
        }*/

        bool b_changeToJump = true;

        if (b_changeToJump)
        {
            Player.ChangeState(Player.jumpState);
        }
    }

    void OnEventTouchStart(System.Object data)
    {
        Hashtable table = (Hashtable)data;
        Vector2 screenPos = (Vector2)table["touchPoint"];
        Vector2 WorldPos = Camera.main.ScreenToWorldPoint(screenPos);
        RaycastHit2D[] hits = Physics2D.RaycastAll(WorldPos, Vector2.zero);
        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                if (hit.collider.tag.Contains("GesturePrefab"))
                {
                    EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_OBJET_DESTROY, hit.collider.gameObject);
                    break;
                }
            }
        }
    }

    void OnEventTouchDrag(System.Object data)
    {
        Player.ChangeState(Player.gestureState);
    }

    void OnEventTouchImmediate(System.Object data)
    {
        Hashtable table = (Hashtable)data;
        Vector2 touchPos = (Vector2)table["touchPoint"];
        if (touchPos.x > moveLeftCoords.x && touchPos.x < moveLeftCoords.y)
            Player.ChangeState(Player.movementState);
        if (touchPos.x > moveRightCoords.x && touchPos.x < moveRightCoords.y)
            Player.ChangeState(Player.movementState);
    }

    void OnEventKeyInput(System.Object data)
    {
        Player.ChangeState(Player.movementState);
    }

    public void OnStateExit()
    {
#if UNITY_ANDROID
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_STATIONARY, OnEventTouchStationary);
        EventHandler.RemoveListener(EEventID.EVENT_POINTER_DRAG, OnEventTouchDrag);
        EventHandler.RemoveListener(EEventID.EVENT_JUMP, OnEventTouchTap);
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_START, OnEventTouchStart);
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_IMMEDIATE, OnEventTouchImmediate);
#elif UNITY_WEBGL
        EventHandler.RemoveListener(EEventID.EVENT_KEY_A, OnEventKeyInput);
        EventHandler.RemoveListener(EEventID.EVENT_KEY_D, OnEventKeyInput);
        EventHandler.RemoveListener(EEventID.EVENT_KEY_SPACE, OnEventTouchTap);
        EventHandler.RemoveListener(EEventID.EVENT_MOUSE_START, OnEventTouchStart);
        EventHandler.RemoveListener(EEventID.EVENT_POINTER_DRAG, OnEventTouchDrag);
#endif
    }

    ~PlayerIdleState()
    {
#if UNITY_ANDROID
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_STATIONARY, OnEventTouchStationary);
        EventHandler.RemoveListener(EEventID.EVENT_POINTER_DRAG, OnEventTouchDrag);
        EventHandler.RemoveListener(EEventID.EVENT_JUMP, OnEventTouchTap);
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_START, OnEventTouchStart);
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_IMMEDIATE, OnEventTouchImmediate);
#elif UNITY_WEBGL
        EventHandler.RemoveListener(EEventID.EVENT_KEY_A, OnEventKeyInput);
        EventHandler.RemoveListener(EEventID.EVENT_KEY_D, OnEventKeyInput);
        EventHandler.RemoveListener(EEventID.EVENT_KEY_SPACE, OnEventTouchTap);
        EventHandler.RemoveListener(EEventID.EVENT_MOUSE_START, OnEventTouchStart);
        EventHandler.RemoveListener(EEventID.EVENT_POINTER_DRAG, OnEventTouchDrag);
#endif
    }
}
