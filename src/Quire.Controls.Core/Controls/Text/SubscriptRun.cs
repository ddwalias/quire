#nullable disable
using System;
using System.Windows;
using System.Windows.Documents;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Text;

public partial class SubscriptRun
{
    private static readonly Type TypeOfThis;

    [MirrorVerified("PtcControlsTextSubscriptRunMechanicalDifferentialTests")]
    public static readonly BaselineAlignment DefaultAlignment;

    [MirrorVerified("PtcControlsTextSubscriptRunMechanicalDifferentialTests")]
    private void InitBaseline() => base.BaselineAlignment = DefaultAlignment;

    [MirrorVerified("PtcControlsTextSubscriptRunMechanicalDifferentialTests")]
    protected override FontVariants FontVariantsForCopy => (FontVariants)2;

    static SubscriptRun()
    {
        TypeOfThis = typeof(SubscriptRun);
        DefaultAlignment = CustomScriptsSettings.SubscriptAlignment;
        Inline.BaselineAlignmentProperty.OverrideMetadata(TypeOfThis, new FrameworkPropertyMetadata(Inline.BaselineAlignmentProperty.DefaultMetadata.DefaultValue, null, CoerceBaselineAlignmentCallback));
    }

    private static object CoerceBaselineAlignmentCallback(DependencyObject depObj, object value)
    {
        return DefaultAlignment;
    }

    public SubscriptRun()
    {
        InitBaseline();
    }

    public SubscriptRun(string text)
        : base(text)
    {
        InitBaseline();
    }

    public SubscriptRun(string text, TextPointer insertionPosition)
        : base(text, insertionPosition)
    {
        InitBaseline();
    }
}
