using UnityEngine;
using System.Collections;

public class DestroyGesturePrefab : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag.Contains("GesturePrefab"))
        {
            iTween.ScaleTo(other.gameObject, Vector3.zero, 0.75f);
            Destroy(other.gameObject, 0.8f);
            EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_OBJET_DESTROY, null);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag.Contains("GesturePrefab"))
        {
            iTween.ScaleTo(other.gameObject, Vector3.zero, 0.75f);
            Destroy(other.gameObject, 0.8f);
            EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_OBJET_DESTROY, null);
        }
    }

}
