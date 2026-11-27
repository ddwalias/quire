using System;

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Quire.Core;
using Quire.EquationEditor.Tree.Parsing;
using Quire.EquationEditor.Tree.Transform;
using Quire.Mirror;

namespace Quire.EquationEditor.Tree;

public partial class MathNode
{
    private AlgorithmOptions g;

    public MathNode this[int index]
    {
        get => Children[index];
        set => Children[index] = value;
    }

    public MathNode AdjacentSubexpressionRoot
    {
        get
        {
            MathNode mathNode = this;
            if (mathNode.Parent.Associativity != Associativity.Right)
            {
                while (mathNode.RightAdjacentSubexpression(false) != null)
                {
                    mathNode = mathNode.RightAdjacentSubexpression(false);
                }
                if (mathNode.LeftAdjacentSubexpression(false) != null)
                {
                    return mathNode.Parent;
                }
                return null;
            }
            while (mathNode.LeftAdjacentSubexpression(false) != null)
            {
                mathNode = mathNode.LeftAdjacentSubexpression(false);
            }
            if (mathNode.RightAdjacentSubexpression(false) != null)
            {
                return mathNode.Parent;
            }
            return null;
        }
    }

    public IMathNodeList Children => b__;

    public virtual MathNode ContainerRoot
    {
        get
        {
            if (IsEquationRoot || Parent.IsContainerSlot(IndexInParent))
            {
                return this;
            }
            return Parent.ContainerRoot;
        }
    }

    public MathNode EquationRootNode
    {
        get
        {
            if (IsEquationRoot || IsRoot)
            {
                return this;
            }
            return Parent.EquationRootNode;
        }
    }

    public string Algorithm
    {
        get => e__;
        set => e__ = value;
    }

    public string AlgorithmSExpression => Algorithm + AlgorithmOptions.ToSExpression();

    public AlgorithmOptions AlgorithmOptions
    {
        get => g;
        set => g = value;
    }

    public bool IsEquationRoot
    {
        get
        {
            if (!(Parent is RootMathNode))
            {
                return Parent == null;
            }
            return true;
        }
    }

    public bool IsInContainerSlot
    {
        get
        {
            if (Parent != null)
            {
                return Parent.IsContainerSlot(IndexInParent);
            }
            return true;
        }
    }

    public bool IsReadonly
    {
        get
        {
            if (IsRoot)
            {
                return false;
            }
            if (!Parent.IsReadonlySlot(IndexInParent))
            {
                return Parent.IsReadonly;
            }
            return true;
        }
    }

    public bool IsAssociativeChildAContainerSlot
    {
        get
        {
            if (ChildCount == 0)
            {
                throw new InvalidOperationException("Logic Error: Leaf nodes do not have children");
            }
            switch (Descriptor.Associativity)
            {
                case Associativity.None:
                case Associativity.Left:
                    return Descriptor.IsContainerSlot(0);
                case Associativity.Right:
                    return Descriptor.IsContainerSlot(1);
                default:
                    throw new InvalidOperationException("Logic Error: Unknown Associativity");
            }
        }
    }

    public bool IsRoot => a__ == null;

    public bool IsRooted => RootNode is RootMathNode;

    public virtual bool IsScopeRoot => this == ScopeRoot;

    public MathNode RootNode
    {
        get
        {
            if (IsRoot)
            {
                return this;
            }
            return Parent.RootNode;
        }
    }

    public virtual MathNode ScopeRoot
    {
        get
        {
            if (Parent == null || Parent.IsScopingBoundary || IsEquationRoot)
            {
                return this;
            }
            return Parent.ScopeRoot;
        }
    }

    public bool HasContainerSlot => ContainerSlots.Length > 0;

    public bool HasLastElement
    {
        get
        {
            if (IsPairNary)
            {
                return (ChildCount - FirstElementDescriptor.ChildCount) % ActualMiddleElementDescriptor.ChildCount != 0;
            }
            return false;
        }
    }

