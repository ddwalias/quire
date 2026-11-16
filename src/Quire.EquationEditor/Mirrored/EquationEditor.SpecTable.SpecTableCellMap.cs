using System.Collections.Generic;
using Quire.Controls.Core;

namespace Quire.EquationEditor.SpecTable;

public partial class SpecTableCellMap
{
    private readonly Dictionary<IEquationModel, IEquationControl> a = new Dictionary<IEquationModel, IEquationControl>();
    private readonly Dictionary<IEquationControl, IEquationModel> b = new Dictionary<IEquationControl, IEquationModel>();

    public SpecTableCellMap()
    {
    }

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
