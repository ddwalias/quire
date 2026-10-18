#nullable disable
using System.Windows.Controls;
using Quire.Mirror;

namespace Quire.Controls;

public partial class CopyWidget
{
    [MirrorVerified("PtcControlsCopyWidgetMechanicalDifferentialTests")]
    private readonly CopyWidgetViewModel _viewModel;

    [MirrorVerified("PtcControlsCopyWidgetMechanicalDifferentialTests")]
    public int TotalPages { get => _viewModel.TotalPages; set => _viewModel.TotalPages = value; }

    [MirrorVerified("PtcControlsCopyWidgetMechanicalDifferentialTests")]
    public Nullable<DateTime> DisplaySavedDate { get => _viewModel.DisplaySavedDate; set => _viewModel.DisplaySavedDate = value; }

    [MirrorVerified("PtcControlsCopyWidgetMechanicalDifferentialTests")]
    public string DisplayFileName { get => _viewModel.DisplayFileName; set => _viewModel.DisplayFileName = value; }

    [MirrorVerified("PtcControlsCopyWidgetMechanicalDifferentialTests")]
    public string DisplayFilePath { get => _viewModel.DisplayFilePath; set => _viewModel.DisplayFilePath = value; }

    [MirrorVerified("PtcControlsCopyWidgetMechanicalDifferentialTests")]
    public string DisplayDocumentName { get => _viewModel.DisplayDocumentName; set => _viewModel.DisplayDocumentName = value; }

    [MirrorVerified("PtcControlsCopyWidgetMechanicalDifferentialTests")]
    public int PageNumber { get => _viewModel.PageNumber; set => _viewModel.PageNumber = value; }

    [MirrorVerified("PtcControlsCopyWidgetMechanicalDifferentialTests")]
    public override double ParentWidth { get => _viewModel.ParentWidth; set => _viewModel.ParentWidth = value; }

    [MirrorVerified("PtcControlsCopyWidgetMechanicalDifferentialTests")]
    public override double IncludeWidth { get => _viewModel.IncludeWidth; set => _viewModel.IncludeWidth = value; }

    [MirrorVerified("PtcControlsCopyWidgetMechanicalDifferentialTests")]
    private Border _childBorder;

    [MirrorVerified("PtcControlsCopyWidgetMechanicalDifferentialTests")]
    public override Border ChildBorder => _childBorder;
}
