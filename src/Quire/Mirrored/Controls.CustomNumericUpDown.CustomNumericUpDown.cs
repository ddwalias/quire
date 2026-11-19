using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Quire.Controls.CustomNumericUpDown;

public partial class CustomNumericUpDown
{
    public const string Increase = "increase";
    public const string Decrease = "decrease";
    public const int DefaultNumberValue = -1;
    public RoutedUICommand PropertyChangingWrappingCommand;
    internal global::System.Windows.Controls.Label Label;
    internal global::System.Windows.Controls.Image LockImage;
    internal global::System.Windows.Controls.TextBox ValueTextBox;
    internal global::Fluent.DropDownButton UpButton;
    internal global::Fluent.DropDownButton DownButton;

    public global::Quire.Controls.Core.IPropertyAggregate TargetObject
    {
        get
        {
            return (global::Quire.Controls.Core.IPropertyAggregate)GetValue(TargetObjectProperty);
        }
        set
        {
            SetValue(TargetObjectProperty, value);
        }
    }

    public ICommand BaseCommand
    {
        get
        {
            return (ICommand)GetValue(BaseCommandProperty);
        }
        set
        {
            SetValue(BaseCommandProperty, value);
        }
    }

    public string PropertyName
    {
        get
        {
            return (string)GetValue(PropertyNameProperty);
        }
        set
        {
            SetValue(PropertyNameProperty, value);
        }
    }

    public int NumberValue
    {
        get
        {
            return (int)GetValue(NumberValueProperty);
        }
        set
        {
            SetValue(NumberValueProperty, value);
        }
    }

    public string LabelText
    {
        get
        {
            return (string)GetValue(LabelTextProperty);
        }
        set
        {
            SetValue(LabelTextProperty, value);
        }
    }

    public int MinimumValue
    {
        get
        {
            return (int)GetValue(MinimumValueProperty);
        }
        set
        {
            SetValue(MinimumValueProperty, value);
        }
    }

    public int MaximumValue
    {
        get
        {
            return (int)GetValue(MaximumValueProperty);
        }
        set
        {
            SetValue(MaximumValueProperty, value);
        }
    }

    public string ShownValue
    {
        get
        {
            return (string)GetValue(ShownValueProperty);
        }
        set
        {
            if (int.TryParse(value, out var result) && result < 0)
            {
                value = "";
            }

            SetValue(ShownValueProperty, value);
        }
    }

    public global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey FunctionalityKey
    {
        get
        {
            return (global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey)GetValue(FunctionalityKeyProperty);
        }
        set
        {
            SetValue(FunctionalityKeyProperty, value);
            PropertyChangingWrappingCommand = new global::Quire.Wpf.RoutedUICommandWithFunctionalityKey("Execute base command with extracted value", "PropertyChangingWrappingCommand", typeof(CustomNumericUpDown), FunctionalityKey);
            CommandBindings.Add(new CommandBinding(PropertyChangingWrappingCommand, ExecuteWrappingCommandHandler, CanExecuteWrappingCommandHandler));
        }
    }

    public DependencyObject CurrentElementOrigin
    {
        get
        {
            return (DependencyObject)GetValue(CurrentElementOriginProperty);
        }
        set
        {
            SetValue(CurrentElementOriginProperty, value);
        }
    }

    public bool IsFunctionalityEnabled => global::Quire.Services.FunctionalityService.Instance.IsFunctionalityEnabled(FunctionalityKey);

    private void CanExecuteWrappingCommandHandler(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = false;
    }

    private void OnLicenseTypeChanged(object sender, EventArgs e)
    {
        LockImage.GetBindingExpression(UIElement.VisibilityProperty)?.UpdateTarget();
        Label.GetBindingExpression(Control.FontStyleProperty)?.UpdateTarget();
        Label.GetBindingExpression(Control.ForegroundProperty)?.UpdateTarget();
        ValueTextBox.GetBindingExpression(Control.FontStyleProperty)?.UpdateTarget();
        ValueTextBox.GetBindingExpression(Control.ForegroundProperty)?.UpdateTarget();
    }

