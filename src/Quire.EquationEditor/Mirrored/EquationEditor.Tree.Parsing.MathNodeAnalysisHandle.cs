namespace Quire.EquationEditor.Tree.Parsing;

public partial struct MathNodeAnalysisHandle
{
    public readonly MathNode Node;

    public readonly Fixity EffectiveFixity;

    public readonly Associativity EffectiveAssociativity;

    public MathNodeAnalysisHandle(MathNode node)
        : this(node, node.Fixity, node.Associativity)
    {
    }

    public MathNodeAnalysisHandle(MathNode node, Fixity fixity, Associativity associativity)
    {
        Node = node;
        EffectiveAssociativity = associativity;
        switch (fixity)
        {
            case Fixity.Outfix:
                EffectiveFixity = Fixity.None;
                break;
            case Fixity.Special:
            {
                bool leftIsEdge = !node.IsContainerSlot(0) && !node.ChildNotPrimal(0);
                bool rightIsEdge = (!node.IsContainerSlot(node.ChildCount - 1) && !node.ChildNotPrimal(node.ChildCount - 1)) || node.ConsiderAsPrefixInInsertion;
                if (leftIsEdge & rightIsEdge)
                {
                    EffectiveFixity = Fixity.Infix;
                }
                else if (leftIsEdge)
                {
                    EffectiveFixity = Fixity.Suffix;
                }
                else if (rightIsEdge)
                {
                    EffectiveFixity = Fixity.Prefix;
                }
                else
                {
                    EffectiveFixity = Fixity.None;
                }
                break;
            }
            default:
                EffectiveFixity = fixity;
                break;
        }
        if (EffectiveFixity == Fixity.Infix && EffectiveAssociativity == Associativity.None)
        {
            EffectiveAssociativity = Associativity.Left;
        }
    }
}
