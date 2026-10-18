#nullable disable
using System.Windows;
using Quire.Controls.Core.BookmarkTags;
using Quire.Controls.Core.IORegions;
using Quire.Mirror;

namespace Quire.Controls.Core;

public static partial class DocumentAttachedProperties
{
    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty DocCalculationOptionsProperty;

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetDocCalculationOptions(DependencyObject o, CalculationOptions value) => o.SetValue(DocCalculationOptionsProperty, value);

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty DocCalculationVariablesProperty;

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetDocCalculationVariables(DependencyObject o, CalculationVariables value) => o.SetValue(DocCalculationVariablesProperty, value);

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty InputOutputRegionManagerProperty;

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetInputOutputRegionManager(DependencyObject region, IIORegionManager value) => region.SetValue(InputOutputRegionManagerProperty, value);

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty BookmarksManagerProperty;

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetBookmarksManager(DependencyObject region, IBookmarksManager value) => region.SetValue(BookmarksManagerProperty, value);

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty LabelProperty;

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetLabel(DependencyObject dependencyObject, string value) => dependencyObject.SetValue(LabelProperty, value);

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty FilePathProperty;

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void SetFilePath(DependencyObject o, string value) => o.SetValue(FilePathProperty, value);

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty IsInPrintingProperty;

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearIsInPrinting(DependencyObject o) => o.ClearValue(IsInPrintingProperty);

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty IsInCopyProperty;

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearIsInCopy(DependencyObject o) => o.ClearValue(IsInCopyProperty);

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty IsInPastingProperty;

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearIsInPasting(DependencyObject o) => o.ClearValue(IsInPastingProperty);

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static readonly DependencyProperty CopyAsImageSettingsProperty;

    [MirrorVerified("PtcControlsCoreDocumentAttachedPropertiesMechanicalDifferentialTests")]
    public static void ClearCopyAsImageSettings(DependencyObject target) => target.ClearValue(CopyAsImageSettingsProperty);
}
