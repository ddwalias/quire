namespace Quire.Controls.Plots.ViewModel;

public partial class TickmarkFormatter
{
    public TickmarkFormatter(Core.NumberFormat numberFormat)
        : base(numberFormat, new TickmarkFormatterHelper())
    {
    }
}