    public int ElementsCount
    {
        get
        {
            if (IsPairNary)
            {
                int num = 1;
                num += (ChildCount - FirstElementDescriptor.ChildCount) / ActualMiddleElementDescriptor.ChildCount;
                if (HasLastElement)
                {
                    num++;
                }
                return num;
            }
            return -1;
        }
    }

    public MathNode AdjacentSubexpression(Direction dir)
    {
        if (dir == Direction.Left)
        {
            return LeftAdjacentSubexpression(false);
        }
        if (dir == Direction.Right)
        {
            return RightAdjacentSubexpression(false);
        }
        return null;
    }

    public void CopyAlgorithm(MathNode original)
    {
        int num = Math.Min(ChildCount, original.ChildCount);
        for (int i = 0; i < num; i++)
        {
            Children[i].CopyAlgorithm(original.Children[i]);
        }
        if (original.Algorithm != string.Empty)
        {
            Algorithm = original.Algorithm;
        }
        if (original.AlgorithmOptions.HasAnyOption)
        {
            AlgorithmOptions = original.AlgorithmOptions.Clone();
        }
    }

    public MathNode DescendentFromPath(params int[] path)
    {
        IList<int> list = new List<int>(path);
        return DescendentFromPath(list);
    }

    public MathNode DescendentFromPath(IList<int> path)
    {
        MathNode mathNode = this;
        for (int i = 0; i < path.Count; i++)
        {
            if (mathNode.ChildCount <= path[i])
            {
                return null;
            }
            mathNode = mathNode.Children[path[i]];
        }
        return mathNode;
    }

