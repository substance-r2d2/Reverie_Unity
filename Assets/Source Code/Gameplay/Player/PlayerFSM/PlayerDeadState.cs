using UnityEngine;
using System.Collections;

public class PlayerDeadState : IPlayerState {
    public PLAYERSTATE StateName { get; set; }

    public PlayerFSM Player { get; set; }

    float m_fStartTime;
    float PlayerMoveTime;

    public PlayerDeadState(PlayerFSM player)
    {
        this.Player = player;
        StateName = PLAYERSTATE.DEAD;
        PlayerMoveTime = 1.5f;
    }

    public void OnStateInit()
    {

    }

    public void OnStateEnter()
    {
        GameManager.Instance.m_ptrUIController.OnPlayerStateChange(PLAYERSTATE.DEAD, false);
        EventHandler.TriggerEvent(EEventID.EVENT_PLAYER_STATE_CHANGE, PLAYERSTATE.DEAD);
        iTween.MoveTo(Player.gameObject, GameManager.Instance.GetSavePoint(), PlayerMoveTime);
        m_fStartTime = Time.time;
        Player.PlayIdle();
        Player.StartCoroutine(Player.BlinkPlayer());
    }

    public void OnStateUpdate()
    {
        if (Time.time - m_fStartTime > PlayerMoveTime)
            Player.ChangeState(Player.idleState);
    }

    public void OnStateExit()
    {

    }
}
