using System.Collections.Generic;
using System.Windows.Controls;
using Quire.Controls.Core;

namespace Quire.Controls.InputControls;

internal partial class EditableComboBoxLayoutManager
{
    public EditableComboBoxLayoutManager()
    {
    }

    public EditableComboBoxLayoutManager(EditableComboBoxLayoutManager other)
        : base(other)
    {
    }

    public override object Clone()
    {
        return new EditableComboBoxLayoutManager(this);
    }

    protected override MaxMatrixWidth[] CalcMaxDecimalPerColumn(IList<LayoutMetrics> cellsInfo, UIElementCollection cellsContent, int rows)
    {
        DebugTools.FailAssertion((string)null);
        return new MaxMatrixWidth[cellsContent.Count];
    }
}
