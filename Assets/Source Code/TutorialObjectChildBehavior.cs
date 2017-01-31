using UnityEngine;
using System.Collections;

public class TutorialObjectChildBehavior : MonoBehaviour
{
    bool b_triggered = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" && !b_triggered)
        {
            b_triggered = true;
            EventHandler.TriggerEvent(EEventID.EVENT_TRIGGER_TUTORIAL, transform.parent);
        }
    }

}
