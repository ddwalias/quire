using Quire.Controls.Core;
using Quire.EquationEditor.Tree.Transform;
using Quire.Mirror;

namespace Quire.EquationEditor.Tree;

public partial class IdentifierNodeInfo
{
    private IdLabels c;

    public new FormattedString Value
    {
        get => (FormattedString)base.Value;
        set => base.Value = value;
    }

    public override string ValueSExpression => InlineToSExpressionTransform.Default.Transform(Value.WpfTextBlock.Inlines.FirstInline);

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

    public IdentifierNodeInfo Clone()
    {
        return new IdentifierNodeInfo
        {
            Value = Value,
            IdLabel = IdLabel,
            ResolvedIdLabel = ResolvedIdLabel
        };
    }

    public override bool IsEquivalentTo(INodeInfo other)
    {
        if (!(other is IdentifierNodeInfo))
        {
            return false;
        }
        return IsEquivalentTo((IdentifierNodeInfo)other);
    }

    public bool IsEquivalentTo(IdentifierNodeInfo other)
    {
        if (other == null)
        {
            return false;
        }
        IdLabels? label = IdLabel;
        IdLabels? otherLabel = other.IdLabel;
        // Release IL evaluates both comparisons (non-short-circuit `and`), so `&` is kept.
        if (!(label.GetValueOrDefault() == otherLabel.GetValueOrDefault() & label.HasValue == otherLabel.HasValue))
        {
            return false;
        }
        return base.IsEquivalentTo(other);
    }

    [MirrorVerified("Release IL")]
    protected override object bk() => Clone();

    partial class a
    {
        [MirrorVerified("Release IL")]
        public a() { }
    }
}
