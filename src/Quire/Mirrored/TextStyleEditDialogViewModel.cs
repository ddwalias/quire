#nullable disable

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire;

public partial class TextStyleEditDialogViewModel
{
    private bool _okIsEnabled = true;

    public string TextStyleBasedOnKeyNoneLocalized
    {
        get
        {
            return StylesCollection[0].StyleName;
        }
        set
        {
            StylesCollection[0].StyleName = value;
        }
    }

    public string TextStyleNameNewLocalized
    {
        get
        {
            return _textStyle.Name;
        }
        set
        {
            if (Action == StyleActionTypeEnum.New)
            {
                _textStyle.Name = value;
            }
        }
    }

    public string TextStyleNames { get; set; }

    public RichTextBox PreviewBox { get; set; }

    public TextStyleEditDialogViewModel(ITextStyle textStyle, StyleActionTypeEnum action, ITextStylesManager textStyleManager)
    {
        _textStyle = textStyle;
        _bBasedOn = new TextStyleData("(none)", "(none)", new Style());
        _textStyleManager = textStyleManager;
        Action = action;
        Header = action.ToString() + ": " + _textStyle.Name;
        StylesCollection = new List<TextStyleData>();
        StylesCollection.Add(_bBasedOn);
        IsNameReadOnly = !TextStylesDictionary.Instance.Contains(textStyle.StyleKey);
        IList<string> values = textStyleManager.StylesCollection.CollectAsList((TextStyleData textStyleData) => textStyleData.StyleName);
        string text = "," + string.Join(",", values) + ",";
        if (action == StyleActionTypeEnum.Edit)
        {
            text = text.Replace(_textStyle.Name + ",", string.Empty);
        }
        else if (action == StyleActionTypeEnum.New)
        {
            textStyle.UpdateBasedOn(new TextStyle(textStyleManager.GetTextStyleDataByKey("Normal")), null, isFixInHeritage: false);
        }

        TextStyleNames = text;
        foreach (TextStyleData item in textStyleManager.StylesCollection)
        {
            if (_textStyle.StyleKey != item.StyleKey && !IsKeyBasedOnKey(_textStyle.StyleKey, item, textStyleManager))
            {
                StylesCollection.Add(item);
            }
        }

        if (_textStyle.BasedOnKey != null)
        {
            _bBasedOn = textStyleManager.GetTextStyleDataByKey(_textStyle.BasedOnKey);
        }
    }

    private object ResetValue(object value, string propertyName)
    {
        object basedOnSetterValue = TextStyleBase.GetBasedOnSetterValue(_textStyle.Style, propertyName);
        if ((basedOnSetterValue is TextDecorationCollection textDecorations && textDecorations.Count == ((TextDecorationCollection)value)?.Count) || (basedOnSetterValue is Brush brush && ((object)brush).ToString() == value?.ToString()) || (basedOnSetterValue is double value2 && value is double points && Math.Round(value2, 3).Equals(Math.Round(WpfUtils.PointsToDips(points), 3))) || (basedOnSetterValue != null && basedOnSetterValue.Equals(value)))
        {
            return null;
        }

        return value;
    }

    private bool IsKeyBasedOnKey(string styleKey, TextStyleData textStyleData, ITextStylesManager textStyleManager)
    {
        if (textStyleData?.BaseOnKey == styleKey)
        {
            return true;
        }

        if (textStyleData?.BaseOnKey != null)
        {
            TextStyleData textStyleDataByKey = textStyleManager.GetTextStyleDataByKey(textStyleData.BaseOnKey);
            return IsKeyBasedOnKey(styleKey, textStyleDataByKey, textStyleManager);
        }

        return false;
    }

    public bool IsNameReadOnly { get; }

    public bool OkIsEnabled
    {
        get
        {
            return _okIsEnabled;
        }
        set
        {
            _okIsEnabled = value;
            OnPropertyChanged("OkIsEnabled");
        }
    }

    public string BaseOnKey => _textStyle.BasedOnKey;

    public TextStyleData BasedOn
    {
        get
        {
            return _bBasedOn;
        }
        set
        {
            if (value.StyleName == TextStyleBasedOnKeyNoneLocalized)
            {
                _textStyle.MergeBasedOn();
            }
            else
            {
                _textStyle.UpdateBasedOn(new TextStyle(value), _textStyleManager.StylesCollection);
            }

            _bBasedOn = value;
            OnPropertyChanged("TextAlignment");
            OnPropertyChanged("Margin");
            OnPropertyChanged("TextIndent");
            OnPropertyChanged("TextDecorations");
            OnPropertyChanged("MarkerStyle");
            OnPropertyChanged("StyleSetters");
            OnPropertyChanged("FontWeight");
            OnPropertyChanged("FontStyle");
            OnPropertyChanged("FontFamily");
            OnPropertyChanged("FontSize");
            OnPropertyChanged("ForegroundColor");
            OnPropertyChanged("BackgroundColor");
            OnPropertyChanged("Style");
            UpdateMarkerStyle();
        }
    }

    public Style Style => _textStyle.Style;

    public Thickness Margin
    {
        get
        {
            if (_textStyle.TextIndent.HasValue)
            {
                return new Thickness((double)_textStyle.TextIndent.Value, 0.0, 0.0, 0.0);
            }

            if (_textStyle.Margin.HasValue && !_textStyle.Margin.Equals(default(Thickness)))
            {
                if (_textStyle.Margin.Value.Left > 0.0 && (!_textStyle.TextIndent.HasValue || _textStyle.TextIndent == 0.0))
                {
                    TextIndent = (int)_textStyle.Margin.Value.Left;
                }

                return _textStyle.Margin.Value;
            }

            return new Thickness(0.0, 0.0, 0.0, 0.0);
        }
    }

