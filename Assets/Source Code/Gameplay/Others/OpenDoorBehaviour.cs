using UnityEngine;
using System.Collections;

public class OpenDoorBehaviour : MonoBehaviour {

    public float DefaultValue = 90f;
    public float MoveToValue = 0f;
    public Transform Door;

    bool b_LowerBridge = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player" || other.tag == "GesturePrefab")
            b_LowerBridge = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player" || other.tag == "GesturePrefab")
            b_LowerBridge = false;
    }

    void Update()
    {
        if(b_LowerBridge)
        {
            iTween.RotateUpdate(Door.gameObject, Vector3.zero, 10f);
            if (Door.transform.localEulerAngles.z < 15f)
                Destroy(this.gameObject);
        }
        else if (Door.transform.localEulerAngles.z > 15f)
        {
            iTween.RotateTo(Door.gameObject, new Vector3(0f, 0f, 90f), 1f);
        }
    }

}
