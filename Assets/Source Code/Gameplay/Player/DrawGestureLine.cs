using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DrawGestureLine : MonoBehaviour {

    GameObject effectObj;
    Vector2 targetPos;
    bool b_setInitPos;

	// Use this for initialization
	void Start ()
    {
        EventHandler.AddListener(EEventID.EVENT_TOGGLE_DRAW_TRAIL, OnEventToggleDrawTrail);
        effectObj = this.transform.GetChild(0).gameObject;
        effectObj.SetActive(false);
    }

    void OnEventToggleDrawTrail(System.Object data)
    {
        Hashtable table = (Hashtable)data;
        bool b_enable = (bool)table["enable"];
        if(b_enable)
        {
            //effectObj.SetActive(true);
            AddSwipeListener();
        }
        else
        {
            if (IsInvoking("EnableEffectObj"))
                CancelInvoke("EnableEffectObj");
            effectObj.SetActive(false);
            EventHandler.RemoveListener(EEventID.EVENT_POINTER_DRAG, OnEventTouchSwipe);
        }
    }

    void AddSwipeListener()
    {
        EventHandler.AddListener(EEventID.EVENT_POINTER_DRAG, OnEventTouchSwipe);
    }

    void OnEventTouchSwipe(System.Object data)
    {
        Hashtable table = (Hashtable)data;
        Vector2 TouchPos = (Vector2)table["touchPos"];
        Vector2 WorldPos = Camera.main.ScreenToWorldPoint(TouchPos);
        if (!IsInvoking("EnableEffectObj"))
            Invoke("EnableEffectObj", 2 * Time.fixedDeltaTime);
        transform.position = WorldPos;
    }

    void EnableEffectObj()
    {
        effectObj.SetActive(true);
    }

    void OnDestroy()
    {
        EventHandler.RemoveListener(EEventID.EVENT_POINTER_DRAG, OnEventTouchSwipe);
        EventHandler.RemoveListener(EEventID.EVENT_TOGGLE_DRAW_TRAIL, OnEventToggleDrawTrail);
    }

}
