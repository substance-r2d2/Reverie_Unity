using UnityEngine;
using System.Collections;

public class DetectChildCollision : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Player")
            EventHandler.TriggerEvent(EEventID.EVENT_CHILD_COLLISION, other.collider);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
            EventHandler.TriggerEvent(EEventID.EVENT_CHILD_TRIGGER, other);
    }
}
