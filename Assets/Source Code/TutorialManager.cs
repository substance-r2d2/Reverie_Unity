using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public TutorialEventID tut;
    Animator m_anim;
    Transform m_playerPos;
    Text m_PromptText;

    static int CURRENT_TUTORIAL = 0;

    void Start()
    {
        m_anim = GetComponent<Animator>();
        EventHandler.AddListener(EEventID.EVENT_TRIGGER_TUTORIAL, OnEventTriggerEvent);
        m_playerPos = GameObject.FindGameObjectWithTag("Player").transform;
        m_PromptText = GameObject.Find("TextPrompt").GetComponent<Text>();
    }

    void OnLevelWasLoaded(int currentLevel)
    {
        if (currentLevel == (int)SCENEID.LEVEL_1_1)
        {
            CURRENT_TUTORIAL = 0;
        }
    }

    void OnEventTriggerEvent(System.Object data)
    {
        Transform t = (Transform)data;
        if(t == this.transform)
        {
            switch(tut)
            {
                case TutorialEventID.WALK_LEFT:
                    //if (CURRENT_TUTORIAL == 2)
                    {
                        m_PromptText.text = "Hold stationary touch on left to start moving";
                        EventHandler.TriggerEvent(EEventID.EVENT_PLAYER_STATE_UPDATE, PLAYERSTATE.LOCK);
                        m_anim.Play("walk_left");
                        EventHandler.AddListener(EEventID.EVENT_TOUCH_STATIONARY, OnEventTouchStationaryLeft);
                    }
                    break;

                case TutorialEventID.WALK_RIGHT:
                    //if (CURRENT_TUTORIAL == 0)
                    {
                        m_PromptText.text = "Hold stationary touch on right to start moving";
                        EventHandler.TriggerEvent(EEventID.EVENT_PLAYER_STATE_UPDATE, PLAYERSTATE.LOCK);
                        m_anim.Play("walk_right");
                        EventHandler.AddListener(EEventID.EVENT_TOUCH_STATIONARY, OnEventTouchStationary);
                    }
                    break;

                case TutorialEventID.JUMP:
                    //if(CURRENT_TUTORIAL == 1)
                    {
                        EventHandler.TriggerEvent(EEventID.EVENT_PLAYER_STATE_UPDATE, PLAYERSTATE.LOCK);
                        m_anim.Play("tap");
                        EventHandler.AddListener(EEventID.EVENT_TOUCH_TAP, OnEventTap);
                    }
                    break;

                case TutorialEventID.DRAW_CIRCLE:
                    break;

                case TutorialEventID.DRAW_SQUARE:
                    //if (CURRENT_TUTORIAL == 0)
                    {
                        m_PromptText.text = "Draw a square nice and slow";
                        EventHandler.TriggerEvent(EEventID.EVENT_PLAYER_STATE_UPDATE, PLAYERSTATE.LOCK);
                        m_anim.Play("draw_square");
                        EventHandler.AddListener(EEventID.EVENT_GESTURE_SQUARE, OnEventGestureSquare);
                    }
                    break;

                case TutorialEventID.DRAW_TRIANGLE:
                    m_PromptText.text = "Draw and drop a triangle on the enemy";
                    EventHandler.TriggerEvent(EEventID.EVENT_PLAYER_STATE_UPDATE, PLAYERSTATE.GESTURE_DRAW);
                    m_anim.Play("draw_triangle");
                    EventHandler.AddListener(EEventID.EVENT_GESTURE_TRIANGLE, OnEventGestureTriangle);
                    break;

                case TutorialEventID.TAP_ON_PREFAB:
                    //if (CURRENT_TUTORIAL == 4)
                    {
                        m_PromptText.text = "Remember to collect back the spawned object!";
                        EventHandler.TriggerEvent(EEventID.EVENT_PLAYER_STATE_UPDATE, PLAYERSTATE.IDLE);
                        m_anim.Play("tap_on_gesture");
                        EventHandler.AddListener(EEventID.EVENT_UPDATE_GESTURE_UI, UpdateGestureUI);
                    }
                    break;
            }
        }
    }

    void OnEventTouchStationary(System.Object data)
    {
        m_PromptText.text = "";
        ++CURRENT_TUTORIAL;
        m_anim.Play("none");
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_STATIONARY, OnEventTouchStationary);
        EventHandler.TriggerEvent(EEventID.EVENT_PLAYER_STATE_UPDATE, PLAYERSTATE.IDLE);
        Destroy(this.gameObject);
    }

    void UpdateGestureUI(System.Object data)
    {
        m_PromptText.text = "";
        ++CURRENT_TUTORIAL;
        m_anim.Play("none");
        EventHandler.RemoveListener(EEventID.EVENT_UPDATE_GESTURE_UI, UpdateGestureUI);
        EventHandler.TriggerEvent(EEventID.EVENT_PLAYER_STATE_UPDATE, PLAYERSTATE.IDLE);
        Destroy(this.gameObject);
    }

    void OnEventTouchStationaryLeft(System.Object data)
    {
        Hashtable table = (Hashtable)data;
        Vector2 touchPos = (Vector2)table["touchPoint"];
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(touchPos);
        if (worldPos.x < m_playerPos.position.x)
        {
            m_PromptText.text = "";
            ++CURRENT_TUTORIAL;
            m_anim.Play("none");
            EventHandler.RemoveListener(EEventID.EVENT_TOUCH_STATIONARY, OnEventTouchStationaryLeft);
            EventHandler.TriggerEvent(EEventID.EVENT_PLAYER_STATE_UPDATE, PLAYERSTATE.IDLE);
            Destroy(this.gameObject);
        }
    }

    void OnEventTap(System.Object data)
    {
        ++CURRENT_TUTORIAL;
        m_PromptText.text = "";
        m_anim.Play("none");
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_TAP, OnEventTap);
        EventHandler.TriggerEvent(EEventID.EVENT_PLAYER_STATE_UPDATE, PLAYERSTATE.JUMP);
        Destroy(this.gameObject);
    }

    void OnEventGestureSquare(System.Object data)
    {
        ++CURRENT_TUTORIAL;
        m_PromptText.text = "";
        m_anim.Play("none");
        EventHandler.RemoveListener(EEventID.EVENT_GESTURE_SQUARE, OnEventGestureSquare);
        EventHandler.TriggerEvent(EEventID.EVENT_PLAYER_STATE_UPDATE, PLAYERSTATE.IDLE);
        Destroy(this.gameObject);
    }

    void OnEventGestureTriangle(System.Object data)
    {
        m_anim.Play("none");
        m_PromptText.text = "";
        EventHandler.RemoveListener(EEventID.EVENT_GESTURE_TRIANGLE, OnEventGestureTriangle);
        EventHandler.TriggerEvent(EEventID.EVENT_PLAYER_STATE_UPDATE, PLAYERSTATE.IDLE);
        Destroy(this.gameObject);
    }

    void OnDestroy()
    {
        EventHandler.RemoveListener(EEventID.EVENT_TRIGGER_TUTORIAL, OnEventTriggerEvent);
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_STATIONARY, OnEventTouchStationary);
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_STATIONARY, OnEventTouchStationaryLeft);
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_TAP, OnEventTap);
        EventHandler.RemoveListener(EEventID.EVENT_GESTURE_SQUARE, OnEventGestureSquare);
        EventHandler.RemoveListener(EEventID.EVENT_GESTURE_TRIANGLE, OnEventGestureTriangle);
        EventHandler.RemoveListener(EEventID.EVENT_UPDATE_GESTURE_UI, UpdateGestureUI);
    }
}

public enum TutorialEventID
{
    WALK_RIGHT,
    WALK_LEFT,
    JUMP,
    DRAW_SQUARE,
    DRAW_TRIANGLE,
    DRAW_CIRCLE,
    TAP_ON_PREFAB,
}
