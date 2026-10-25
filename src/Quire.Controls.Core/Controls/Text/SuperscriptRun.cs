#nullable disable
using System;
using System.Windows;
using System.Windows.Documents;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Text;

public partial class SuperscriptRun
{
    private static readonly Type TypeOfThis;

    [MirrorVerified("PtcControlsTextSuperscriptRunMechanicalDifferentialTests")]
    public static readonly BaselineAlignment DefaultAlignment;

    [MirrorVerified("PtcControlsTextSuperscriptRunMechanicalDifferentialTests")]
    private void InitBaseline() => base.BaselineAlignment = DefaultAlignment;

    [MirrorVerified("PtcControlsTextSuperscriptRunMechanicalDifferentialTests")]
    protected override FontVariants FontVariantsForCopy => (FontVariants)1;

    static SuperscriptRun()
    {
        TypeOfThis = typeof(SuperscriptRun);
        DefaultAlignment = CustomScriptsSettings.SuperscriptAlignment;
        Inline.BaselineAlignmentProperty.OverrideMetadata(TypeOfThis, new FrameworkPropertyMetadata(Inline.BaselineAlignmentProperty.DefaultMetadata.DefaultValue, null, CoerceBaselineAlignmentCallback));
    }

    private static object CoerceBaselineAlignmentCallback(DependencyObject depObj, object value)
    {
        return DefaultAlignment;
    }

    public SuperscriptRun()
    {
        InitBaseline();
    }

    public SuperscriptRun(string text)
        : base(text)
    {
        InitBaseline();
    }

    public SuperscriptRun(string text, TextPointer insertionPosition)
        : base(text, insertionPosition)
    {
        InitBaseline();
    }
}
