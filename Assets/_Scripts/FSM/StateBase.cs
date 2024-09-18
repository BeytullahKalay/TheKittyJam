public abstract class StateBase
{
    protected NodeFSM FSM;

    public StateBase(NodeFSM fsm)
    {
        FSM = fsm;
    }

    public abstract void OnStateEnter();

    public abstract void OnStateExit();

    public abstract void Initialize();

    public abstract void HandleClick();

    public abstract void HandleKittyJump();

    public abstract void BeforeStartMove();

    public abstract void OnMoveStart();

    public abstract void OnMoveCompleted();

    public abstract void CheckForObstacles();

    public abstract void SpawnAnimalModel();

}
