using UnityEngine;
using System.Collections;

public class StationaryEnemyBehaviour : MonoBehaviour {

    bool b_hasTriggered = false;


    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag.Contains("GesturePrefab"))
        {
            if (coll.contacts[0].normal.y < -0.9f)
            {
                Rigidbody2D rigid = coll.rigidbody;
                if (rigid.mass > 1.5f)
                {
                    if (!b_hasTriggered)
                    {
                        b_hasTriggered = true;
                        iTween.ScaleTo(this.gameObject, Vector3.zero, 0.75f);
                        SoundManager.SharedInstance.PlayClip("EnemyDestroy");
                        Destroy(this.gameObject, 0.8f);
                    }
                }
            }
            else
            {
                EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_OBJET_DESTROY, coll.gameObject);
            }
        }
        if (coll.gameObject.tag == "Player")
        {
            PlayerFSM player = coll.gameObject.GetComponent<PlayerFSM>();
            if (player != null)
            {
                player.ChangeState(player.deadState);
            }
        }
    }

}
