using UnityEngine;
using System.Collections;

public class DestroyTrianglePrefab : MonoBehaviour {

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag.Contains("GesturePrefab"))
        {
            if (other.rigidbody.mass > 1.5f)
            {
                iTween.ScaleTo(other.gameObject, Vector3.zero, 0.75f);
                Destroy(other.gameObject, 0.8f);
                EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_OBJET_DESTROY, null);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag.Contains("GesturePrefab"))
        {
            Rigidbody2D rigid = other.gameObject.GetComponent<Rigidbody2D>();
            if (rigid.mass > 1.5f)
            {
                iTween.ScaleTo(other.gameObject, Vector3.zero, 0.75f);
                Destroy(other.gameObject, 0.8f);
                EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_OBJET_DESTROY, null);
            }
        }
    }
}
