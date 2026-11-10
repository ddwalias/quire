using System.Windows;

namespace Quire.Controls.Internal;

internal partial class VisualComboBoxControlEditor
{
    public VisualComboBoxControlEditor(VisualComboBoxControlNode editNode, VisualMathRegionNode region)
        : base(editNode, region)
    {
    }

    public VisualComboBoxControlEditor(VisualComboBoxControlNode editNode, FrameworkElement adornedElement, VisualMathRegionNode region)
        : base(editNode, adornedElement, region)
    {
    }
}
