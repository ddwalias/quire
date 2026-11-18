using System;
using System.Runtime.CompilerServices;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Navigation;

namespace Quire.Native.PtcEquationEditor;

internal partial class bn
{
    private readonly MathNode a;
    private readonly int b;
    private readonly int c;
    private readonly string d;

    [SkipLocalsInit]
    public bn(a5 host, MathNode editNode, int selectionStart, int selectionEnd, string replacement)
        : base(host)
    {
        if (editNode == null)
        {
            throw new ArgumentNullException("editNode");
        }

        if (!editNode.CarriesValue)
        {
            throw new ArgumentException("edit node does not carry a value", "editNode");
        }

        if (editNode.DescriptorName != "STRING")
        {
            throw new ArgumentException("edit node is not a string node", "editNode");
        }

        if (selectionStart > selectionEnd)
        {
            throw new ArgumentOutOfRangeException("selectionStart");
        }

        if (selectionStart < 0 || selectionStart > editNode.NodeInfoValueLength)
        {
            throw new ArgumentOutOfRangeException("selectionStart");
        }

        if (selectionEnd > editNode.NodeInfoValueLength)
        {
            throw new ArgumentOutOfRangeException("selectionEnd");
        }

        a = editNode;
        b = selectionStart;
        c = selectionEnd;
        d = replacement;
    }
}
