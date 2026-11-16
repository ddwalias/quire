namespace Quire.Controls.Plots.Plot3D.Model;

internal partial interface IEquationListModel3D
{
    PersistentDataObjects.plotEquation[] GetDataObject(Calculation.Core.IResultManager resultManager);
}
