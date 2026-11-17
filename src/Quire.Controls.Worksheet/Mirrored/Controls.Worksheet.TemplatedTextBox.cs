using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Core;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls.Worksheet;

public partial class TemplatedTextBox
{
    public bool IsUnregisterWithCommand { get; set; }

    string ITemplatedTextBox.Text => Text;

    public virtual double VerticalAlignmentOffset
    {
        get
        {
            if (!_baseline.HasValue)
            {
                Typeface typeface = new Typeface(FontFamily, FontStyle, FontWeight, FontStretch);
                FormattedText formattedText = new FormattedText(Text, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight, typeface, FontSize, Foreground);
                _baseline = -(formattedText.Baseline + FontFamily.Baseline);
                FirePropertyChanged(PropertyNames.VerticalAlignmentOffsetPropertyName);
            }
            return _baseline.Value;
        }
    }

    public bool IsEmpty => string.IsNullOrEmpty(Text);

    public virtual string TemplateFormat
    {
        get
        {
            return _templateFormat;
        }
        set
        {
            _templateFormat = value;
            FirePropertyChanged("TemplateFormat");
            FireChanged();
        }
    }

    public TextFormat CurrentTextFormat
    {
        get
        {
            return _currentTextFormat;
        }
        set
        {
            if (_currentTextFormat != value)
            {
                if (_currentTextFormat != null)
                {
                    _currentTextFormat.FormatChanged -= HandleTextFormattingChanged;
                }
                if (value != null)
                {
                    value.FormatChanged += HandleTextFormattingChanged;
                }
                _currentTextFormat = value;
                _baseline = null;
                FirePropertyChanged("CurrentTextFormat");
            }
        }
    }

    private void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    protected override void OnGotFocus(RoutedEventArgs routedEventArgs)
    {
        base.OnGotFocus(routedEventArgs);
        OnGotFocusEvent(this);
    }

    private void FireChanged()
    {
        RaiseEvent(new RoutedEventArgs(ChangedEvent));
    }

    public Vector? NavigateIntoItem(Direction direction)
    {
        Activate(isActive: true);
        return null;
    }

    public void SimulateMouseClick(Point location, int numberOfClicks)
    {
        Activate(isActive: true);
    }

    private void SetTextFormatPropertiesAccordingTo(TextFormat textFormat)
    {
        FontFamily = textFormat.FontFamily;
        FontSize = textFormat.FontSize;
        Foreground = textFormat.Foreground;
        Background = textFormat.Background;
        FontWeight = textFormat.FontWeight;
        FontStyle = textFormat.FontStyle;
        TextDecorations = textFormat.TextDecorations;
    }
}
