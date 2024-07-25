using System;

public static class EventManager
{
    public static Action OnLevelLoadPreapre;
    public static Action OnLevelLoad;
    public static Action LevelLoadExecute;

    public static Action OnGameLosePrepare;
    public static Action OnGameLose;
    public static Action GameLoseExecute;

    public static Action OnGameWinPrepare;
    public static Action OnGameWin;
    public static Action GameWinExecute;

    static EventManager()
    {
        InitializeActionsQueue();
    }

    private static void InitializeActionsQueue()
    {
        // Level load actions
        LevelLoadExecute += () => OnLevelLoadPreapre?.Invoke();
        LevelLoadExecute += () => OnLevelLoad?.Invoke();


        // Game lose actions
        GameLoseExecute += () => OnGameLosePrepare?.Invoke();
        GameLoseExecute += () => OnGameLose?.Invoke();

        // Game win actions
        GameWinExecute += () => OnGameWinPrepare?.Invoke();
        GameWinExecute += () => OnGameWin?.Invoke();
    }
}
