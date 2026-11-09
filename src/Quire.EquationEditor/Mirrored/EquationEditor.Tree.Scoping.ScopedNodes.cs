using System.Collections.Generic;

namespace Quire.EquationEditor.Tree.Scoping;

public partial class ScopedNodes
{
    private readonly IList<IList<MathNode>> a;

    private readonly bool b;

    public ScopedNodes(IList<IList<MathNode>> scopedNodesByLines, bool scopeAsSingleBlock = false)
    {
        a = scopedNodesByLines;
        b = scopeAsSingleBlock;
        if (scopeAsSingleBlock)
        {
            DebugTools.DevAssert(scopedNodesByLines.Count == 1);
        }
    }

    public IList<IList<MathNode>> ScopedLines => a;

    public MathNode GetFirstScopedNode
    {
        get
        {
            if (ScopedLines.Count == 0)
            {
                return null;
            }
            var firstLine = ScopedLines[0];
            if (firstLine.Count > 0)
            {
                return firstLine[0];
            }
            DebugTools.DevAssert(ScopedLines.Count == 1);
            return null;
        }
    }

    public bool ScopeAsSingleBlock => b;

    public MathNode GetLastScopedNode
    {
        get
        {
            if (ScopedLines.Count == 0)
            {
                return null;
            }
            var lastLine = ScopedLines[ScopedLines.Count - 1];
            if (lastLine.Count > 0)
            {
                return lastLine[lastLine.Count - 1];
            }
            DebugTools.DevAssert(ScopedLines.Count == 1);
            return null;
        }
    }
}
