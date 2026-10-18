#nullable disable
using System.Windows;
using Quire.Controls.Core;
using Quire.Mirror;

namespace Quire;

public partial class SingleRichTextCopyVisitor
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
}