    public IList<TextStyleData> StylesCollection { get; }

    public Dictionary<string, TextStylePropertyInfoExt> StyleSetters => _textStyle.GetSetters(_textStyle.Style, new Dictionary<string, TextStylePropertyInfoExt>());

    public string StyleName
    {
        get
        {
            return _textStyle.Name;
        }
        set
        {
            _textStyle.Name = value;
        }
    }

    public FontWeight? FontWeight
    {
        get
        {
            return _textStyle.FontWeight;
        }
        set
        {
            _textStyle.FontWeight = (FontWeight?)ResetValue(value, TextElement.FontWeightProperty.Name);
            OnPropertyChanged("FontWeight");
            OnPropertyChanged("Style");
            OnPropertyChanged("StyleSetters");
        }
    }

    public FontFamily FontFamily
    {
        get => throw global::Quire.Mirror.MirrorStub.NotImplemented();
        set => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    }

    public SolidColorBrush BackgroundColor
    {
        get
        {
            return (SolidColorBrush)_textStyle.Background;
        }
        set
        {
            _textStyle.Background = (SolidColorBrush)ResetValue(value, TextElement.BackgroundProperty.Name);
            OnPropertyChanged("BackgroundColor");
            OnPropertyChanged("Style");
            OnPropertyChanged("StyleSetters");
        }
    }

    public SolidColorBrush ForegroundColor
    {
        get
        {
            return (SolidColorBrush)_textStyle.Foreground;
        }
        set
        {
            _textStyle.Foreground = (SolidColorBrush)ResetValue(value, TextElement.ForegroundProperty.Name);
            OnPropertyChanged("ForegroundColor");
            OnPropertyChanged("Style");
            OnPropertyChanged("StyleSetters");
        }
    }

    public TextEffectCollection TextEffects => _textStyle.TextEffects;

    public void UpdateMarkerStyle() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public double? TextIndent
    {
        get
        {
            return _textStyle.TextIndent;
        }
        set
        {
            if (!value.HasValue)
            {
                value = 0.0;
            }

            _textStyle.TextIndent = (double?)ResetValue(value, Paragraph.TextIndentProperty.Name);
            if (_textStyle.Margin.HasValue && _textStyle.TextIndent.HasValue && _textStyle.TextIndent != _textStyle.Margin.Value.Left)
            {
                _textStyle.Margin = new Thickness((double)_textStyle.TextIndent.Value, _textStyle.Margin.Value.Top, _textStyle.Margin.Value.Right, _textStyle.Margin.Value.Bottom);
            }

            OnPropertyChanged("TextIndent");
            OnPropertyChanged("Margin");
            OnPropertyChanged("Style");
            OnPropertyChanged("StyleSetters");
        }
    }

    public TextAlignment? TextAlignment
    {
        get
        {
            return _textStyle.TextAlignment;
        }
        set
        {
            if (!value.HasValue)
            {
                value = System.Windows.TextAlignment.Left;
            }

            if (_textStyle.TextAlignment != value)
            {
                _textStyle.TextAlignment = (TextAlignment?)ResetValue(value, Block.TextAlignmentProperty.Name);
            }

            OnPropertyChanged("TextAlignment");
            OnPropertyChanged("Style");
            OnPropertyChanged("StyleSetters");
        }
    }

    public BaselineAlignment? BaselineAlignment => _textStyle.BaselineAlignment;

    public TextDecorationCollection TextDecorations
    {
        get
        {
            return _textStyle.TextDecorations;
        }
        set
        {
            _textStyle.TextDecorations = (TextDecorationCollection)ResetValue(value, TextBlock.TextDecorationsProperty.Name);
            OnPropertyChanged("TextDecorations");
            OnPropertyChanged("Style");
            OnPropertyChanged("StyleSetters");
        }
    }

    public double? FontSize
    {
        get
        {
            if (!_textStyle.FontSize.HasValue)
            {
                return null;
            }

            return Math.Round(WpfUtils.DipsToPoints((double)_textStyle.FontSize.Value), 1);
        }
        set
        {
            value = (double?)ResetValue(value, TextElement.FontSizeProperty.Name);
            _textStyle.FontSize = (value.HasValue ? new double?(WpfUtils.PointsToDips((double)value.Value)) : value);
            OnPropertyChanged("FontSize");
            OnPropertyChanged("Style");
            OnPropertyChanged("StyleSetters");
        }
    }

    public FontStyle? FontStyle
    {
        get
        {
            return _textStyle.FontStyle;
        }
        set
        {
            _textStyle.FontStyle = (FontStyle?)ResetValue(value, TextElement.FontStyleProperty.Name);
            OnPropertyChanged("FontStyle");
            OnPropertyChanged("Style");
            OnPropertyChanged("StyleSetters");
        }
    }

    public TextMarkerStyle? MarkerStyle
    {
        get
        {
            return _textStyle.MarkerStyle;
        }
        set
        {
            object basedOnSetterValue = TextStyleBase.GetBasedOnSetterValue(_textStyle.Style, List.MarkerStyleProperty.Name);
            if (!value.HasValue)
            {
                value = TextMarkerStyle.None;
            }

            if (basedOnSetterValue != null && basedOnSetterValue.Equals(value))
            {
                value = null;
            }

            _textStyle.MarkerStyle = value;
            OnPropertyChanged("MarkerStyle");
            OnPropertyChanged("StyleSetters");
        }
    }

    public IWorksheetProperties Worksheet { get; }

    public StyleActionTypeEnum Action { get; set; }

    public string Header { get; set; }
}