    private static void OnNumberValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ((CustomNumericUpDown)d).UpdateShownValue();
    }

    private static void OnFunctionalityKeyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        CustomNumericUpDown customNumericUpDown = (CustomNumericUpDown)d;
        customNumericUpDown.PropertyChangingWrappingCommand = new global::Quire.Wpf.RoutedUICommandWithFunctionalityKey("Execute base command with extracted value", "PropertyChangingWrappingCommand", typeof(CustomNumericUpDown), (global::Quire.FunctionalitiesLimitation.EnumFunctionalityKey)e.NewValue);
        switch (customNumericUpDown.PropertyName)
        {
            case "ExponentialThreshold":
                global::Quire.Controls.Core.NumberFormat.SetNumberFormatExponentialThresholdCommand = customNumericUpDown.PropertyChangingWrappingCommand;
                global::Quire.Controls.Core.NumberFormat.ExponentialThresholdControl = customNumericUpDown;
                break;
            case "ZeroThreshold":
                global::Quire.Controls.Core.NumberFormat.SetNumberFormatZeroThresholdCommand = customNumericUpDown.PropertyChangingWrappingCommand;
                global::Quire.Controls.Core.NumberFormat.ZeroThresholdControl = customNumericUpDown;
                break;
            case "ComplexThreshold":
                global::Quire.Controls.Core.NumberFormat.SetNumberFormatComplexThresholdCommand = customNumericUpDown.PropertyChangingWrappingCommand;
                global::Quire.Controls.Core.NumberFormat.ComplexThresholdControl = customNumericUpDown;
                break;
        }

        customNumericUpDown.CommandBindings.Add(new CommandBinding(customNumericUpDown.PropertyChangingWrappingCommand, customNumericUpDown.ExecuteWrappingCommandHandler));
    }

    private static void CurrentElementChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        CustomNumericUpDown customNumericUpDown = (CustomNumericUpDown)d;
        if (customNumericUpDown.IsPlotControls())
        {
            customNumericUpDown.SetIsEnabledToElements(isEnabled: false);
            if (customNumericUpDown.PropertyName.Equals("ComplexThreshold") || customNumericUpDown.PropertyName.Equals("ZeroThreshold") || customNumericUpDown.PropertyName.Equals("ExponentialThreshold"))
            {
                customNumericUpDown.ShownValue = customNumericUpDown.PropertyName.Equals("ExponentialThreshold") ? 3.ToString() : "";
            }
        }
        else
        {
            customNumericUpDown.SetIsEnabledToElements(customNumericUpDown.IsEnabled);
        }
    }

    private void UpdateShownValue()
    {
        if (!(CurrentElementOrigin is global::Quire.Controls.Plots.View.PlotControlBase))
        {
            ShownValue = NumberValue.ToString();
        }
    }

    public void DisableAndSetTemporaryValue(string value)
    {
    }

    private bool IsValidNumberValue(object value)
    {
        if (value is int num)
        {
            if (num >= MinimumValue)
            {
                return num <= MaximumValue;
            }

            return false;
        }

        return false;
    }

    private void UpButton_Click(object sender, RoutedEventArgs e)
    {
        if (!CheckIfFreemiumAndShowAboutBox())
        {
            CallWrappingCommand(NumberValue, Increase);
        }
    }

    private void DownButton_Click(object sender, RoutedEventArgs e)
    {
        if (!CheckIfFreemiumAndShowAboutBox())
        {
            CallWrappingCommand(NumberValue, Decrease);
        }
    }

    private void CallWrappingCommand(int dependencyPropVal, string userData = null)
    {
        if (IsFunctionalityEnabled)
        {
            global::Quire.Controls.Core.SetPropertyValueCommandParameter parameter = GenerateParameterObjectForWrappingCommand(dependencyPropVal, userData);
            if (PropertyChangingWrappingCommand.CanExecute(parameter, this))
            {
                PropertyChangingWrappingCommand.Execute(parameter, this);
            }
        }
    }

    private global::Quire.Controls.Core.SetPropertyValueCommandParameter GenerateParameterObjectForWrappingCommand(int dependencyPropVal, string userData = null)
    {
        return new global::Quire.Controls.Core.SetPropertyValueCommandParameter(TargetObject, PropertyName, dependencyPropVal, userData);
    }

    private void ExecuteBaseCommand(global::Quire.Controls.Core.SetPropertyValueCommandParameter commandParameters)
    {
        BaseCommand?.Execute(commandParameters);
    }

    private void CustomNumericUpDownPreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (sender is CustomNumericUpDown)
        {
            CheckIfFreemiumAndShowAboutBox();
        }
    }

    private bool CheckIfFreemiumAndShowAboutBox()
    {
        bool flag = false;
        _ = FunctionalityKey;
        if (!IsFunctionalityEnabled)
        {
            flag = true;
            global::Quire.AppCommands.DisplayFreemiumAboutBox.Execute(new global::Quire.FunctionalitiesLimitation.DisplayFreemiumAboutBoxCommandParameter(this, FunctionalityKey), null);
        }

        ValueTextBox.IsReadOnly = flag;
        return flag;
    }

    private void ValueTextBox_OnGotFocus(object sender, RoutedEventArgs e)
    {
        SelectAllTextInValueTextBox();
    }

    private void ValueTextBox_OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        SelectAllTextInValueTextBox();
    }

    private void CustomNumericUpDownOnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (sender is CustomNumericUpDown && e != null && ((int)e.Key == 6 || (int)e.Key == 6))
        {
            Keyboard.Focus(null);
            e.Handled = true;
        }
    }

    private void ValueTextBox_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        if (sender is TextBox element)
        {
            Keyboard.Focus(element);
        }
    }
}
