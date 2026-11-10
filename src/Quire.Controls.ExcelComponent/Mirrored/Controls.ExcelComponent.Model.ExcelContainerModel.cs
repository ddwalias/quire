using Quire.FunctionalitiesLimitation;

namespace Quire.Controls.ExcelComponent.Model;

public partial class ExcelContainerModel
{
    public override string ProgId => "Excel.Sheet";

    public override bool ShouldCommitAtLeastOnce => true;

    public override bool ShouldCommitIfDirty => true;

    protected override bool CheckOleActiveState => true;

    protected override EnumFunctionalityKey ToggleActivateStateFunctionalityKey => (EnumFunctionalityKey)797;

    public global::System.Nullable<bool> IsAsIcon { get; set; }

    public bool NeedRecalculateOnClose { get; set; }

    public QueryAttachedInputsOrOutputsHandler QueryAttachedInputsOrOutputs { get; set; }

    private bool IsDeactivateInitiatedFromMathcad { get; set; }

    private void OnWorkbookOpen(global::Quire.Microsoft.Office.Interop.Excel.Workbook wb)
    {
    }

    internal global::Quire.Microsoft.Office.Interop.Excel.Workbook GetSafeWorkBook(bool forceRun)
    {
        try
        {
            return GetWorkbook(forceRun);
        }
        catch
        {
            return null;
        }
    }

    private void FireRequestEvaluation()
    {
        if (RequestEvaluation != null)
        {
            RequestEvaluation();
        }
    }

    private global::Quire.Microsoft.Office.Interop.Excel.Sheets GetWorksheets(bool forceRun)
    {
        global::Quire.Microsoft.Office.Interop.Excel.Workbook workbook = GetWorkbook(forceRun);
        if (workbook == null)
        {
            return null;
        }
        return workbook.Worksheets;
    }
}
