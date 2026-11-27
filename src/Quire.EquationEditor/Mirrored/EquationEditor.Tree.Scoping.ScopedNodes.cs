using System.Collections.Generic;

using System.Collections.Generic;

using Quire.Mirror;
using Quire.Mirror;

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

    partial class a_
    {
        [MirrorVerified("Release IL")]
        public a_() { }
    }

    partial class b_
    {
        [MirrorVerified("Release IL")]
        public b_() { }
    }

    partial class b_
    {
        [MirrorVerified("Release IL")]
        internal bool a(IList<MathNode> iList) => Enumerable.Any<MathNode>(iList, a_);
    }
}
