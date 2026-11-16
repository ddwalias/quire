namespace Quire.Controls.Plots.Model;

public partial interface IHasNumberOfPoints
{
    bool IsNumOfPointsMutable { get; }
    int NumberOfPoints { get; set; }
}
