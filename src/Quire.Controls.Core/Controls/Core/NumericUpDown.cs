#nullable disable
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class NumericUpDown
{
    protected static readonly DependencyProperty NumberValueProperty = DependencyProperty.Register("NumberValue", typeof(int), typeof(NumericUpDown), new PropertyMetadata(-1, OnNumberChanged));

    private static readonly DependencyProperty IsInputValidProperty = DependencyProperty.Register("IsInputValid", typeof(bool), typeof(NumericUpDown), new PropertyMetadata(true));

    private static readonly DependencyProperty ValidationMessageProperty = DependencyProperty.Register("ValidationMessage", typeof(string), typeof(NumericUpDown), new PropertyMetadata(""));

    internal TextBox ValueTextBox;

    internal TextBlock WaterMarkTextBlock;

    internal Button UpButton;

    internal Button DownButton;

    private bool _contentLoaded;

    [MirrorVerified("PtcControlsCoreNumericUpDownMechanicalDifferentialTests")]
    private void UpButton_Click(object sender, RoutedEventArgs e) => IncrementValue();

    [MirrorVerified("PtcControlsCoreNumericUpDownMechanicalDifferentialTests")]
    private void DownButton_Click(object sender, RoutedEventArgs e) => DecrementValue();

    [MirrorVerified("PtcControlsCoreNumericUpDownMechanicalDifferentialTests")]
    public Action<int> NumberChangedAction { get; set; }

    public virtual int NumberValue
    {
        get => (int)GetValue(NumberValueProperty);
        set => SetValue(NumberValueProperty, value);
    }

    public bool IsInputValid
    {
        get => (bool)GetValue(IsInputValidProperty);
        set => SetValue(IsInputValidProperty, value);
    }

    [MirrorVerified("PtcControlsCoreNumericUpDownMechanicalDifferentialTests")]
    public int Minimum { get; set; } = int.MinValue;

    [MirrorVerified("PtcControlsCoreNumericUpDownMechanicalDifferentialTests")]
    public int Maximum { get; set; } = int.MaxValue;

    public string DefaultValidationMessage => $"{(string)TryFindResource("NumericRangeValidationStart")} {Minimum} {(string)TryFindResource("NumericRangeValidationEnd")} {Maximum}";

    public string ValidationMessage
    {
        get => (string)GetValue(ValidationMessageProperty);
        set => SetValue(ValidationMessageProperty, value);
    }

    public NumericUpDown()
    {
        InitializeComponent();
    }

    protected override void OnInitialized(EventArgs e)
    {
        base.OnInitialized(e);
        if (string.IsNullOrEmpty(ValidationMessage))
        {
            ValidationMessage = DefaultValidationMessage;
        }
    }

    private void ValueTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        IsInputValid = int.TryParse(ValueTextBox.Text, out var result) && result >= Minimum && result <= Maximum;
        if (IsInputValid)
        {
            NumberValue = result;
        }
        else
        {
            ValidationMessage = DefaultValidationMessage;
        }
    }

    private void ValueTextBox_KeyUp(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Up)
        {
            IncrementValue();
        }
        else if (e.Key == Key.Down)
        {
            DecrementValue();
        }
    }

    private void IncrementValue()
    {
        if (NumberValue < Maximum)
        {
            NumberValue++;
        }
    }

    private void DecrementValue()
    {
        if (NumberValue > Minimum)
        {
            NumberValue--;
        }
    }

    protected static void OnNumberChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        NumericUpDown numericUpDown = (NumericUpDown)d;
        numericUpDown.ShowValue();
        numericUpDown.NumberChangedAction?.Invoke(numericUpDown.NumberValue);
    }

    private void ShowValue()
    {
        ValueTextBox.Text = NumberValue.ToString();
    }

    public bool IsTextBoxEmpty()
    {
        return string.IsNullOrEmpty(ValueTextBox.Text);
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Core;component/numericupdown.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                ValueTextBox = (TextBox)target;
                ValueTextBox.TextChanged += ValueTextBox_TextChanged;
                ValueTextBox.KeyUp += ValueTextBox_KeyUp;
                break;
            case 2:
                WaterMarkTextBlock = (TextBlock)target;
                break;
            case 3:
                UpButton = (Button)target;
                UpButton.Click += UpButton_Click;
                break;
            case 4:
                DownButton = (Button)target;
                DownButton.Click += DownButton_Click;
                break;
            default:
                _contentLoaded = true;
                break;
        }
    }
}
