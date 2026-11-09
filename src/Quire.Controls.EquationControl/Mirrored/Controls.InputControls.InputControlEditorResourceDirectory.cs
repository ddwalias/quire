using System;
using System.Windows;
using System.Windows.Markup;

namespace Quire.Controls.InputControls;

public partial class InputControlEditorResourceDirectory
{
    public static readonly Type[] KeysOwnerTypes = new Type[] { typeof(InputControlEditorErrors) };

    private bool _contentLoaded;

    public InputControlEditorResourceDirectory()
    {
        InitializeComponent();
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.EquationControl;component/inputcontrols/editor/inputcontroleditorresourcedirectory.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
