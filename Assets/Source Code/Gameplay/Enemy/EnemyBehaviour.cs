using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum EnemyState
{
	STOP= 0,
	START,
	FOUNDPLAYER,
	MOVE,
	END,
	DESTROY,
}

public enum EnemySubState
{
	FORWARD = 0,
	REVERSE
}
public class EnemyBehaviour : MonoBehaviour
{
    public bool b_stationary;
    public int ID;
    public EnemyState m_state;
    public EnemySubState m_subState;
    private List<GameObject> m_ptrBlockersList;
    private Vector2[] OriginalPathPoints;
    float rate;
    float time;
    int index;
    int count;

    Vector2 startPos;
    Vector2 endPos;

    public Vector2[] pathPoints;
    //Vector2[] reversePoints;

    public float speed = 0.64f;

    // Use this for initialization
    void Start()
    {

        EventHandler.AddListener(EEventID.EVENT_DESTROY_ENEMY, OnEventDestroy);

        rate = 0;
        index = 0;
        if(pathPoints.Length > 1)
         time = Vector2.Distance(pathPoints[0], pathPoints[1]);
        speed = 0;
        m_subState = EnemySubState.FORWARD;
        if (b_stationary)
            m_state = EnemyState.STOP;
        else
            m_state = EnemyState.MOVE;
        startPos = transform.position;
        endPos = transform.position;
        OriginalPathPoints = pathPoints;
        m_ptrBlockersList = new List<GameObject>();
    }


    void Init()
    {
        rate = 0;
        index = 0;
        if (pathPoints.Length > 1)
            time = Vector2.Distance(pathPoints[0], pathPoints[1]);
        speed = 0;
        //m_subState = EnemySubState.FORWARD;
        m_state = EnemyState.MOVE;
    }

    public void OnShapeDestroy(System.Object data)
    {
        GameObject destroyedShape = (GameObject)data;
        if (m_ptrBlockersList.Contains(destroyedShape))
        {
            m_ptrBlockersList.Remove(destroyedShape);
            SetDefaultPathPoints();
        }
    }
    public void SetDefaultPathPoints()
    {
        pathPoints = OriginalPathPoints;
    }
    public void OnMove()
    {
        Init();
        pathPoints = ReverseMovingPlatformData(pathPoints);
        startPos = pathPoints[index];
        endPos = pathPoints[++index];
        count = pathPoints.Length - 1;
        m_state = EnemyState.MOVE;

    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.gameObject.tag.Contains("GesturePrefab"))
        {
            if (coll.contacts[0].normal.y < -0.9f)
            {
                Rigidbody2D rigid = coll.rigidbody;
                if (rigid.mass > 1.5f)
                {
                    iTween.ScaleTo(this.gameObject, Vector3.zero, 0.75f);
                    SoundManager.SharedInstance.PlayClip("EnemyDestroy");
                    Destroy(this.gameObject, 0.8f);
                }
            }
            else
            {
                EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_OBJET_DESTROY, coll.gameObject);
            }
        }
    }

    void OnEventDestroy(System.Object data)
    {
        if (this.gameObject == (GameObject)data)
        {
            iTween.ScaleTo(this.gameObject, Vector3.zero, 0.75f);
            SoundManager.SharedInstance.PlayClip("EnemyDestroy");
            Destroy(this.gameObject, 0.8f);
        }
    }

    public void OnStop()
    {
        m_state = EnemyState.STOP;
    }

    void EditPathPoints(Vector3 ObstaclePos)
    {
        if (ObstaclePos.x < transform.position.x)
        {
            Vector3 pos = transform.position;
            pos.x += 1f;
            pathPoints[0] = pos;
        }
        else if (ObstaclePos.x < transform.position.x)
        {
            Vector3 pos = transform.position;
            pos.x -= 1f;
            pathPoints[1] = transform.position;
        }
        ReverseEnemySubState();
        OnMove();
    }
    // Update is called once per frame
    void Update()
    {
        if (m_state == EnemyState.MOVE)
        {
            Move(pathPoints);
        }
    }

    public void Move(Vector2[] points)
    {
        rate = 1.0f / (time*25);

        if (speed < 1)
        {
            //rate += SPEED;
            speed += rate;//Time.deltaTime * rate;

            Vector2 point = Vector2.Lerp(startPos, endPos, speed);
            transform.position = new Vector3(point.x, point.y, transform.position.z);

            if (transform.position == new Vector3(endPos.x, endPos.y, transform.position.z) && index < count)
            {
                startPos = endPos;
                endPos = points[++index];
                speed = 0;

            }

        }
        else
        {
            ReverseEnemySubState();
            OnMove();
        }

    }
    public Vector2[] ReverseMovingPlatformData(Vector2[] points)
    {

        count = points.Length;
        Vector2[] reversePoints = new Vector2[count];
        int i, j;
        for (i = count - 1, j = 0; i >= 0; i--, j++)
        {
            reversePoints[j] = points[i];
        }
        return reversePoints;

    }
    void ReverseEnemySubState()
    {
        if (m_subState == EnemySubState.FORWARD)
        {
            m_subState = EnemySubState.REVERSE;
            transform.localScale = new Vector3(-1 * this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
        }
        else if (m_subState == EnemySubState.REVERSE)
        {
            m_subState = EnemySubState.FORWARD;
            transform.localScale = new Vector3(-1 * this.transform.localScale.x, this.transform.localScale.y, this.transform.localScale.z);
        }

    }

    void OnDestroy()
    {
        EventHandler.RemoveListener(EEventID.EVENT_DESTROY_ENEMY, OnEventDestroy);
    }
}
