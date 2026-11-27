using System.Runtime.CompilerServices;

using System.Runtime.CompilerServices;
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.Tree.Navigation;
using Quire.Mirror;

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

    [MirrorVerified("Release IL")]
    public NavigatorState LeftNavigatorState { get => a_; set => a_ = value; }
}
