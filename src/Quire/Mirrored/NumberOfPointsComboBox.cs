#nullable disable

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Quire.Controls.Plots.View;

namespace Quire;

public partial class NumberOfPointsComboBox
{
    public int SelectedTracesNumOfPoints
    {
        get => (int)GetValue(SelectedTracesNumOfPointsProperty);
        set => SetValue(SelectedTracesNumOfPointsProperty, value);
    }

    string IDataErrorInfo.this[string columnName]
    {
        get
        {
            if (columnName == FrameworkElement.TagProperty.Name)
            {
                if (Tag == null)
                {
                    return null;
                }

                if (!int.TryParse(Tag.ToString(), out int result))
                {
                    result = -1;
                }

                if (SelectedTracesNumOfPoints == result || (result >= _minimumValidValue && result <= _maximumValidValue))
                {
                    return null;
                }

                return columnName;
            }

            return null;
        }
    }

    string IDataErrorInfo.Error => null;

    public override void EndInit()
    {
        base.EndInit();
        PreviewKeyDown += NumberOfPointsComboBoxKeyDown;
        SelectionChanged += NumberOfPointsComboBoxSelectionChanged;
        PreviewLostKeyboardFocus += NumberOfPointsComboBoxPreviewLostFocus;
        DropDownOpened += NumberOfPointsComboBoxDropDownOpened;
        DropDownClosed += NumberOfPointsComboBoxDropDownClosed;
        Binding binding = new Binding
        {
            Source = this,
            Path = new PropertyPath(FrameworkElement.TagProperty),
            Mode = BindingMode.TwoWay,
            ValidatesOnDataErrors = true,
            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        };
        SetBinding(ComboBox.TextProperty, binding);
    }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        if (Template.FindName("PART_EditableTextBox", this) is TextBox textBox)
        {
            textBox.MaxLength = 4;
        }
    }

    private static void HandleSelectedTracesNumOfPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is NumberOfPointsComboBox numberOfPointsComboBox)
        {
            numberOfPointsComboBox.Tag = ConvertIntToStringForTag((int)e.NewValue);
        }
    }

    private static void ChangeOrRestoreNumberOfPoints(NumberOfPointsComboBox numberOfPointsComboBox, bool activate = true)
    {
        if (((IDataErrorInfo)numberOfPointsComboBox)[FrameworkElement.TagProperty.Name] == null)
        {
            if (int.TryParse(numberOfPointsComboBox.Tag.ToString(), out int result) && result != numberOfPointsComboBox.SelectedTracesNumOfPoints)
            {
                PlotControlBase.ChangeNumOfPointsCommand.Execute(new Pair<int, bool>(result, activate), null);
            }
        }
        else
        {
            numberOfPointsComboBox.Tag = ConvertIntToStringForTag(numberOfPointsComboBox.SelectedTracesNumOfPoints);
        }
    }

    protected static void NumberOfPointsComboBoxKeyDown(object sender, KeyEventArgs args)
    {
        if (sender is NumberOfPointsComboBox numberOfPointsComboBox && args != null && (int)args.Key == 6)
        {
            ChangeOrRestoreNumberOfPoints(numberOfPointsComboBox);
            args.Handled = true;
        }
    }

    private static void NumberOfPointsComboBoxPreviewLostFocus(object sender, RoutedEventArgs args)
    {
        if (sender is NumberOfPointsComboBox numberOfPointsComboBox)
        {
            ChangeOrRestoreNumberOfPoints(numberOfPointsComboBox, activate: false);
        }
    }

    private static void NumberOfPointsComboBoxSelectionChanged(object sender, SelectionChangedEventArgs args)
    {
        if (sender is NumberOfPointsComboBox numberOfPointsComboBox && _handleSelection && args != null && args.AddedItems != null && args.AddedItems.Count > 0)
        {
            numberOfPointsComboBox.Tag = args.AddedItems[0].ToString();
            ChangeOrRestoreNumberOfPoints(numberOfPointsComboBox);
            args.Handled = true;
        }
    }

    private static void NumberOfPointsComboBoxDropDownOpened(object sender, EventArgs e)
    {
        _handleSelection = true;
    }

    private static void NumberOfPointsComboBoxDropDownClosed(object sender, EventArgs e)
    {
        _handleSelection = false;
    }

    private static string ConvertIntToStringForTag(int intValue)
    {
        if (intValue != -1)
        {
            return intValue.ToString();
        }

        return string.Empty;
    }

    protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
    {
        base.OnItemsSourceChanged(oldValue, newValue);
        if (ItemsSource != null)
        {
            IEnumerable<string> source = ItemsSource.Cast<string>();
            _minimumValidValue = int.Parse(source.ElementAt(0));
            _maximumValidValue = int.Parse(source.ElementAt(source.Count() - 1));
        }
    }
}
