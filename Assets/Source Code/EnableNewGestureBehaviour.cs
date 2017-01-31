using UnityEngine;
using System.Collections;

public class EnableNewGestureBehaviour : MonoBehaviour {

    public GESTURE_ID id;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            SoundManager.SharedInstance.PlayClip("Collect");
            EventHandler.TriggerEvent(EEventID.EVENT_ENABLE_GESTURE_DETECTION, id);
            Destroy(GetComponent<Animator>());
            iTween.ScaleTo(this.gameObject, Vector3.zero, 0.75f);
            Destroy(gameObject,0.8f);
        }
    }

}
