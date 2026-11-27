#nullable disable

namespace Quire;

public partial class SpiritMainWindow
{
    private void CanReplaceAndFindNext(
        object sender,
        global::System.Windows.Input.CanExecuteRoutedEventArgs e)
    {
        var findReplaceQuery = new global::Quire.Controls.Core.FindReplaceQuery(
            FindProperties.FindText,
            FindProperties.ReplaceText,
            FindProperties.Options)
        {
            Status = null
        };
        e.CanExecute = !ReplaceIsNotAllowedInFreemiumMode() &&
                       global::Quire.Controls.Core.RegionCommands.Replace.CanExecute(findReplaceQuery, null);
        if (findReplaceQuery.Status.HasValue)
        {
            global::System.Windows.Controls.ToolTipService.SetShowOnDisabled(
                (global::System.Windows.DependencyObject)FindBar.ReplaceButton,
                !global::Quire.Controls.Core.FindReplaceStatusExtensions.IsEmpty(findReplaceQuery.Status));
            FindBar.ReplaceButtonTooltipTextBlock.Tag = findReplaceQuery.Status;
        }
    }

    private void HandleReplaceAndFindNext(
        object sender,
        global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        var findReplaceQuery = new global::Quire.Controls.Core.FindReplaceQuery(
            FindProperties.FindText,
            FindProperties.ReplaceText,
            FindProperties.Options,
            FindProperties.FindTextSubscriptPositions,
            FindProperties.ReplaceTextSubscriptPositions)
        {
            Status = null
        };
        global::Quire.Controls.Core.RegionCommands.Replace.Execute(findReplaceQuery, null);
        global::Quire.DebugTools.DevAssert(
            findReplaceQuery.Status.GetValueOrDefault(out var result),
            null);
        if (!global::Quire.Controls.Core.FindReplaceStatusExtensions.IsAtDocumentBoundary(FindProperties.Status) ||
            !global::Quire.Controls.Core.FindReplaceStatusExtensions.IsEmpty(result))
        {
            FindProperties.Status = result;
        }

        bool foundNext = false;
        if (global::Quire.Controls.Core.FindReplaceStatusExtensions.IsEmpty(result) && e.Parameter as bool? != false)
        {
            foundNext = HandleFindNext();
        }

        findReplaceQuery.PostReplaceAndFindNext?.Invoke(foundNext);
    }

    private bool Find(global::System.Windows.Documents.LogicalDirection direction)
    {
        global::Quire.Controls.Core.FindReplaceStatus status = FindProperties.Status;
        FindProperties.Status = global::Quire.Controls.Core.FindReplaceStatus.Empty;
        bool found = false;
        if (!string.IsNullOrEmpty(FindProperties.FindText))
        {
            bool canContinue =
                (direction == global::System.Windows.Documents.LogicalDirection.Forward &&
                 status != global::Quire.Controls.Core.FindReplaceStatus.EndOfDocument) ||
                (direction == global::System.Windows.Documents.LogicalDirection.Backward &&
                 status != global::Quire.Controls.Core.FindReplaceStatus.BeginningOfDocument);
            found = ActiveDocument.Find(
                FindProperties.FindText,
                direction,
                canContinue,
                FindProperties.Options,
                FindProperties.FindTextSubscriptPositions);
            if (!found)
            {
                FindProperties.Status = canContinue
                    // Mathcad loads the Forward constant before direction for this comparison.
                    ? global::System.Windows.Documents.LogicalDirection.Forward == direction
                        ? global::Quire.Controls.Core.FindReplaceStatus.EndOfDocument
                        : global::Quire.Controls.Core.FindReplaceStatus.BeginningOfDocument
                    : global::Quire.Controls.Core.FindReplaceStatus.NotFound;
            }
        }

        return found;
    }

    private void HandleGoToPage(
        object sender,
        global::System.Windows.Input.ExecutedRoutedEventArgs e)
    {
        global::Quire.Controls.Core.IEngineeringDocument activeDocument = ActiveDocument;
        var spiritMainWindow = global::Quire.Wpf.WpfUtils.TryFindVisualAncestor<SpiritMainWindow>(
            activeDocument as global::System.Windows.DependencyObject);
        new global::Quire.FunctionalitiesLimitation.InputBindingFunctionalityCommandWrapper(
                global::Quire.Controls.Worksheet.WorksheetCommands.EditCurrentGotoPage)
            .Execute(null, (global::Quire.Controls.Worksheet.WorksheetControl)spiritMainWindow.ActiveDocument.Worksheet);
    }
}