    public bool HasAdjacentSub(MathNode test, Direction direction)
    {
        if (test == this)
        {
            throw new ArgumentException("Cannot be same as this math node", "test");
        }
        if (direction == Direction.Indeterminate)
        {
            throw new ArgumentException("Cannot be indeterminate", "direction");
        }
        MathNode mathNode = this;
        while (mathNode != null)
        {
            mathNode = mathNode.AdjacentSubexpression(direction);
            if (mathNode == test)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsDescendentOf(MathNode node)
    {
        if (Parent == node)
        {
            return true;
        }
        if (Parent == null)
        {
            return false;
        }
        return Parent.IsDescendentOf(node);
    }

    public MathNode GetNearestAncestor(Func<MathNode, bool> matchCriteria)
    {
        for (MathNode parent = Parent; parent != null; parent = parent.Parent)
        {
            if (matchCriteria(parent))
            {
                return parent;
            }
        }
        return null;
    }

    public MathNode GetNearestEvaluationAncestor()
    {
        for (MathNode parent = Parent; parent != null; parent = parent.Parent)
        {
            if (parent.IsEvaluation)
            {
                return parent;
            }
        }
        return null;
    }

    public MathNode GetNearestAncestor(string descriptorName)
    {
        for (MathNode parent = Parent; parent != null; parent = parent.Parent)
        {
            if (parent.DescriptorName == descriptorName)
            {
                return parent;
            }
        }
        return null;
    }

    public MathNode GetNearestAncestor(OperatorFamily family)
    {
        for (MathNode parent = Parent; parent != null; parent = parent.Parent)
        {
            if (parent.Family == family)
            {
                return parent;
            }
        }
        return null;
    }

    public MathNode GetNearestVerticalNavigatableAncestor()
    {
        for (MathNode parent = Parent; parent != null; parent = parent.Parent)
        {
            if (parent.AllowVerticalNavigation)
            {
                return parent;
            }
        }
        return null;
    }

    public MathNode GetNearestNaryAncestor()
    {
        for (MathNode parent = Parent; parent != null; parent = parent.Parent)
        {
            if (parent.IsNary)
            {
                return parent;
            }
        }
        return null;
    }

    public MathNode GetNearestLinearNaryAncestor()
    {
        for (MathNode parent = Parent; parent != null; parent = parent.Parent)
        {
            if (parent.IsLinearNary)
            {
                return parent;
            }
        }
        return null;
    }

    public bool AreAllChildrenEmpty()
    {
        for (int i = 0; i < ChildCount; i++)
        {
            MathNode mathNode = this[i];
            if (!mathNode.IsPlaceholder)
            {
                if (mathNode.IsInherentChildOfParent)
                {
                    if (mathNode.AreAllChildrenEmpty())
                    {
                        continue;
                    }
                    return false;
                }
                return false;
            }
        }
        return true;
    }

    public MathNode NodeFromPath(IList<int> path)
    {
        return DescendentFromPath(path);
    }

    public IList<int> PathFromAncestor(MathNode root)
    {
        List<int> list = new List<int>();
        for (MathNode mathNode = this; mathNode != root; mathNode = mathNode.Parent)
        {
            if (mathNode.IsRoot)
            {
                throw new ArgumentException("Argument node is not an ancestor", "root");
            }
            list.Add(mathNode.IndexInParent);
        }
        list.Reverse();
        return list;
    }

    public IList<int> PathFromEquationRoot()
    {
        return PathFromAncestor(EquationRootNode);
    }

    public bool IsInSameFamily(MathNode longLostCousin)
    {
        if (longLostCousin == null)
        {
            return false;
        }
        if (longLostCousin.DescriptorName == DescriptorName)
        {
            return true;
        }
        if (Family.HasValue)
        {
            return Family == longLostCousin.Family;
        }
        return false;
    }

    public bool ChildNotPrimal(int childIndex)
    {
        if (PrimalChildPosition.HasValue)
        {
            return PrimalChildPosition != childIndex;
        }
        return false;
    }

    [System.Runtime.CompilerServices.SkipLocalsInit]
    public MathNode GetContainerRoot(bool nonPrimalChildAsContainer)
    {
        if (!nonPrimalChildAsContainer)
        {
            return ContainerRoot;
        }
        if (IsEquationRoot || Parent.IsContainerSlot(IndexInParent) || Parent.ChildNotPrimal(IndexInParent))
        {
            return this;
        }
        return Parent.GetContainerRoot(true);
    }

    public int ElementIndexOfChild(int childIndex)
    {
        if (childIndex < FirstElementDescriptor.ChildCount)
        {
            return 0;
        }
        return (childIndex - FirstElementDescriptor.ChildCount) / ActualMiddleElementDescriptor.ChildCount + 1;
    }

    public void MakeEditable()
    {
        foreach (MathNode child in Children)
        {
            child.MakeEditable();
        }
        if (NodeInfo != null)
        {
            NodeInfo.MakeEditable();
        }
    }

    public void MakeReadOnly()
    {
        foreach (MathNode child in Children)
        {
            child.MakeReadOnly();
        }
        if (NodeInfo != null)
        {
            NodeInfo.MakeReadOnly();
        }
    }

    [MirrorVerified("Release IL")]
    internal MathNode(MathNodeDescriptor mathNodeDescriptor, params MathNode[] mathNodes) : this(mathNodeDescriptor, null, mathNodes) { }

    [MirrorVerified("Release IL")]
    public int GetMatrixCellIndex(MathNode matrixAncestor) => GetBranchIndexInAncestor(matrixAncestor);

    [MirrorVerified("Release IL")]
    public MathNode LeftNeighbor => null;

    [MirrorVerified("Release IL")]
    public virtual string SExpression => MathNodeToSExpressionTransform.Default.Transform(this);

    [MirrorVerified("Release IL")]
    public bool ContainsOrIsComboBox => ContainsOrIs("ComboBoxControl");

    partial class b_
    {
        [MirrorVerified("Release IL")]
        public b_() { }
    }

    partial class c_
    {
        [MirrorVerified("Release IL")]
        public c_() { }
    }

    partial class d_
    {
        [MirrorVerified("Release IL")]
        public d_() { }
    }

    partial class e_
    {
        [MirrorVerified("Release IL")]
        public e_() { }
    }

    partial class f_
    {
        [MirrorVerified("Release IL")]
        public f_() { }
    }

    partial class a_
    {
        partial class a__
        {
            [MirrorVerified("Release IL")]
            public a__() { }
        }
    }
}
