using UnityEngine;
using System.Collections;
using System;

public class CharacterFSM : MonoBehaviour {
    public float MaxVelocity;
    public float JumpMagnitude;
    public float Acceleration;

    PLAYER_STATES m_eCurrentState;

    Vector2 m_vInitialTouchposition;
    int MoveDir;
    Transform GroundCheck;

    Rigidbody2D m_ptrRigidbody;
    Collider2D[] GroundColliders;
    Camera m_MoveCamera;

    bool b_flipscale;
	void Start () {
        m_eCurrentState = PLAYER_STATES.IDLE;
        GroundCheck = transform.GetChild(0);
        GroundColliders = new Collider2D[5];
        m_MoveCamera = GameObject.FindGameObjectWithTag("InputCamera").GetComponent<Camera>() ;


        m_ptrRigidbody = GetComponent<Rigidbody2D>();
        b_flipscale = true;
	}

    void OnEventTouchMoveStart(System.Object data)
    {
        Hashtable table = (Hashtable)data;
        Vector2 TouchPos = (Vector2)table["StartPos"];
        Vector2 WorldPos = m_MoveCamera.ScreenToWorldPoint(TouchPos);
        m_vInitialTouchposition = WorldPos;
    }

    void OnEventTouchMove(System.Object data)
    {
        Hashtable table = (Hashtable)data;
        float TargetVelocity = MaxVelocity;
        if (m_eCurrentState == PLAYER_STATES.IDLE)
        {
            m_eCurrentState = PLAYER_STATES.MOVE;
            TargetVelocity = MaxVelocity;
        }
        else if (m_eCurrentState == PLAYER_STATES.JUMP)
        {
            TargetVelocity = MaxVelocity / 1.5f;
        }

        Vector2 TouchPos = (Vector2)table["TouchPos"];
        Vector2 WorldPos = m_MoveCamera.ScreenToWorldPoint(TouchPos);
        if (m_vInitialTouchposition.x <= WorldPos.x)
        {
            if (b_flipscale)
            {
                b_flipscale = false;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                MoveDir = 1;
            }
        }
        else
        {
            if (!b_flipscale)
            {
                b_flipscale = true;
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                MoveDir = -1;
            }
        }

        float velocity = Mathf.MoveTowards(m_ptrRigidbody.velocity.x, TargetVelocity * MoveDir, Acceleration * Time.fixedDeltaTime);
       
        m_ptrRigidbody.velocity = new Vector2(velocity, m_ptrRigidbody.velocity.y);
    }

    void OnEventTouchMoveEnd(System.Object data)
    {
        if (m_eCurrentState != PLAYER_STATES.JUMP)
        {
            m_eCurrentState = PLAYER_STATES.IDLE;
            float NewVelocity = Mathf.MoveTowards(m_ptrRigidbody.velocity.x, 0f, Acceleration * 5f);
            //float NewVelocity = m_ptrRigidbody.velocity.x - (Acceleration * 3f);
            m_ptrRigidbody.velocity = new Vector2(NewVelocity, m_ptrRigidbody.velocity.y);
        }
    }

    void OnEventTouchTap(System.Object data)
    {
        Hashtable table = (Hashtable)data;
        Vector2 TouchPos = (Vector2)table["EndPos"];
        Vector2 WorldPos = Camera.main.ScreenToWorldPoint(TouchPos);
        RaycastHit2D[] hits = Physics2D.RaycastAll(WorldPos, Vector2.zero);
        bool b_Jump = true;

        foreach(var hit in hits)
        {
            if(hit.collider != null)
            {
                if(hit.collider.tag == "GesturePrefab")
                {
                    iTween.ScaleTo(hit.collider.gameObject, Vector3.zero, 0.75f);
                    Destroy(hit.collider.gameObject, 0.8f);
                    EventHandler.TriggerEvent(EEventID.EVENT_GESTURE_OBJET_DESTROY, null);
                    b_Jump = false;
                    break;
                }
            }
        }

        if ((m_eCurrentState != PLAYER_STATES.JUMP) && b_Jump)
        {
            m_ptrRigidbody.AddForce(new Vector2(0f, JumpMagnitude));
            StartCoroutine(CheckGround());
            m_eCurrentState = PLAYER_STATES.JUMP;
        }
    }


    void OnIdleStateUpdate()
    {
        if (Mathf.Abs(m_ptrRigidbody.velocity.x) > 0)
        {
            float NewVelocity = Mathf.MoveTowards(m_ptrRigidbody.velocity.x, 0f, Acceleration * 10f * Time.deltaTime);
            m_ptrRigidbody.velocity = new Vector2(NewVelocity, m_ptrRigidbody.velocity.y);
        }
    }

    void OnJumpStateUpdate()
    {

    }

    void OnMoveStateUpdate()
    {

    }

    void FixedUpdate()
    {
        Debug.Log(m_eCurrentState);
        switch(m_eCurrentState)
        {
            case PLAYER_STATES.IDLE:
                OnIdleStateUpdate();
                break;

            case PLAYER_STATES.JUMP:
                OnJumpStateUpdate();
                break;

            case PLAYER_STATES.MOVE:
                OnMoveStateUpdate();
                break;
        }
    }

    IEnumerator CheckGround()
    {
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();
        System.Array.Clear(GroundColliders, 0, GroundColliders.Length);
        while (true)
        {
            Physics2D.OverlapCircleNonAlloc(GroundCheck.position, 0.2f, GroundColliders);
            foreach (Collider2D collider in GroundColliders)
            {
                if (collider != null)
                {
                    if (collider.tag == "Ground")
                    {
                        m_eCurrentState = PLAYER_STATES.IDLE;
                        yield break;
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    void FlipScale()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, -1f * transform.localScale.z);
    }

    public float GetPlayerVelocity()
    {
        return m_ptrRigidbody.velocity.x;
    }

    public PLAYER_STATES GetCurrentState()
    {
        return m_eCurrentState;
    }

    void OnDestroy()
    {

    }
}

public enum PLAYER_STATES
{
    NONE = -1,
    IDLE,
    MOVE,
    JUMP,
}
