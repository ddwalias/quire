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

    public int IndexInParent
    {
        get
        {
            if (a__ != null)
            {
                return a__.Children.IndexOf(this);
            }
            return -1;
        }
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

    public bool IsValid => b(out _, IndexInParent);

    public string Label
    {
        get => c__;
        set
        {
            a(value);
            c__ = value;
        }
    }

    public string NodeInfoValueAsString => NodeInfoExtensions.ValueAsString(NodeInfo, ValueType);

    public int NodeInfoValueLength => NodeInfoExtensions.ValueLength(NodeInfo, ValueType);

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

    public Direction[] PrimalChildTypeoverExtendDirections => d__.PrimalChildTypeoverExtendDirections;

    public string DescriptorName => d__.DescriptorName;

    public Fixity Fixity => d__.Fixity;

    public Associativity Associativity => d__.Associativity;

    public bool IsInfix => d__.IsInfix;

    public bool IsUnary => d__.IsUnary;

    public int Precedence => d__.Precedence;

    public Type InfoType => d__.InfoType;

    public char? OperatorSymbol => d__.OperatorSymbol;

    public SymbolMargin OperatorSymbolMargin => d__.OperatorSymbolMargin;

    public string OperatorSExpName => d__.OperatorSExpName;

    public string OperatorVisualRepresentation => d__.OperatorVisualRepresentation;

    public string RLFormat => d__.RLFormat;

    public string XMLFormat => d__.XMLFormat;

    public string SplitOposite => d__.SplitOposite;

    public bool IsLineSplit => d__.LineSpan == LineSpan.SplitLine;

    public bool IsMonoLine => d__.LineSpan == LineSpan.MonoLine;

    public LineSpan LineSpan => d__.LineSpan;

    public string[] Alternates => d__.Alternates;

    public NodeValueType ValueType => d__.ValueType;

    public bool CarriesValue => d__.CarriesValue;

    public bool HasCaretPositions => d__.HasCaretPositions;

    public int ChildCount
    {
        get
        {
            if (IsNary || IsPairNary)
            {
                return Children.Count;
            }
            return d__.ChildCount;
        }
    }

    public int[] ChildrenToRemoveOnDelete => d__.ChildrenToRemoveOnDelete;

    public bool AllChildSlotsAreContainerSlots => d__.AllChildSlotsAreContainerSlots;

    public bool IgnoreOperandsOnOvertype => d__.IgnoreOperandsOnOvertype;

    public bool IsPlaceholder => d__.IsPlaceholder;

    public bool IsEvaluatable => d__.IsEvaluatable;

    public bool IsEvaluation
    {
        get
        {
            if (d__ is MathNodeDescriptorPiggyback)
            {
                return f;
            }
            return d__.IsEvaluation;
        }
        set => f = value;
    }

    public bool IsNary => d__.IsNary;

    public bool IsLinearNary => d__.IsLinearNary;

    public bool IsScopable
    {
        get
        {
            if (d__.IsScopable && IsLinearNary && !HasVisualRepresentation && ChildCount == 1)
            {
                return false;
            }
            return d__.IsScopable;
        }
    }

    public bool IsScopingBoundary => d__.IsScopingBoundary;

    public bool JumpThrough => d__.JumpThrough;

    public Regex ValidValueRegex => d__.ValidValueRegex;

    public Regex MatchRegex => d__.MatchRegex;

    public IValueCleaner ValueFixer => d__.ValueFixer;

    public bool AlsoRemoveParent => d__.AlsoRemoveParent;

    public bool HasVisualRepresentation => d__.HasVisualRepresentation;

    public bool AllowCaretOnLeft => d__.AllowCaretOnLeft;

    public bool AllowCaretOnRight => d__.AllowCaretOnRight;

    public bool RestrictFromNavigation => d__.RestrictFromNavigation;

    public bool RestrictsDescendants => d__.RestrictsDescendants;

    public bool ProhibitsDescendants => d__.ProhibitsDescendants;

    public bool RestrictsAncestors => d__.RestrictsAncestors;

    public bool DeleteParentInsteadOfNode => d__.DeleteParentInsteadOfNode;

    public bool Temporal => d__.Temporal;

    public bool MustScopeParent => d__.MustScopeParent;

    public bool ShowOnlyWhenFocused => d__.ShowOnlyWhenFocused;

    public bool IsSkipGenerateVisual => d__.IsSkipGenerateVisual;

    public int? ReplaceWithChildOnDelete => d__.ReplaceWithChildOnDelete;

    public IDictionary<int, string> DefaultNodeOverrides => d__.DefaultNodeOverrides;

    public bool IsImplicit => d__.IsImplicit;

    public string SExpFormat => d__.SExpFormat;

    public bool DistinctNodeEdges => d__.DistinctNodeEdges;

    public bool ExplicitValue => d__.ExplicitValue;

    public OperatorFamily? Family => d__.Family;

    public MathNodeGroups? Group => d__.Group;

    public bool IsLiteral => d__.IsLiteral;

    public bool IsIdentifier => d__.IsIdentifier;

    public bool NoLogicOnlyVisual => d__.NoLogicOnlyVisual;

    public bool PreFenceNavigatable => d__.PreFenceNavigatable;

    public bool PostFenceNavigatable => d__.PostFenceNavigatable;

    public int? PrimalChildPosition => d__.PrimalChildPosition;

    public bool AllowVerticalNavigation => d__.AllowVerticalNavigation;

    public bool IsLeafOperator => d__.IsLeafOperator;

    public bool JumpIntoChildren => d__.JumpIntoChildren;

    public bool HasContainerSlot => ContainerSlots.Length > 0;

    public bool IsPairNary => d__.IsPairNary;

    public string FirstElementDescriptorName => d__.FirstElementDescriptorName;

    public string MiddleElementDescriptorName => d__.MiddleElementDescriptorName;

    public string LastElementDescriptorName => d__.LastElementDescriptorName;

    public string AlternateMiddleElementDescriptorName => d__.AlternateMiddleElementDescriptorName;

    public string AlternateOperatorSExpName => d__.AlternateOperatorSExpName;

    public bool IsPairNarySubElement => d__.IsPairNarySubElement;

    public string ParentPairNaryDescriptorName => d__.ParentPairNaryDescriptorName;

    public bool IsFullContainer
    {
        get
        {
            if (AllChildSlotsAreContainerSlots)
            {
                return true;
            }
            return d__.IsFullContainer;
        }
    }

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
