using System.Collections.Generic;
using System.Linq;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.View;

namespace Quire.EquationEditor;

public partial class MathNodeToVisualNodeMap
{
    private IDictionary<MathNode, IVisualMathNode> b;
    private IDictionary<IVisualMathNode, MathNode> c;
    private MathNode d;
    private IVisualMathNode e;

    public MathNode RootModelNode
    {
        get => d;
        private set => d = value;
    }

    public IVisualMathNode RootVisualNode
    {
        get => e;
        private set => e = value;
    }

    public MathNode AssociateNode(IVisualMathNode visual)
    {
        if (visual != null && c != null && c.ContainsKey(visual))
        {
            return c[visual];
        }

        return null;
    }

    public IVisualMathNode AssociateNode(MathNode modelNode)
    {
        if (modelNode != null && b != null && b.ContainsKey(modelNode))
        {
            return b[modelNode];
        }

        return null;
    }

    public IEnumerable<T> AllVisualNodesOfType<T>() where T : IVisualMathNode
    {
        return c.Keys.OfType<T>();
    }
}
