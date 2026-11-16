namespace Quire.Controls.Worksheet;

public static partial class WorksheetPageExtensions
{
    private static IWorksheetPageViewModel GetViewModel(this WorksheetPageControl control)
    {
        return control.DataContext as IWorksheetPageViewModel;
    }

    public static IWorksheetPageSection GetPageBody(this WorksheetPageControl control)
    {
        return control.GetViewModel().PageBody;
    }
}
