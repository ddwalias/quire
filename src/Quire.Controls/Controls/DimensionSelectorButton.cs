#nullable disable
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Quire.Mirror;

namespace Quire.Controls;

public partial class DimensionSelectorButton
{
    public static readonly DependencyProperty IsSelectedProperty = DependencyProperty.RegisterAttached("IsSelected", typeof(bool), typeof(DimensionSelectorButton), new PropertyMetadata(false));

    public static readonly DependencyProperty TitleFormatProperty = DependencyProperty.Register("TitleFormat", typeof(string), typeof(DimensionSelectorButton), new PropertyMetadata(UpdateTitle));

    public static readonly DependencyProperty NoSelectionTitleProperty = DependencyProperty.Register("NoSelectionTitle", typeof(string), typeof(DimensionSelectorButton), new PropertyMetadata(UpdateTitle));

    public static readonly DependencyProperty RowDisplayTextProperty = DependencyProperty.Register("RowDisplayText", typeof(string), typeof(DimensionSelectorButton), new PropertyMetadata(UpdateTitle));

    public static readonly DependencyProperty ColumnDisplayTextProperty = DependencyProperty.Register("ColumnDisplayText", typeof(string), typeof(DimensionSelectorButton), new PropertyMetadata(UpdateTitle));

    private static readonly DependencyPropertyKey TitlePropertyKey = DependencyProperty.RegisterReadOnly("Title", typeof(string), typeof(DimensionSelectorButton), new PropertyMetadata());

    public static readonly DependencyProperty TitleProperty = TitlePropertyKey.DependencyProperty;

    private UniformGrid _child;

    public bool IsSelected { get => GetIsSelected(this); set => SetIsSelected(this, value); }

    public object Content => _child;

    public event MouseOverCellHandler MouseOverCell;

    public event Delegate0 Commit;

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        private set => SetValue(TitlePropertyKey, value);
    }

    public string TitleFormat
    {
        get => (string)GetValue(TitleFormatProperty);
        set => SetValue(TitleFormatProperty, value);
    }

    public string NoSelectionTitle
    {
        get => (string)GetValue(NoSelectionTitleProperty);
        set => SetValue(NoSelectionTitleProperty, value);
    }

    public string RowDisplayText
    {
        get => (string)GetValue(RowDisplayTextProperty);
        set => SetValue(RowDisplayTextProperty, value);
    }

    public string ColumnDisplayText
    {
        get => (string)GetValue(ColumnDisplayTextProperty);
        set => SetValue(ColumnDisplayTextProperty, value);
    }

    public int TotalRows
    {
        get => _child.Rows;
        set
        {
            _child.Rows = value;
            PopulateTiles();
        }
    }

    public int TotalColumns
    {
        get => _child.Columns;
        set
        {
            _child.Columns = value;
            PopulateTiles();
        }
    }

    private static void UpdateTitle(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        DimensionSelectorButton button = d as DimensionSelectorButton;
        if (button == null)
        {
            return;
        }
        if (button.TitleFormat == null || button.RowDisplayText == null || button.ColumnDisplayText == null)
        {
            button.Title = button.NoSelectionTitle;
            return;
        }
        try
        {
            button.Title = string.Format(button.TitleFormat, button.RowDisplayText, button.ColumnDisplayText);
        }
        catch
        {
            button.Title = button.NoSelectionTitle;
        }
    }

    public static bool GetIsSelected(DependencyObject obj) => (bool)obj.GetValue(IsSelectedProperty);

    public static void SetIsSelected(DependencyObject obj, bool value) => obj.SetValue(IsSelectedProperty, value);

    private void PopulateTiles()
    {
        foreach (UIElement child in _child.Children)
        {
            child.MouseEnter -= OnCellMouseEnter;
        }
        _child.Children.Clear();
        for (int i = 0; i < TotalColumns * TotalRows; i++)
        {
            DimensionCell cell = new DimensionCell();
            cell.MouseEnter += OnCellMouseEnter;
            _child.Children.Add(cell);
        }
    }

    private void OnCellMouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
    {
        if (MouseOverCell != null)
        {
            int index = _child.Children.IndexOf(sender as UIElement);
            MouseOverCell(TotalColumns, index);
        }
    }

    public void SetSelection(params int[] cellIndexes)
    {
        foreach (UIElement child in _child.Children)
        {
            SetIsSelected(child, false);
        }
        foreach (int index in cellIndexes)
        {
            SetIsSelected(_child.Children[index], true);
        }
    }

    private void FireCommitIfNeeded(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (!(bool)e.NewValue && Commit != null)
        {
            Commit.Invoke();
        }
    }

    private void FireCommit()
    {
        if (Commit != null)
        {
            Commit.Invoke();
        }
    }
}
