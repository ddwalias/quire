using System;
using System.Windows;
using Quire.Controls.Core;
using Quire.EquationEditor;
using Quire.EquationEditor.Equation;

namespace Quire.Controls;

internal partial class EquationTriadBuilder
{
    public IEquationControl BuildEquationControl(IEquationModel eqModel)
    {
        EquationControl control = new EquationControl((EquationModel)eqModel);
        AttachStyleFromExternalResource(
            control,
            "pack://application:,,,/Quire.Controls.EquationControl;component/SpecTableCellResources.xaml",
            ("SPEC_TABLE_SCALING_CELL" == eqModel.Root.DescriptorName) ? "SpecTableScalingEqStyle" : "SpecTableCellEqStyle");
        return control;
    }

    private static void AttachStyleFromExternalResource(EquationControl control, string sourceUri, string styleName)
    {
        ResourceDictionary resourceDictionary = new ResourceDictionary
        {
            Source = new Uri(sourceUri)
        };
        control.Style = (Style)resourceDictionary[styleName];
    }
}
