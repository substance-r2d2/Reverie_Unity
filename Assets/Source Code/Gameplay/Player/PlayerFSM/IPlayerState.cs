public interface IPlayerState
{
    PLAYERSTATE StateName { get; set; }

    PlayerFSM Player { get; set; }

    void OnStateInit();
    void OnStateEnter();
    void OnStateUpdate();
    void OnStateExit();
}
