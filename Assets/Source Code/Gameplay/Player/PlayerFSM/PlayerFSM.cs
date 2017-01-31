//#define debug

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PlayerFSM : MonoBehaviour
{
    #region Editable variables
    public float MaxVelocity;
    public float Acceleration;
    public float JumpMagnitude;
    #endregion


    #region Variable
    public Camera m_MoveCamera { get; set; }
    public Vector2 m_vInitialTouchposition { get; set; }
    public bool b_flipscale { get; set; }
    public bool b_Grounded { get; set; }
    public int MoveDir { get; set; }
    public Vector3 playerScale
    {
        get
        {
            return transform.localScale;
        }
        set
        {
            transform.localScale = value;
        }
    }
    public int m_iPrimaryTouch;
    Collider2D[] GroundCheckLeft;
    Collider2D[] GroundCheckRight;
    Vector2 GroundCheckPosLeft;
    Vector2 GroundCheckPosRight;
    #endregion

    #region Componenets
    public Rigidbody2D m_ptrRigidbody;
    public Animator m_ptrAnim;
    public SpriteRenderer m_ptrSpriteRenderer;
    #endregion


    public IPlayerState idleState;
    public IPlayerState movementState;
    public IPlayerState gestureState;
    public IPlayerState deadState;
    public IPlayerState jumpState;
    public IPlayerState lockState;

    private IPlayerState m_PlayerCurrentState = null;

    private IPlayerState m_PlayerPreviousState = null;

    public void GameInit()
    {
        m_iPrimaryTouch = -1;
        idleState = new PlayerIdleState(this);
        movementState = new PlayerMovementState(this);
        gestureState = new PlayerGestureDrawState(this);
        deadState = new PlayerDeadState(this);
        jumpState = new PlayerJumpState(this);
        lockState = new PlayerLockState(this);
    }

    public void GameStart()
    {
        SetCompoenentReferences();
        if (idleState != null) idleState.OnStateInit();
        if (movementState != null) movementState.OnStateInit();
        if (gestureState != null) gestureState.OnStateInit();
        if (deadState != null) deadState.OnStateInit();

        ChangeState(idleState);
    }

    private void MachineUpdate()
    {
        if (m_PlayerCurrentState != null)
            m_PlayerCurrentState.OnStateUpdate();
    }

    public void ChangeState(IPlayerState currentState, IPlayerState targetState)
    {
        m_PlayerPreviousState = currentState;
        currentState.OnStateExit();

        targetState.OnStateEnter();
        m_PlayerCurrentState = targetState;
    }

    public void ChangeState(IPlayerState targetState)
    {
        if (m_PlayerCurrentState != null)
        {
            m_PlayerPreviousState = m_PlayerCurrentState;
            m_PlayerCurrentState.OnStateExit();
        }

        targetState.OnStateEnter();
        m_PlayerCurrentState = targetState;
    }

    #region MonoBehaviour functions

    void Awake()
    {
        GameInit();
    }

    void Start()
    {
        MoveDir = 1;
        playerScale = transform.localScale;
        b_flipscale = false;
        GroundCheckLeft = new Collider2D[4];
        GroundCheckRight = new Collider2D[4];
        SetCompoenentReferences();
        GameStart();

        EventHandler.AddListener(EEventID.EVENT_PLAYER_STATE_UPDATE, OnPlayerStateUpdate);
    }

    public void SetCompoenentReferences()
    {
        m_ptrRigidbody = GetComponent<Rigidbody2D>();
        m_ptrAnim = GetComponent<Animator>();
        m_ptrSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void FixedUpdate()
    {
        PlayerGroundedCheck();
        MachineUpdate();
    }

    void Update()
    {
        Debug.Log(m_PlayerCurrentState.StateName);
    }

    #endregion

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Hazard")
        {
            ChangeState(deadState);
        }
        EventHandler.TriggerEvent(EEventID.EVENT_COLLISION_ENTER, other);
    }

    void OnCollisionStay2D(Collision2D other)
    {
        EventHandler.TriggerEvent(EEventID.EVENT_COLLISION_STAY, other);
    }

    void OnCollisionExit2D(Collision2D other)
    {
        EventHandler.TriggerEvent(EEventID.EVENT_COLLISION_EXIT, other);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Hazard")
        {
            ChangeState(deadState);
        }
    }

    void PlayerGroundedCheck()
    {
        GroundCheckPosLeft = transform.GetChild(0).position;
        GroundCheckPosRight = transform.GetChild(1).position;
        System.Array.Clear(GroundCheckLeft, 0, GroundCheckLeft.Length);
        System.Array.Clear(GroundCheckRight, 0, GroundCheckRight.Length);
        int leftCount = Physics2D.OverlapCircleNonAlloc(GroundCheckPosLeft, 0.2f, GroundCheckLeft);
        int rightCount = Physics2D.OverlapCircleNonAlloc(GroundCheckPosRight, 0.2f, GroundCheckRight);
        bool b_LeftGrounded = false;
        bool b_RightGrounded = false;
        if (leftCount > 0)
        {
            foreach (var checkLeft in GroundCheckLeft)
            {
                if (checkLeft != null)
                {
                    if (checkLeft.tag.Contains("Ground") || checkLeft.tag.Contains("GesturePrefab"))
                    {
                        b_LeftGrounded = true;
                        break;
                    }
                }
            }
        }

        if(rightCount > 0)
        {
            foreach (var checkRight in GroundCheckRight)
            {
                if (checkRight != null)
                {
                    if (checkRight.tag.Contains("Ground") || checkRight.tag.Contains("GesturePrefab"))
                    {
                        b_RightGrounded = true;
                        break;
                    }
                }
            }
        }

        if (b_LeftGrounded && b_RightGrounded)
            b_Grounded = true;
        else
            b_Grounded = false;

    }

    public float GetVectorAngle(Vector2 EndPos,Vector2 StartPos)
    {
        Vector3 diff = EndPos - StartPos;
        diff.Normalize();

        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        return rot_z;
    }

    void OnPlayerStateUpdate(System.Object data)
    {
        PLAYERSTATE state = (PLAYERSTATE)data;
        if (state == PLAYERSTATE.LOCK)
            ChangeState(lockState);
        else if (state == PLAYERSTATE.IDLE)
            ChangeState(idleState);
        else if (state == PLAYERSTATE.JUMP)
            ChangeState(jumpState);
    }

    public IEnumerator BlinkPlayer()
    {
        for(int i=0;i<5;++i)
        {
            m_ptrSpriteRenderer.enabled = false;
            for (int j = 0; j < 4; ++j)
            {
                yield return new WaitForFixedUpdate();
            }
            m_ptrSpriteRenderer.enabled = true;
            for (int j = 0; j < 4; ++j)
            {
                yield return new WaitForFixedUpdate();
            }
        }
        m_ptrSpriteRenderer.enabled = true;
    }

    public void PlayIdle()
    {
        if (m_ptrAnim == null)
            m_ptrAnim = GetComponent<Animator>();
        m_ptrAnim.Play("Idle");
    }

    public void PlayWalk()
    {
        if (m_ptrAnim == null)
            m_ptrAnim = GetComponent<Animator>();
        m_ptrAnim.Play("Walk");
    }

    public void PlayJump()
    {
        if (m_ptrAnim == null)
            m_ptrAnim = GetComponent<Animator>();
        m_ptrAnim.Play("Jump");
    }

    public IEnumerator ResetText()
    {
        yield return new WaitForSeconds(2.5f);
        GameUIController.Instance.m_PromptText.text = "";
    }

    void OnDestroy()
    {
        EventHandler.RemoveListener(EEventID.EVENT_PLAYER_STATE_UPDATE, OnPlayerStateUpdate);
        idleState = null;
        movementState = null;
        gestureState = null;
        deadState = null;
        jumpState = null;
        lockState = null;
    }

    void ResetComponent(ref Behaviour componenet)
    {
        if(componenet.GetType() == typeof(Animator))
        {
            componenet = GetComponent<Animator>();    
        }
    }
}
