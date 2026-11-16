using System;
using Quire.EquationEditor.Tree.Parsing;

namespace Quire.EquationEditor.Tree;

public partial class NearestNeighborSelector
{
    public NearestNeighborSelector()
    {
    }

    public MathNode[] SelectNeighbors(MathNode current)
    {
        bool neighborsShareParent;
        return SelectNeighbors(current, out neighborsShareParent);
    }

    public virtual MathNode[] SelectNeighbors(MathNode current, out bool neighborsShareParent)
    {
        if (current == null)
        {
            throw new ArgumentNullException("current");
        }
        MathNode mathNode;
        MathNode mathNode2;
        MathNode[] array;
        switch (current.Fixity)
        {
        case Fixity.Infix:
        {
            mathNode = current[1];
            mathNode2 = mathNode.LeftAdjacentSubexpression(considerAssociativity: true) ?? current[0];
            neighborsShareParent = mathNode.Parent == mathNode2.Parent;
            return new MathNode[2] { mathNode2, mathNode };
        }
        case Fixity.Prefix:
        case Fixity.Suffix:
        case Fixity.Outfix:
            neighborsShareParent = true;
            return new MathNode[1] { current[0] };
        case Fixity.None:
            neighborsShareParent = true;
            return new MathNode[0];
        case Fixity.Special:
        {
            neighborsShareParent = true;
            array = current.Children.ToArray();
            if (current.IsEffectiveInfix)
            {
                mathNode = current[current.ChildCount - 1];
                mathNode2 = mathNode.LeftAdjacentSubexpression(considerAssociativity: true) ?? current[0];
                neighborsShareParent = mathNode.Parent == mathNode2.Parent;
                array[0] = mathNode2;
                array[current.ChildCount - 1] = mathNode;
            }
            return array;
        }
        default:
            throw new NotImplementedException("This was...well, unexpected");
        }
    }
}
