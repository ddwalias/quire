using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using Quire.Controls.Core;
using Quire.Controls.InputControls;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.Internal;

internal partial class VisualInputScriptableControlNode
{
    public new static readonly Type TypeOfThis;
    internal VisualInputScriptableControlNode _visual;
    internal Border xamlBorder;
    internal Grid xamlGrid;
    internal ToggleButton xamlToggle;
    internal ScrollViewer xamlScrollViewer;
    private bool _contentLoaded;

    static VisualInputScriptableControlNode()
    {
        TypeOfThis = typeof(VisualInputScriptableControlNode);
        VisualInputScriptableControlNodeBase.ContentChangedEvent = CommonRoutedEvents.ContentChangedEvent.AddOwner(TypeOfThis);
    }

    public VisualInputScriptableControlNode()
    {
        InitializeComponent();
    }

    public VisualInputScriptableControlNode(string mathNodeName, MathNode mathNode, VisualMathRegionNode region)
    {
        _inputControl = InitInputControl(mathNodeName, mathNode);
        DataContext = ViewModel;
        ((IInputScriptableControlModelWithInputs)ViewModel.Model).CommitNeeded += HandleCommitNeeded;
        ((IInputScriptableControlModelWithInputs)ViewModel.Model).PropertyChanged += HandleModelPropertyChanged;
        BindCommands();
        InitializeComponent();
        _scrollViewer = xamlScrollViewer;
        _grid = xamlGrid;
        _toggle = xamlToggle;
        _border = xamlBorder;
        init(mathNodeName, mathNode, region);
        AddHandler(UIElement.MouseDownEvent, new MouseButtonEventHandler(HandleMouseDown), handledEventsToo: true);
        AddHandler(UIElement.GotFocusEvent, new RoutedEventHandler(HandleGotFocus), handledEventsToo: true);
        AddHandler(UIElement.LostFocusEvent, new RoutedEventHandler(HandleLostFocus), handledEventsToo: true);
    }

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
