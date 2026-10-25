#nullable disable
using System;
using System.Windows;
using Quire.Controls.Core;
using Quire.Mirror;

namespace Quire;

public partial class SingleRichTextCopyVisitor : ISpecialCopyVisitor
{
    [MirrorVerified("PtcSingleRichTextCopyVisitorMechanicalDifferentialTests")]
    private readonly IDataObject _dataObject;

    [MirrorVerified("PtcSingleRichTextCopyVisitorMechanicalDifferentialTests")]
    public SingleRichTextCopyVisitor(IDataObject dataObject)
    {
        _dataObject = dataObject;
    }

    [MirrorVerified("PtcSingleRichTextCopyVisitorMechanicalDifferentialTests")]
    public void Visit(IEquationControl equationControl) => equationControl.AddToDataObject(_dataObject);

    public void Visit(IRichTextWithEmbeddedItem textBox)
    {
        DebugTools.FailAssertion(null, Array.Empty<object>());
    }

    public void Visit(ICollapsibleAreaControl collapsibleArea)
    {
        DebugTools.FailAssertion(null, Array.Empty<object>());
    }
}
