using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Quire.Controls.Core;
using Quire.Core;
using Quire.EquationEditor.View;

namespace Quire.Controls.InputControls;

public partial class EditableComboBoxTableBorder : Border, IComponentConnector
{
    public static readonly Size MinimumVisibleEditableTableSize = new Size(57.0, 71.0);

    public static readonly Brush BorderBrushColor = new SolidColorBrush(Color.FromArgb(byte.MaxValue, 78, 126, 155));

    public const double TableValuesSeparatingLineThickness = 0.5;

    public static readonly Thickness EditableComboBoxBorderThickness = new Thickness(0.5);

    private readonly EditableComboBoxVisualTable _editableComboBoxView;

    internal ScrollViewer ECB_ScrollViewer;

    internal Grid tableGrid;

    internal Line horizontalLine;

    internal Line verticalLine;

    private bool _contentLoaded;

    public bool IsEditModeActive => _editableComboBoxView.IsActive;

    public IEditableComboBoxVisualTable EditableTable => _editableComboBoxView;

    public EditableComboBoxTableBorder(EditableComboBoxVisualTable editableComboBoxView)
    {
        _editableComboBoxView = editableComboBoxView;
        DataContext = _editableComboBoxView.ViewModel;
        InitializeComponent();
        tableGrid.Children.Insert(0, _editableComboBoxView);
    }

    public void Refresh(EditableComboBoxTableCellsFontFormats cellsFontFormats, ExtendedFont operatorFormat)
    {
        _editableComboBoxView.Refresh(cellsFontFormats, operatorFormat);
    }

    private void OnDragDeltaResizeTable(object sender, DragDeltaEventArgs e)
    {
        double num = ECB_ScrollViewer.ViewportWidth + e.HorizontalChange;
        if (num.IsGreaterThan(MinimumVisibleEditableTableSize.Width, 1E-05))
        {
            double maximumWidth = ECB_ScrollViewer.ExtentWidth + SystemParameters.VerticalScrollBarWidth;
            ECB_ScrollViewer.MaxWidth = (num.IsLessThan(maximumWidth, 1E-05) ? num : maximumWidth);
        }

        double requestedHeight = ECB_ScrollViewer.ViewportHeight + e.VerticalChange;
        if (requestedHeight.IsGreaterThan(MinimumVisibleEditableTableSize.Height, 1E-05))
        {
            double maximumHeight = ECB_ScrollViewer.ExtentHeight + SystemParameters.HorizontalScrollBarHeight;
            ECB_ScrollViewer.MaxHeight = (requestedHeight.IsLessThan(maximumHeight, 1E-05) ? requestedHeight : maximumHeight);
        }
    }

    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
        e.Handled = true;
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.EquationControl;component/inputcontrols/implementations/comboboxinputcontrol/editablecomboboxtableborder.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
        case 1:
            ECB_ScrollViewer = (ScrollViewer)target;
            break;
        case 2:
            tableGrid = (Grid)target;
            break;
        case 3:
            horizontalLine = (Line)target;
            break;
        case 4:
            verticalLine = (Line)target;
            break;
        case 5:
            ((Thumb)target).DragDelta += OnDragDeltaResizeTable;
            break;
        default:
            _contentLoaded = true;
            break;
        }
    }
}
