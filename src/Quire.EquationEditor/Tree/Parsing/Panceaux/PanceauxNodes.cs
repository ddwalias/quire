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
}
