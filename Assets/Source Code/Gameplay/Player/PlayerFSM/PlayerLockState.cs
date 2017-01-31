using UnityEngine;
using System.Collections;

public class PlayerLockState : PlayerGestureDrawState
{

    public PlayerLockState(PlayerFSM player):base(player)
    {
        StateName = PLAYERSTATE.LOCK;
        Player = player;
    }

    public void OnStateInit()
    {
        //base.OnStateInit();
    }

    public void OnStateEnter()
    {
        base.OnStateEnter();
        Player.m_ptrRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        base.RemoveTouchEnd();
        Player.PlayIdle();
    }

    public void OnStateUpdate()
    {

    }

    public void OnStateExit()
    {

    }

}
