using DG.Tweening;

public class PathData
{
    public Tween PathTween;
    public NodeFSM DestinationNodeFSM;
    public AnimationType AnimationType;

    public PathData(Tween pathTween, NodeFSM finalNodeFSM, AnimationType animationType)
    {
        PathTween = pathTween;
        DestinationNodeFSM = finalNodeFSM;
        AnimationType = animationType;
    }
}
