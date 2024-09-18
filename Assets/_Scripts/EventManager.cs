using _Scripts.Node;
using System;

public static class EventManager
{
    public static Action OnLevelLoadPrepare;
    public static Action OnLevelLoad;
    public static Action LevelLoadExecute;

    public static Action OnGameLosePrepare;
    public static Action OnGameLose;
    public static Action GameLoseExecute;

    public static Action OnGameWinPrepare;
    public static Action OnGameWin;
    public static Action GameWinExecute;

    public static Action OnKittyStartTurnPrepare;
    public static Action OnKittyStartTurn;
    public static Action<NodeObject> KittyStartTurnExecute;

    public static Action OnKittyEndTurnPrepare;
    public static Action OnKittyEndTurn;
    public static Action KittyEndTurnExecute;

    public static Action OnFoxStartTurnPrepare;
    public static Action OnFoxStartTurn;
    public static Action FoxStartTurnExecute;

    public static Action OnFoxEndTurnPrepare;
    public static Action OnFoxEndTurn;
    public static Action FoxEndTurnExecute;

    static EventManager()
    {
        InitializeActionsQueue();
    }

    private static void InitializeActionsQueue()
    {
        // Level load actions
        LevelLoadExecute += () =>
        {
            OnLevelLoadPrepare?.Invoke();
            OnLevelLoad?.Invoke();
        };

        // Game lose actions
        GameLoseExecute += () =>
        {
            OnGameLosePrepare?.Invoke();
            OnGameLose?.Invoke();
        };

        // Game win actions
        GameWinExecute += () =>
        {
            OnGameWinPrepare?.Invoke();
            OnGameWin?.Invoke();
        };

        // Kitty start turn actions
        KittyStartTurnExecute += (NodeObject) =>
        {
            OnKittyStartTurnPrepare?.Invoke();
            OnKittyStartTurn?.Invoke();
        };

        // Kitty end turn actions
        KittyEndTurnExecute += () =>
        {
            OnKittyEndTurnPrepare?.Invoke();
            OnKittyEndTurn?.Invoke();
        };

        // Fox start turn actions
        FoxStartTurnExecute += () =>
        {
            OnFoxStartTurnPrepare?.Invoke();
            OnFoxStartTurn?.Invoke();
        };

        // Fox end turn actions
        FoxEndTurnExecute += () =>
        {
            OnFoxEndTurnPrepare?.Invoke();
            OnFoxEndTurn?.Invoke();
        };
    }
}
