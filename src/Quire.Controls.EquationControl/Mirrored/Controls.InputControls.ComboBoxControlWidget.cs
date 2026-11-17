using System.Windows;
using Quire.Controls.Core;
using Quire.Core;
using Quire.EquationEditor.View;

namespace Quire.Controls.InputControls;

public partial class ComboBoxControlWidget : VisualAdorner
{
    private readonly EditableComboBoxTableBorder _editableComboBoxTableBorder;

    private readonly ComboBoxControl _comboBox;

    public bool IsEditModeActive => _editableComboBoxTableBorder.IsEditModeActive;

    public IEditableComboBoxVisualTable EditableComboBoxVisualTable => _editableComboBoxTableBorder.EditableTable;

    public ComboBoxControlWidget(ComboBoxControl comboBox, EditableComboBoxTableCellsFontFormats cellsFontFormats, ExtendedFont operatorFormat)
        : base(comboBox, false)
    {
        _comboBox = comboBox;
        EditableComboBoxVisualTable editableComboBoxVisualTable = new EditableComboBoxVisualTable(comboBox.CreatedEditableViewModel(), cellsFontFormats, operatorFormat);
        editableComboBoxVisualTable.RequestCloseEditingMode += CloseEditingMode;
        editableComboBoxVisualTable.ContentChanged += OnEditableComboBoxContentChanged;
        _editableComboBoxTableBorder = new EditableComboBoxTableBorder(editableComboBoxVisualTable);
        AddVisualChild(_editableComboBoxTableBorder);
        IsHitTestVisible = true;
    }

    private void OnEditableComboBoxContentChanged()
    {
        _comboBox.OnEditableComboBoxContentChanged();
    }

    public void Refresh(EditableComboBoxTableCellsFontFormats cellsFontFormats, ExtendedFont operatorFormat)
    {
        _editableComboBoxTableBorder.Refresh(cellsFontFormats, operatorFormat);
    }

    public void OpenEditingMode()
    {
        DebugTools.DevAssert(_comboBox.IsFocused || _comboBox.IsKeyboardFocusWithin, null);
        Add();
        EnableOrDisableComboBox(false);
    }

    private void CloseEditingMode(bool shouldKeepFocus)
    {
        Remove();
        EnableOrDisableComboBox(true);
        if (shouldKeepFocus)
        {
            _comboBox.Focus();
        }
        _comboBox.EditingCompleted(shouldKeepFocus);
    }

    private void EnableOrDisableComboBox(bool shouldEnable)
    {
        _comboBox.IsEnabled = shouldEnable;
        _comboBox.IsHitTestVisible = shouldEnable;
    }

    protected override void Add()
    {
        if (_editableComboBoxTableBorder == null)
        {
            return;
        }

        base.Add();
    }

    protected override void AddToAdornerLayer()
    {
        FireAdornerAdded();
    }

    protected override Size MeasureOverride(Size constraint)
    {
        if (DebugTools.AssertNotNull(_editableComboBoxTableBorder, null).Failed)
        {
            return base.MeasureOverride(constraint);
        }
        _editableComboBoxTableBorder.Measure(constraint);
        return _editableComboBoxTableBorder.DesiredSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        if (DebugTools.AssertNotNull(_editableComboBoxTableBorder, null).Failed)
        {
            return base.ArrangeOverride(finalSize);
        }
        _editableComboBoxTableBorder.Arrange(new Rect(new Point(0.0, 0.0), _editableComboBoxTableBorder.DesiredSize));
        return _editableComboBoxTableBorder.RenderSize;
    }
}
