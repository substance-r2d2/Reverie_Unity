using UnityEngine;
using System.Collections;

public class KeyboardInputController
{

    float mouseClickStartTime;
    Vector2 mouseClickStartPos;

    public KeyboardInputController()
    {

    }

    public void Update()
    {
        if(Input.GetKey(KeyCode.A))
        {
            EventHandler.TriggerEvent(EEventID.EVENT_KEY_A, null);
        }

        if(Input.GetKey(KeyCode.D))
        {
            EventHandler.TriggerEvent(EEventID.EVENT_KEY_D, null);
        }

        if(Input.GetKeyDown(KeyCode.Space))
        {
            EventHandler.TriggerEvent(EEventID.EVENT_KEY_SPACE, null);
        }

        if(Input.GetMouseButtonDown(0))
        {
            Hashtable table = new Hashtable();
            table.Add("touchPoint", (Vector2)Input.mousePosition);
            EventHandler.TriggerEvent(EEventID.EVENT_MOUSE_START, table);
            mouseClickStartTime = Time.time;
            mouseClickStartPos = Input.mousePosition;
        }

        if(Input.GetMouseButton(0))
        {
            if(Vector2.Distance(mouseClickStartPos,Input.mousePosition) > 0.5f)
            {
                Hashtable table = new Hashtable();
                table.Add("touchPos", (Vector2)Input.mousePosition);
                EventHandler.TriggerEvent(EEventID.EVENT_POINTER_DRAG, table);
            }
        }

        if(Input.GetMouseButtonUp(0))
        {
            EventHandler.TriggerEvent(EEventID.EVENT_MOUSE_END, Input.mousePosition);
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            EventHandler.TriggerEvent(EEventID.EVENT_KEY_ESCAPE, null);
        }
    }

}
