using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PDollarGestureRecognizer;

public class PlayerGestureDrawState : IPlayerState {
    public PLAYERSTATE StateName { get; set; }

    public PlayerFSM Player { get; set; }

    List<Point> GesturePoints;
    List<Gesture> trainingSet = new List<Gesture>();
    List<Vector2> GestureWorldPoints = new List<Vector2>();

    GESTURE_ID[] gesturesToRecognize;

    GameUIController m_ptrUIController;

    public PlayerGestureDrawState(PlayerFSM player)
    {
        this.Player = player;
        StateName = PLAYERSTATE.GESTURE_DRAW;
        GesturePoints = new List<Point>();

        TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/10-stylus-MEDIUM/");
        foreach (TextAsset gestureXml in gesturesXml)
            trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));
    }

    public void OnStateInit()
    {
        var temp = GestureEventHandler.Instance.gesturesAndActions;
        gesturesToRecognize = new GESTURE_ID[temp.Length];
        for(int i=0;i<temp.Length;++i)
        {
            gesturesToRecognize[i] = temp[i].gesture;
        }

        m_ptrUIController = GameObject.FindObjectOfType<GameUIController>();
    }

    public void OnStateEnter()
    {
        GameManager.Instance.m_ptrUIController.OnPlayerStateChange(PLAYERSTATE.GESTURE_DRAW, false);
        EventHandler.TriggerEvent(EEventID.EVENT_PLAYER_STATE_CHANGE, PLAYERSTATE.GESTURE_DRAW);

        var temp = GestureEventHandler.Instance.gesturesAndActions;
        gesturesToRecognize = new GESTURE_ID[temp.Length];
        for (int i = 0; i < temp.Length; ++i)
        {
            gesturesToRecognize[i] = temp[i].gesture;
        }

#if UNITY_ANDROID
        EventHandler.AddListener(EEventID.EVENT_POINTER_DRAG, OnEventPointerDrag);
        EventHandler.AddListener(EEventID.EVENT_TOUCH_END, OnEventTouchEnd);
#elif UNITY_WEBGL
        EventHandler.AddListener(EEventID.EVENT_POINTER_DRAG, OnEventPointerDrag);
        EventHandler.AddListener(EEventID.EVENT_MOUSE_END, OnEventTouchEnd);
#endif

        Hashtable table = new Hashtable();
        table.Add("enable", true);
        EventHandler.TriggerEvent(EEventID.EVENT_TOGGLE_DRAW_TRAIL, table);
    }

    public void OnStateUpdate()
    {

    }

    void OnEventPointerDrag(System.Object data)
    {
        Hashtable table = (Hashtable)data;
        Vector2 TouchPos = (Vector2)table["touchPos"];
        RegisterPoint(TouchPos);
    }

    void RegisterPoint(Vector2 TouchPos)
    {
        GesturePoints.Add(new Point(TouchPos.x, -TouchPos.y, 0));
        Vector2 WorldPos = Camera.main.ScreenToWorldPoint(TouchPos);
        if (GestureWorldPoints.Count == 0 || (GestureWorldPoints.Count > 0 && Vector2.Distance(WorldPos, GestureWorldPoints[GestureWorldPoints.Count - 1]) > 0.25f))
        {
            GestureWorldPoints.Add(Camera.main.ScreenToWorldPoint(TouchPos));
        }
    }

    void OnEventTouchEnd(System.Object data)
    {
        if (GesturePoints.Count > 10)
        {
            try
            {
                Gesture candidate = new Gesture(GesturePoints.ToArray());
                Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());
                TriggerGestureEvents(gestureResult);
            }
            catch(System.Exception e)
            {
                Debug.LogError(e.Message + " " +e.StackTrace);
                Player.ChangeState(Player.idleState);
            }
        }
        else
        {
            GestureWorldPoints.Clear();
        }
        GesturePoints.Clear();

        Player.ChangeState(Player.idleState);
    }

    void TriggerGestureEvents(Result result)
    {

        if (GameUIController.maxCount > 0)
        {
            if (result.Score > 0.85f)
            {
                if (result.GestureClass.Contains("triangle"))
                {
                    foreach (var gesture in gesturesToRecognize)
                    {
                        if (gesture == GESTURE_ID.TRIANGLE)
                        {
                            Vector2[] vertexPoints = new Vector2[3];
                            int index = GestureWorldPoints.Count / 3;
                            for (int j = 0; j < vertexPoints.Length; ++j)
                            {
                                vertexPoints[j] = GestureWorldPoints[index * j];
                            }
                            EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_TRIANGLE, vertexPoints);
                            Player.ChangeState(Player.idleState);
                        }
                    }
                }
                else if (result.GestureClass.Contains("circle"))
                {
                    foreach (var gesture in gesturesToRecognize)
                    {
                        if (gesture == GESTURE_ID.CIRCLE)
                        {
                            Vector2[] DiameterPoints = new Vector2[2];
                            int index = GestureWorldPoints.Count / 2;
                            DiameterPoints[0] = GestureWorldPoints[0];
                            DiameterPoints[1] = GestureWorldPoints[index];
                            EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_CIRCLE, DiameterPoints);
                            Player.ChangeState(Player.idleState);
                        }
                    }
                }
                else if (result.GestureClass.Contains("rectangle"))
                {
                    foreach (var gesture in gesturesToRecognize)
                    {
                        if (gesture == GESTURE_ID.SQUARE)
                        {
                            Vector2[] VertexPoints = new Vector2[4];
                            int index = GestureWorldPoints.Count / 4;
                            for (int j = 0; j < VertexPoints.Length; ++j)
                            {
                                VertexPoints[j] = GestureWorldPoints[index * j];
                            }
                            EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_SQUARE, VertexPoints);
                            Player.ChangeState(Player.idleState);
                        }
                    }
                }
            }
        }
        else
        {
            iTween.PunchScale(GameUIController.Instance.m_PromptText.gameObject, Vector3.one * 1.5f, 0.75f);
            GameUIController.Instance.m_PromptText.text = "Woops! Can count to just 3! Remember to collect back dream objects";
            SoundManager.SharedInstance.PlayClip("Alert");
            Player.StartCoroutine(Player.ResetText());
            Player.ChangeState(Player.idleState);
        }
        GestureWorldPoints.Clear();
    }

    public void RemoveTouchEnd()
    {
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_END, OnEventTouchEnd);
    }

    public void OnStateExit()
    {
#if UNITY_ANDROID
        EventHandler.RemoveListener(EEventID.EVENT_POINTER_DRAG, OnEventPointerDrag);
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_END, OnEventTouchEnd);
#elif UNITY_WEBGL
        EventHandler.RemoveListener(EEventID.EVENT_POINTER_DRAG, OnEventPointerDrag);
        EventHandler.RemoveListener(EEventID.EVENT_MOUSE_END, OnEventTouchEnd);
#endif


        Hashtable table = new Hashtable();
        table.Add("enable", false);
        EventHandler.TriggerEvent(EEventID.EVENT_TOGGLE_DRAW_TRAIL, table);
    }

    ~PlayerGestureDrawState()
    {
#if UNITY_ANDROID
        EventHandler.RemoveListener(EEventID.EVENT_POINTER_DRAG, OnEventPointerDrag);
        EventHandler.RemoveListener(EEventID.EVENT_TOUCH_END, OnEventTouchEnd);
#elif UNITY_WEBGL
        EventHandler.RemoveListener(EEventID.EVENT_POINTER_DRAG, OnEventPointerDrag);
        EventHandler.RemoveListener(EEventID.EVENT_MOUSE_END, OnEventTouchEnd);
#endif
    }
}
