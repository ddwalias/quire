namespace Quire.Controls.ExcelComponent.ViewModel;

public partial class ExcelContainerViewModel
{
    protected override bool IsStrictMinSize => false;

    protected override bool MustTakeLockOnUserDeactivation => false;

    public global::System.Nullable<bool> GetExcelVersionInstalled(bool forceRun)
    {
        return Model.GetExcelVersionInstalled(forceRun);
    }

    public bool Select(string rangeAddress, bool forceRun)
    {
        return Model.Select(rangeAddress, forceRun);
    }
}
