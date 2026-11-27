using System.Collections.Generic;
using System.Linq;
using Quire.Core;

namespace Quire.EquationEditor.Tree;

public partial class ComboBoxInputControlData
{
    public static ComboBoxInputControlData CreateComboBoxInputControlData(int rows, int cols, int selectedId, string tableName, IList<string> titlesList, IList<string> unitsList, IList<string> valuesList, StringToComboBoxMathTransformer transformer)
    {
        var units = unitsList ?? Enumerable.Repeat(string.Empty, cols).ToList();
        var titles = titlesList ?? Enumerable.Repeat(string.Empty, rows).ToList();
        var values = valuesList ?? Enumerable.Repeat(string.Empty, cols * rows).ToList();
        var data = new ComboBoxInputControlData(rows, cols, selectedId, tableName, titles, units, values, transformer);
        if (!DebugTools.DevAssert(data.IsValid(), null).Passed)
        {
            return null;
        }

        return data;
    }
}
