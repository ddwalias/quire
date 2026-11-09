using System.Collections.Generic;
using Quire.Controls.Core;

namespace Quire.EquationEditor.SpecTable;

public partial class SpecTableCellMap
{
    public IEnumerable<IEquationControl> Views => b.Keys;

    public void Map(IEquationModel eqModel, IEquationControl eqView)
    {
        GetAssociatedView(eqModel)?.DetachFromModel();
        a[eqModel] = eqView;
        b[eqView] = eqModel;
    }

    public IEquationControl GetAssociatedView(IEquationModel cellModel)
    {
        if (cellModel == null || !a.TryGetValue(cellModel, out var view))
        {
            return null;
        }
        return view;
    }

    public IEquationModel GetAssociatedModel(IEquationControl viewObj)
    {
        if (viewObj == null || !b.TryGetValue(viewObj, out var model))
        {
            return null;
        }
        return model;
    }
}
