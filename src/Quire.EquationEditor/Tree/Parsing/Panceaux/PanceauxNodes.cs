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
}
