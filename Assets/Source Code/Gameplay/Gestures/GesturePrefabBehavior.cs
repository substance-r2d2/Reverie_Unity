using UnityEngine;
using System.Collections;

public class GesturePrefabBehavior : MonoBehaviour
{
    public PREFAB_TYPEID type_id;
    public bool b_destroyOnTouch;

    void Start()
    {
        if(b_destroyOnTouch)
            EventHandler.AddListener(EEventID.EVENT_GESTURE_OBJET_DESTROY, OnEventDestroyPrefab);

        EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_OBJECT_CREATE, type_id);
        EventHandler.TriggerEvent(EEventID.EVENT_UPDATE_GESTURE_UI, 1);
    }

    void OnEventDestroyPrefab(System.Object data)
    {
        GameObject obj = (GameObject)data;
        if (obj == this.gameObject)
        {
            iTween.ScaleTo(this.gameObject, Vector3.zero, 0.75f);
            Destroy(this.gameObject, 0.8f);
        }
    }


    void OnDestroy()
    {
        EventHandler.TriggerEvent(EEventID.EVENT_UPDATE_GESTURE_UI, -1);
        if (b_destroyOnTouch)
            EventHandler.RemoveListener(EEventID.EVENT_GESTURE_OBJET_DESTROY, OnEventDestroyPrefab);
    }
}
