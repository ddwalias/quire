using System.Linq;

namespace Quire.Controls.Plots.Plot3D.Model;

internal partial class EquationListModel3D
{
    public PersistentDataObjects.plotEquation[] GetDataObject(Calculation.Core.IResultManager resultManager)
    {
        return this.Select(equation => equation.GetDataObject()).ToArray();
    }

    public EquationListModel3D()
    {
    }
}
