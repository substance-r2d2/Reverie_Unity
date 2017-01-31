using UnityEngine;
using System.Collections;

public class UpdateSavePoint : MonoBehaviour {

    GameObject GuidingLight;
    public float AnimTime = 1f;
    bool b_Hidden;

    void Start()
    {
        if(transform.childCount > 0)
            GuidingLight = transform.GetChild(0).gameObject;
        b_Hidden = false;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            if (!IsInvoking("ScaleToZero") && !b_Hidden)
            {
                SoundManager.SharedInstance.PlayClip("Collect");
                b_Hidden = true;
                EventHandler.TriggerEvent(EEventID.EVENT_UPDATE_SAVE_POSITION, (Vector2)other.transform.position);
                if (GuidingLight != null)
                {
                    iTween.MoveBy(GuidingLight, new Vector3(0f, 1.5f), AnimTime);
                    iTween.PunchScale(GuidingLight, new Vector3(2f, 2f, 2f), 1.5f * AnimTime);
                }
                else
                {
                    iTween.MoveBy(this.gameObject, new Vector3(0f, 1.5f), AnimTime);
                    iTween.PunchScale(this.gameObject, new Vector3(2f, 2f, 2f), 1.5f * AnimTime);
                }
                Invoke("ScaleToZero", 2f * AnimTime);
            }
        }
    }

    void ScaleToZero()
    {
        if (GuidingLight != null)
            iTween.ScaleTo(GuidingLight, Vector3.zero, 0.25f);
        else
            iTween.ScaleTo(this.gameObject, Vector3.zero, 0.25f);
    }

}
