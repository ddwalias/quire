namespace Quire.Controls.Worksheet;

public partial class HeaderFooterOptionsDialog
{
    public HeaderFooterOptionsViewModel ViewModel { get; set; }

    private HeaderFooterOptionsDialog(IWorksheetViewModel worksheetViewModel)
    {
        InitializeComponent();
        HeaderFooterOptionsViewModel viewModel = new HeaderFooterOptionsViewModel(worksheetViewModel)
        {
            InvalidTotalPagesMessage = (string)TryFindResource("InvalidTotalPagesNumber"),
        };
        DataContext = ViewModel = viewModel;
        BindNumericUpDownActions();
    }
}
