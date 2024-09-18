using UnityEditor;

public class LockMenu : UnityEditor.Editor
{
    [MenuItem("Tools/Toggle Inspector Lock #q")] // Shift + Q
    public static void ToggleInspectorLock()
    {
        ActiveEditorTracker.sharedTracker.isLocked = !ActiveEditorTracker.sharedTracker.isLocked;
        ActiveEditorTracker.sharedTracker.ForceRebuild();
    }
}