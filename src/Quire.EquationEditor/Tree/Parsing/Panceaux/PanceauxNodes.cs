namespace Quire.EquationEditor.Tree.Parsing.Panceaux
{
    public partial class InsertChildrenIntoParent
    {
        public override void Accept(global::Quire.EquationEditor.Tree.Parsing.PanaceaVisitor visitor)
        {
            visitor.Visit(this);
        }

        public InsertChildrenIntoParent()
        {
        }
    }

    public partial class ReplaceWithChild
    {
        public override void Accept(global::Quire.EquationEditor.Tree.Parsing.PanaceaVisitor visitor)
        {
            visitor.Visit(this);
        }

        public ReplaceWithChild()
        {
        }
    }

    public partial class ReplaceWith
    {
        public ReplaceWith(string mathNodeName)
        {
            MathNodeName = mathNodeName;
        }

        public override void Accept(global::Quire.EquationEditor.Tree.Parsing.PanaceaVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public partial class ReplaceWithDefaultNode
    {
        public ReplaceWithDefaultNode(int childIndex)
        {
            ChildIndex = childIndex;
        }

        public override void Accept(global::Quire.EquationEditor.Tree.Parsing.PanaceaVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public partial class AutoLabelIdChild
    {
        public AutoLabelIdChild(int childIndex)
        {
            ChildIndex = childIndex;
        }

        public override void Accept(global::Quire.EquationEditor.Tree.Parsing.PanaceaVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public partial class AutoLabelIdDescendant
    {
        public AutoLabelIdDescendant(global::Quire.Controls.Core.IdLabels newIdLabel, int childIndex)
        {
            NewIdLabel = newIdLabel;
            ChildIndex = childIndex;
        }

        public override void Accept(global::Quire.EquationEditor.Tree.Parsing.PanaceaVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public partial class TransformTo
    {
        public TransformTo(string mathNodeName)
        {
            MathNodeName = mathNodeName;
        }

        public override void Accept(global::Quire.EquationEditor.Tree.Parsing.PanaceaVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public partial class PullOutAdjacentDescendantFromChild
    {
        public PullOutAdjacentDescendantFromChild(int childIndex)
        {
            ChildIndex = childIndex;
        }

        public override void Accept(global::Quire.EquationEditor.Tree.Parsing.PanaceaVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public partial class ReparentNode
    {
        public ReparentNode(string newParent, int indexInNewParent)
        {
            NewParent = newParent;
            IndexInNewParent = indexInNewParent;
        }

        public override void Accept(global::Quire.EquationEditor.Tree.Parsing.PanaceaVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public partial class ReparentChild
    {
        public int ChildIndex
        {
            get => a;
            set => a = value;
        }

        public ReparentChild(string newParentNodeName, int childIndex) : this(newParentNodeName, childIndex, 0)
        {
        }

        public ReparentChild(string newParent, int childIndex, int indexInNewParent)
        {
            NewParent = newParent;
            ChildIndex = childIndex;
            IndexInNewParent = indexInNewParent;
        }

        public override void Accept(global::Quire.EquationEditor.Tree.Parsing.PanaceaVisitor visitor)
        {
            visitor.Visit(this);
        }

        public global::Quire.EquationEditor.Tree.Parsing.IChildHandlerPanacea Clone()
        {
            return new ReparentChild(NewParent, ChildIndex, IndexInNewParent);
        }
    }

    public partial class ReparentNodeAndPullOutChild
    {
        public ReparentNodeAndPullOutChild(string newParent, int childToExtractLocation, int parentIndexInNewParent, int childIndexInNewParent)
        {
            NewParent = newParent;
            ChildToExtractIndex = childToExtractLocation;
            ChildIndexInNewParent = childIndexInNewParent;
            ParentIndexInNewParent = parentIndexInNewParent;
        }

        public override void Accept(global::Quire.EquationEditor.Tree.Parsing.PanaceaVisitor visitor)
        {
            visitor.Visit(this);
        }
    }

    public partial class ReplaceChildDescriptor
    {
        public int ChildIndex
        {
            get => a;
            set => a = value;
        }

        public ReplaceChildDescriptor(int indexInParent, string newDescriptorName)
        {
            ChildIndex = indexInParent;
            NewDescriptorName = newDescriptorName;
        }

        public override void Accept(global::Quire.EquationEditor.Tree.Parsing.PanaceaVisitor visitor)
        {
            visitor.Visit(this);
        }

        public global::Quire.EquationEditor.Tree.Parsing.IChildHandlerPanacea Clone()
        {
            return new ReplaceChildDescriptor(ChildIndex, NewDescriptorName);
        }
    }
}
