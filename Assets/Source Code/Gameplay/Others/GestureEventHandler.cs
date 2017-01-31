using UnityEngine;
using System.Collections;

public class GestureEventHandler : MonoBehaviour {

    public static GestureEventHandler Instance;
    public GesturesAndActions[] gesturesAndActions;
    public float AnimTime = 1f;

    void OnEnable()
    {
        Instance = this;
    }

	// Use this for initialization
	void Start ()
    {
        EventHandler.AddListener(EEventID.EVENT_GESTURE_CIRCLE, OnEventGestureCircle);
        EventHandler.AddListener(EEventID.EVENT_GESTURE_SQUARE, OnEventGestureSquare);
        EventHandler.AddListener(EEventID.EVENT_GESTURE_TRIANGLE, OnEventGestureTriangle);
        EventHandler.AddListener(EEventID.EVENT_SET_GESTURE_PREFAB, OnEventSetGesturePrefab);
	}

    void OnEventSetGesturePrefab(System.Object data)
    {
        Hashtable table = (Hashtable)data;
        GESTURE_ID id = (GESTURE_ID)table["id"];
        foreach (var gesture in gesturesAndActions)
        {
            if(gesture.gesture == id)
            {
                gesture.prefab = null;
                string prefabID = (string)table["prefab"];
                gesture.prefab = Resources.Load("Prefabs/Gesture Prefabs/"+prefabID) as GameObject;
                break;
            }
        }
    }

    void OnEventGestureCircle(System.Object data)
    {
        foreach (var gesture in gesturesAndActions)
        {
            if (gesture.gesture == GESTURE_ID.CIRCLE)
            {
                Vector2[] WorldDiameterPoints = (Vector2[])data;

                float radius = Vector2.Distance(WorldDiameterPoints[0], WorldDiameterPoints[1]) / 2f;
                Vector2 MiddlePos = ((WorldDiameterPoints[0] + WorldDiameterPoints[1]) / 2f);
                Vector2 centerPos = new Vector2(MiddlePos.x, MiddlePos.y);

                radius = Mathf.Clamp(radius, 0f, 3.2f);
                Vector3 targetScale = new Vector3(radius, radius, 0);

                GameObject circlePrefab = GameObject.Instantiate(gesture.prefab, centerPos, Quaternion.identity) as GameObject;
                circlePrefab.transform.localScale = targetScale;
                //Vibration.Vibrate(250);
#if UNITY_ANDROID
                Handheld.Vibrate();
#endif
                //EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_OBJECT_CREATE, null);
            }
        }
    }

    void OnEventGestureSquare(System.Object data)
    {

        foreach (var gesture in gesturesAndActions)
        {
            if (gesture.gesture == GESTURE_ID.SQUARE)
            {
                Vector2[] WorldVertexPoints = (Vector2[])data;
                float radius = Vector2.Distance(WorldVertexPoints[0], WorldVertexPoints[2]) / 2f;
                Vector2 MiddlePos = ((WorldVertexPoints[0] + WorldVertexPoints[2]) / 2f);

                radius = Mathf.Clamp(radius, 0f, 1f);
                Vector3 targetScale = new Vector3(radius, radius, 0);
                GameObject squarePrefab = GameObject.Instantiate(gesture.prefab, MiddlePos, Quaternion.identity) as GameObject;
                squarePrefab.transform.localScale = Vector3.zero;
                iTween.ScaleTo(squarePrefab, targetScale, AnimTime);
#if UNITY_ANDROID
                Handheld.Vibrate();
#endif
                //Vibration.Vibrate(250);
                //EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_OBJECT_CREATE, null);
            }
        }
    }

    void OnEventGestureTriangle(System.Object data)
    {

        foreach (var gesture in gesturesAndActions)
        {
            if (gesture.gesture == GESTURE_ID.TRIANGLE)
            {
                Vector2[] VertexPoints = (Vector2[])data;
                Vector2 Centroid = Vector2.zero;
                foreach (var point in VertexPoints)
                {
                    Centroid += point;
                }
                Centroid = Centroid / 3f;

                GameObject TrianglePrefab = GameObject.Instantiate(gesture.prefab, Centroid, Quaternion.identity) as GameObject;
                TrianglePrefab.transform.localScale = Vector3.zero;
                iTween.ScaleTo(TrianglePrefab, new Vector3(1.5f,1.5f,1.5f), AnimTime/2f);
#if UNITY_ANDROID
                Handheld.Vibrate();
#endif
                //Vibration.Vibrate(250);
                //EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_OBJECT_CREATE, null);
            }
        }
    }

    void OnDestroy()
    {
        EventHandler.RemoveListener(EEventID.EVENT_GESTURE_CIRCLE, OnEventGestureCircle);
        EventHandler.RemoveListener(EEventID.EVENT_GESTURE_SQUARE, OnEventGestureSquare);
        EventHandler.RemoveListener(EEventID.EVENT_GESTURE_TRIANGLE, OnEventGestureTriangle);

    }
}

[System.Serializable]
public class GesturesAndActions
{
    public GESTURE_ID gesture;
    public GameObject prefab;
}

public enum GESTURE_ID
{
    CIRCLE,
    TRIANGLE,
    SQUARE,
}
