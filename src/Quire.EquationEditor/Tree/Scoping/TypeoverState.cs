#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.Tree.Parsing;
using Quire.EquationEditor.Tree.Transform;
using Quire.Matrices;
using Quire.Mirror;
using Quire.Native.PtcEquationEditor;

namespace Quire.EquationEditor.Tree.Scoping;

public partial class TypeoverState
{
    public readonly NavigatorState NavState;

    [MirrorName("c")]
    private bool c__;

    public bool IncludePrimalChildren
    {
        get => c__;
        set => c__ = value;
    }

    public string SExpression => MathNodeToSExpressionTransform.Default.Transform(LeftNode.EquationRootNode, c());

    public IEnumerable<int> MatrixCellsInTypeover
    {
        get
        {
            if (!AreMatrixCellsInTypeover())
            {
                return new int[0];
            }
            MathNode ancestor = MathNode.Utilities.FindCommonAncestor(LeftNode, RightNode);
            if (ancestor == LeftNode || ancestor == RightNode)
            {
                return new int[0];
            }
            int corner1 = LeftNode.PathFromAncestor(ancestor)[0];
            int corner2 = RightNode.PathFromAncestor(ancestor)[0];
            return MatrixHelper.SubMatrixIndices(corner1, corner2, ((MatrixNodeInfo)ancestor.NodeInfo).ActualSize.Columns);
        }
    }

    [MirrorVerified("Release IL")]
    public TypeoverState(MathNode left, MathNode right, NavigatorState navState)
        : this(new LinearSegment(left, right), navState)
    {
    }

    [MirrorVerified("Release IL")]
    public TypeoverState(LinearSegment segment, NavigatorState navState, bool includePrimalChildren)
        : this(NavigatorSegment.NavigatorSegmentFromLinearSegment(segment), navState, includePrimalChildren)
    {
    }

    [MirrorVerified("Release IL")]
    public TypeoverState(NavigatorSegment segment, NavigatorState navState)
        : this(segment.LeftNavigatorState, segment.RightNavigatorState, navState, false)
    {
    }

    [MirrorVerified("Release IL")]
    public TypeoverState(NavigatorSegment segment, NavigatorState navState, bool includePrimalChildren)
        : this(segment.LeftNavigatorState, segment.RightNavigatorState, navState, includePrimalChildren)
    {
    }

    [MirrorVerified("Release IL")]
    public TypeoverState(NavigatorState left, NavigatorState right, NavigatorState navState, bool includePrimalChildren)
        : base(left, right)
    {
        NavState = navState;
        IncludePrimalChildren = includePrimalChildren;
    }

    [MirrorVerified("Release IL")]
    public TypeoverState(TypeoverState objectToCopy)
        : this(objectToCopy.LeftNavigatorState, objectToCopy.RightNavigatorState, objectToCopy.NavState, objectToCopy.IncludePrimalChildren)
    {
    }

    [MirrorVerified("Release IL")]
    public TypeoverState(NavigatorState left, NavigatorState right, NavigatorState navState) : this(left, right, navState, false) { }

    [MirrorVerified("Release IL")]
    public TypeoverState(LinearSegment segment, NavigatorState navState) : this(segment, navState, false) { }

    [MirrorVerified("Release IL")]
    public TypeoverState Clone() => new TypeoverState(this);

    [MirrorVerified("Release IL")]
    public IEnumerable<MathNode> ExcludedNodes => b();

    [MirrorVerified("Release IL")]
    public IEnumerable<MathNode> ExcludedOperands => a();

    private IEnumerable<MathNode> b() => new c_(-2) { d_ = this };

    private IEnumerable<MathNode> a() => new d_(-2) { d__ = this };

    public TypeoverState CloneForNewTree(MathNode newRoot)
    {
        s left = new s(LeftNavigatorState.Current);
        s right = new s(RightNavigatorState.Current);
        s nav = new s(NavState.Current);
        return new TypeoverState(LeftNavigatorState.ChangeCurrent(left.b(newRoot)), RightNavigatorState.ChangeCurrent(right.b(newRoot)), NavState.ChangeCurrent(nav.b(newRoot)), IncludePrimalChildren);
    }

    private IList<MathNode> d()
    {
        if (!AreMatrixCellsInTypeover())
        {
            return new MathNode[0];
        }
        List<MathNode> nodes = new List<MathNode>();
        MathNode ancestor = MathNode.Utilities.FindCommonAncestor(LeftNode, RightNode);
        if (ancestor == LeftNode || ancestor == RightNode)
        {
            nodes.Add(ancestor);
            foreach (MathNode child in ancestor.Children)
            {
                nodes.AddRange(child.WalkSubTree());
            }
        }
        else
        {
            foreach (int cell in MatrixCellsInTypeover)
            {
                nodes.AddRange(ancestor[cell].WalkSubTree());
            }
        }
        return nodes;
    }

    public bool IsMatrixInTypeover()
    {
        IList<MathNode> nodes = d();
        if (nodes.Count > 0 && nodes[0].DescriptorName == "MATRIX")
        {
            return nodes[nodes.Count - 1].IsDescendentOf(nodes[0]);
        }
        return false;
    }

    // The original closure classes are the obfuscated nested types a and b, so they are written out by hand.
    private DelegateFunction1<string, MathNode> c()
    {
        a_ closure = new a_();
        closure.a__ = this;
        if (AreMatrixCellsInTypeover())
        {
            b_ matrixClosure = new b_();
            matrixClosure.a_ = d();
            return matrixClosure.a;
        }
        closure.b = b();
        return closure.a;
    }

    partial class a_
    {
        [MirrorVerified("Release IL")]
        public a_() { }
    }

    partial class b_
    {
        [MirrorVerified("Release IL")]
        public b_() { }

        [MirrorName("a")]
        public IList<MathNode> a_;

        internal string a(MathNode A_0)
        {
            if (a_.IndexOf(A_0) >= 0)
            {
                return "ot";
            }
            return null;
        }
    }
}
