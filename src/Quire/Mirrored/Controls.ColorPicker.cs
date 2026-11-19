using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Quire.Controls;

public partial class ColorPicker
{
    private Brush _lastUsed;
    internal global::System.Windows.Controls.ListBox ColorList;
    internal global::Fluent.Button NoColorCheckBox;
    internal global::Fluent.Button MoreColorsBtn;

    public Brush SelectedColor
    {
        get
        {
            return (Brush)GetValue(SelectedColorProperty);
        }
        set
        {
            SetValue(SelectedColorProperty, value);
        }
    }

    public Brush DefaultColor
    {
        get
        {
            return (Brush)GetValue(DefaultColorProperty);
        }
        set
        {
            SetValue(DefaultColorProperty, value);
        }
    }

    public bool HasNoColorOption
    {
        get
        {
            return (bool)GetValue(HasNoColorOptionProperty);
        }
        set
        {
            SetValue(HasNoColorOptionProperty, value);
        }
    }

    public ICommand Command { get; set; }

    public object CommandParameter
    {
        get
        {
            return GetValue(CommandParameterProperty);
        }
        set
        {
            SetValue(CommandParameterProperty, value);
        }
    }

    public Brush LastUsed
    {
        get
        {
            return _lastUsed;
        }
        private set
        {
            _lastUsed = value;
            FireProperyChanged("LastUsed");
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void HandleSelectedCustomColor(Color color)
    {
        LastUsed = new SolidColorBrush(color);
        ApplySelection();
    }

    private void OnColorListSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ApplySelectionLastUsed();
    }

    private void OnColorListItemPreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (object.Equals(((FrameworkElement)sender).DataContext, ColorList.SelectedItem))
        {
            ApplySelectionLastUsed();
        }
    }

    private void ApplySelectionLastUsed()
    {
        LastUsed = ColorList.SelectedItem as Brush;
        ApplySelection();
    }

    private void ApplySelection()
    {
        if (Command != null)
        {
            Command.Execute(CommandParameter ?? LastUsed);
        }
        else
        {
            SelectedColor = LastUsed;
        }
    }

    private static void OnDefaultColorChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
    {
        if (target is ColorPicker colorPicker)
        {
            colorPicker.LastUsed = e.NewValue as Brush;
        }
    }

    private void FireProperyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
