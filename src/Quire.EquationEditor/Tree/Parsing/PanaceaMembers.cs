using Quire.EquationEditor.Tree.Parsing.Panceaux;

namespace Quire.EquationEditor.Tree.Parsing
{
    public abstract partial class Panacea
    {
        public virtual void Accept(PanaceaVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected Panacea()
        {
        }
    }

    public partial class PanaceaVisitor
    {
        public virtual void Visit(Panacea visited)
        {
            if (PanaceaHandler != null)
            {
                PanaceaHandler(visited);
            }
        }

        public virtual void Visit(ReparentChild visited)
        {
            if (ReparentChildHandler != null)
            {
                ReparentChildHandler(visited);
            }
        }

        public virtual void Visit(ReparentNode visited)
        {
            if (ReparentNodeHandler != null)
            {
                ReparentNodeHandler(visited);
            }
        }

        public virtual void Visit(ReplaceChildDescriptor visited)
        {
            if (ReplaceChildDescriptorHandler != null)
            {
                ReplaceChildDescriptorHandler(visited);
            }
        }

        public virtual void Visit(TransformTo visited)
        {
            if (TransformToHandler != null)
            {
                TransformToHandler(visited);
            }
        }

        public virtual void Visit(BinaryMergeValueChildren visited)
        {
            if (BinaryMergeValueChildrenHandler != null)
            {
                BinaryMergeValueChildrenHandler(visited);
            }
        }

        public virtual void Visit(ReplaceWithDefaultNode visited)
        {
            if (ReplaceWithDefaultNodeHandler != null)
            {
                ReplaceWithDefaultNodeHandler(visited);
            }
        }

        public virtual void Visit(ReplaceWithChild visited)
        {
            if (ReplaceWithChildHandler != null)
            {
                ReplaceWithChildHandler(visited);
            }
        }

        public virtual void Visit(ReplaceWith visited)
        {
            if (ReplaceWithHandler != null)
            {
                ReplaceWithHandler(visited);
            }
        }

        public virtual void Visit(AutoLabelIdDescendant visited)
        {
            if (AutoLabelIdDescendantHandler != null)
            {
                AutoLabelIdDescendantHandler(visited);
            }
        }

        public virtual void Visit(AutoLabelIdChild visited)
        {
            if (AutoLabelIdChildHandler != null)
            {
                AutoLabelIdChildHandler(visited);
            }
        }

        public virtual void Visit(ReparentNodeAndPullOutChild visited)
        {
            if (ReparentNodeAndPullOutChildHandler != null)
            {
                ReparentNodeAndPullOutChildHandler(visited);
            }
        }

        public virtual void Visit(PullOutAdjacentDescendantFromChild visited)
        {
            if (PullOutAdjacentDescendantFromChildHandler != null)
            {
                PullOutAdjacentDescendantFromChildHandler(visited);
            }
        }

        public virtual void Visit(InsertChildrenIntoParent visited)
        {
            if (InsertChildrenIntoParentHandler != null)
            {
                InsertChildrenIntoParentHandler(visited);
            }
        }

        public PanaceaVisitor()
        {
        }
    }
}
