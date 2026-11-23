using Quire.Controls.Core;

using Quire.Mirror;

namespace Quire.EquationEditor.Tree;

public partial class IdentifierNodeInfo
{
    private IdLabels c;

    public bool HasNoLabel => IdLabel.IsNullOrNone();

    public IdLabels ResolvedIdLabel
    {
        get => c;
        set => c = value;
    }

    public IdLabels IdLabelForDisplay
    {
        get
        {
            if (!HasNoLabel)
            {
                return IdLabel.Value;
            }
            return ResolvedIdLabel;
        }
    }

    [MirrorVerified("Release IL")]
    protected override object bk() => Clone();

    partial class a
    {
        [MirrorVerified("Release IL")]
        public a() { }
    }
}
