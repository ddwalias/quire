using System.Runtime.CompilerServices;
using Quire.EquationEditor.Tree.Navigation;

namespace Quire.EquationEditor.Tree.Scoping;

public partial class NavigatorSegment
{
    private NavigatorState b;

    public NavigatorSegment(NavigatorState leftState, NavigatorState rightState)
    {
        LeftNavigatorState = leftState;
        RightNavigatorState = rightState;
    }

    public NavigatorState RightNavigatorState
    {
        get => b;
        set => b = value;
    }

    public int? LeftOperatorPart => LeftNavigatorState.OperatorPartIndex;

    public int? RightOperatorPart => RightNavigatorState.OperatorPartIndex;

    public bool SegmentIsSingleNode
    {
        [SkipLocalsInit]
        get
        {
            if (LeftNode.DescriptorName == "INTEGRAL" && RightNode.Parent == LeftNode && RightNode.IndexInParent == LeftNode.ChildCount - 1)
            {
                return true;
            }

            return LeftNode == RightNode;
        }
    }
}
