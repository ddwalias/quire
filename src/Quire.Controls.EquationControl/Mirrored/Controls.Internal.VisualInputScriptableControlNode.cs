using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;

namespace Quire.Controls.Internal;

internal partial class VisualInputScriptableControlNode
{
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.EquationControl;component/internal/visualinputscriptablecontrolnode.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                _visual = (VisualInputScriptableControlNode)target;
                return;
            case 2:
                xamlBorder = (Border)target;
                return;
            case 3:
                xamlGrid = (Grid)target;
                return;
            case 4:
                xamlToggle = (ToggleButton)target;
                return;
            case 5:
                xamlScrollViewer = (ScrollViewer)target;
                return;
            default:
                _contentLoaded = true;
                return;
        }
    }
}
