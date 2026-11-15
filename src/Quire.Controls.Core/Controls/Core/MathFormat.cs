#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Mirror;
using Quire.PersistentDataObjects;
using Quire.Undo;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class MathFormat
{
    public static readonly RoutedUICommand ChangeFontColorCommand = new RoutedUICommand("ChangeFontColor", "Change Font Color", typeof(MathFormat));

    public static readonly RoutedUICommand ChangeHighlightColorCommand = new RoutedUICommand("ChangeHighlightColor", "Change Highlight Color", typeof(MathFormat));

    public static readonly RoutedUICommand IncreaseFontSizeCommand = new RoutedUICommand("IncreaseFontSize", "Increase Font Size", typeof(MathFormat));

    public static readonly RoutedUICommand DecreaseFontSizeCommand = new RoutedUICommand("DecreaseFontSize", "Decrease Font Size", typeof(MathFormat));

    public static readonly RoutedUICommand ClearMathFormatCommand = new RoutedUICommand("ClearMathFormat", "Clear Math Format", typeof(MathFormat));

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    private readonly DependencyObject _owner;

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    private readonly IList<DependencyProperty> _readonlyProperties = new List<DependencyProperty>();

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    public MathFormat(DependencyObject owner)
    {
        _owner = owner;
        BindToPropertyChanged();
        if (_owner is IMathFormatOwnerWithInitialValues mathFormatOwner)
        {
            StaticMathFormat initialMathFormat = mathFormatOwner.InitialMathFormat;
            using (new CommandScope(new CompoundCommand()))
            {
                FontFamilyName = initialMathFormat.FontFamilyName;
                FontSize = initialMathFormat.FontSize;
                IsItalic = initialMathFormat.IsItalic;
                IsBold = initialMathFormat.IsBold;
                IsUnderline = initialMathFormat.IsUnderline;
                FontColor = initialMathFormat.FontColor;
                HighlightColor = initialMathFormat.HighlightColor;
            }
        }
    }

    public string FontFamilyName
    {
        get => (string)GetPropertyValue(MathFormatAttachedProperties.FontFamilyNameProperty);
        set => SetPropertyValue(MathFormatAttachedProperties.FontFamilyNameProperty, value);
    }

    public double? FontSize
    {
        get => (double?)GetPropertyValue(MathFormatAttachedProperties.FontSizeProperty);
        set => SetPropertyValue(MathFormatAttachedProperties.FontSizeProperty, value);
    }

    public bool? IsBold
    {
        get => (bool?)GetPropertyValue(MathFormatAttachedProperties.BoldProperty);
        set => SetPropertyValue(MathFormatAttachedProperties.BoldProperty, value);
    }

    public bool? IsItalic
    {
        get => (bool?)GetPropertyValue(MathFormatAttachedProperties.ItalicProperty);
        set => SetPropertyValue(MathFormatAttachedProperties.ItalicProperty, value);
    }

    public bool? IsUnderline
    {
        get => (bool?)GetPropertyValue(MathFormatAttachedProperties.UnderlineProperty);
        set => SetPropertyValue(MathFormatAttachedProperties.UnderlineProperty, value);
    }

    public Color? FontColor
    {
        get => (Color?)GetPropertyValue(MathFormatAttachedProperties.FontColorProperty);
        set => SetPropertyValue(MathFormatAttachedProperties.FontColorProperty, value);
    }

    public Color? HighlightColor
    {
        get => (Color?)GetPropertyValue(MathFormatAttachedProperties.HighlightColorProperty);
        set => SetPropertyValue(MathFormatAttachedProperties.HighlightColorProperty, value);
    }

    private void BindToPropertyChanged()
    {
        if (!(_owner is IMathFormatElementCollection mathFormatElementCollection))
        {
            if (_owner is IMathFormatOwner mathFormatOwner)
            {
                mathFormatOwner.PropertyChanged += HandleOwnerPropertyChanged;
            }
        }
        else
        {
            if (mathFormatElementCollection.MathFormatElements != null)
            {
                mathFormatElementCollection.MathFormatElements.Each((ICurrentMathFormatProperty item) =>
                {
                    MathFormat currentMathFormat = item.CurrentMathFormat;
                    if (currentMathFormat != null)
                    {
                        currentMathFormat.PropertyChanged += HandleOwnerPropertyChanged;
                    }
                });
            }
        }
    }

    private object GetPropertyValue(DependencyProperty dependencyProperty)
    {
        if (_owner is not IMathFormatElementCollection mathFormatElementCollection)
        {
            return GetValue(dependencyProperty);
        }
        IEnumerable<ICurrentMathFormatProperty> mathFormatElements = mathFormatElementCollection.MathFormatElements;
        if (mathFormatElements.IsNullOrEmpty())
        {
            return null;
        }
        ICurrentMathFormatProperty currentMathFormatProperty = mathFormatElements.FirstOrDefault(item => item.CurrentMathFormat != null);
        if (currentMathFormatProperty == null)
        {
            return null;
        }
        object candidateValue = currentMathFormatProperty.CurrentMathFormat.GetPropertyValue(dependencyProperty);
        if (candidateValue == null)
        {
            return null;
        }
        if (!mathFormatElements.Any(item => item.CurrentMathFormat != null && !candidateValue.Equals(item.CurrentMathFormat.GetPropertyValue(dependencyProperty))))
        {
            return candidateValue;
        }
        return null;
    }

    private bool SetPropertyValue(DependencyProperty dependencyProperty, object value)
    {
        Func<MathFormat, bool> act = mathFormat =>
        {
            if (mathFormat.IsPropertyReadonly(dependencyProperty))
            {
                return false;
            }
            bool valueUnset = !mathFormat.IsPropertySet(dependencyProperty);
            object oldValue = mathFormat.GetValue(dependencyProperty);
            if (valueUnset ? value == null : object.Equals(oldValue, value))
            {
                return false;
            }
            IMathFormatOwner mathFormatOwner = mathFormat._owner as IMathFormatOwner;
            bool mustNotifyOwner = false;
            DelegateFunction0<bool> doMethod = () =>
            {
                if (value == null)
                {
                    mathFormat.ClearValue(dependencyProperty);
                }
                else
                {
                    mathFormat.SetValue(dependencyProperty, value);
                }
                object newValue = mathFormat.GetValue(dependencyProperty);
                if (value != null && !valueUnset && (oldValue == null ? newValue == null : object.Equals(oldValue, newValue)))
                {
                    return false;
                }
                if ((oldValue == null ? newValue == null : object.Equals(oldValue, newValue)) && mathFormatOwner != null && MathFormatAttachedProperties.GetHandleChangesEnabled((DependencyObject)mathFormatOwner))
                {
                    mustNotifyOwner = true;
                    mathFormatOwner.HandleMathFormatChanged(new DependencyPropertyChangedEventArgs(dependencyProperty, null, newValue));
                }
                return true;
            };
            Delegate0 undoMethod = () =>
            {
                if (valueUnset)
                {
                    mathFormat.ClearValue(dependencyProperty);
                }
                else
                {
                    mathFormat.SetValue(dependencyProperty, oldValue);
                }
                if (mustNotifyOwner && mathFormatOwner != null)
                {
                    mathFormatOwner.HandleMathFormatChanged(new DependencyPropertyChangedEventArgs(dependencyProperty, null, oldValue));
                }
            };
            return CommandBuffer.Execute(new UndoableCommand("Set Math Format Property", doMethod, undoMethod));
        };
        return DoAction(act, "Set Math Format Property");
    }

    public bool IncreaseFontSize()
    {
        Func<MathFormat, bool> act = mathFormat =>
        {
            double num = (double)mathFormat.GetValue(MathFormatAttachedProperties.FontSizeProperty);
            return mathFormat.SetPropertyValue(MathFormatAttachedProperties.FontSizeProperty, num + 1.0);
        };
        return DoAction(act, "Increase Math Font Size");
    }

    public bool DecreaseFontSize()
    {
        Func<MathFormat, bool> act = mathFormat =>
        {
            double num = (double)mathFormat.GetValue(MathFormatAttachedProperties.FontSizeProperty);
            return mathFormat.SetPropertyValue(MathFormatAttachedProperties.FontSizeProperty, num - 1.0);
        };
        return DoAction(act, "Decrease Math Font Size");
    }

    public bool ChangeFontColor(Color fontColor) => SetPropertyValue(MathFormatAttachedProperties.FontColorProperty, fontColor);

    public bool ChangeHighlightColor(Color? highlightColor)
    {
        if (!highlightColor.HasValue)
        {
            highlightColor = Colors.Transparent;
        }
        return SetPropertyValue(MathFormatAttachedProperties.HighlightColorProperty, highlightColor);
    }

    public bool ClearFormat()
    {
        Func<MathFormat, bool> act = mathFormat =>
        {
            StaticMathFormat oldMathFormat = mathFormat.ToStaticMathFormat();
            IMathFormatOwnerWithInitialValues mathFormatOwnerWithInitialValue = mathFormat._owner as IMathFormatOwnerWithInitialValues;
            if (mathFormatOwnerWithInitialValue != null)
            {
                if (mathFormat.IsEquivalentTo(mathFormatOwnerWithInitialValue.InitialMathFormat))
                {
                    return false;
                }
            }
            else if (!mathFormat.AreAnyPropertiesSet)
            {
                return false;
            }
            DelegateFunction0<bool> doMethod = () =>
            {
                if (mathFormatOwnerWithInitialValue != null)
                {
                    mathFormat.RestoreInitialValues();
                }
                else
                {
                    mathFormat.ClearAllValues();
                }
                return true;
            };
            Delegate0 undoMethod = () =>
            {
                mathFormat.FromStaticMathFormat(oldMathFormat);
            };
            return CommandBuffer.Execute(new UndoableCommand("Clear Math Format", doMethod, undoMethod));
        };
        return DoAction(act, "Clear Math Format");
    }

    private bool DoAction(Func<MathFormat, bool> act, string commandName)
    {
        bool result = false;
        using (new CommandScope(commandName))
        {
            if (!(_owner is IMathFormatElementCollection mathFormatElementCollection))
            {
                result = act(this);
            }
            else if (mathFormatElementCollection.MathFormatElements != null)
            {
                result = mathFormatElementCollection.MathFormatElements.Aggregate(false, (prevResult, item) =>
                {
                    MathFormat currentMathFormat = item.CurrentMathFormat;
                    // Mathcad branches on the DoAction result with the prevResult return placed before the true return.
                    if (!(currentMathFormat != null && currentMathFormat.DoAction(act, commandName)))
                    {
                        return prevResult;
                    }
                    return true;
                });
            }
        }
        return result;
    }

    public bool IsEquivalentTo(IMathFormat mathFormat)
    {
        if (FontFamilyName == null ? mathFormat.FontFamilyName != null : !object.Equals(FontFamilyName, mathFormat.FontFamilyName))
        {
            return false;
        }
        if (!FontSize.HasValue ? mathFormat.FontSize.HasValue : !object.Equals(FontSize, mathFormat.FontSize))
        {
            return false;
        }
        if (!IsBold.HasValue ? mathFormat.IsBold.HasValue : !object.Equals(IsBold, mathFormat.IsBold))
        {
            return false;
        }
        if (!IsItalic.HasValue ? mathFormat.IsItalic.HasValue : !object.Equals(IsItalic, mathFormat.IsItalic))
        {
            return false;
        }
        if (!IsUnderline.HasValue ? mathFormat.IsUnderline.HasValue : !object.Equals(IsUnderline, mathFormat.IsUnderline))
        {
            return false;
        }
        if (!FontColor.HasValue ? mathFormat.FontColor.HasValue : !object.Equals(FontColor, mathFormat.FontColor))
        {
            return false;
        }
        if (!HighlightColor.HasValue ? mathFormat.HighlightColor.HasValue : !object.Equals(HighlightColor, mathFormat.HighlightColor))
        {
            return false;
        }
        return true;
    }

    private object ReadLocalValue(DependencyProperty dependencyProperty)
    {
        object value = _owner.ReadLocalValue(dependencyProperty);
        if (value != DependencyProperty.UnsetValue)
        {
            return value;
        }
        return null;
    }

    private object GetValue(DependencyProperty dependencyProperty)
    {
        object value = _owner.GetValue(dependencyProperty);
        if (value != DependencyProperty.UnsetValue)
        {
            return value;
        }
        return null;
    }

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    private void SetValue(DependencyProperty dependencyProperty, object value) => _owner.SetValue(dependencyProperty, value);

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    private void ClearValue(DependencyProperty dependencyProperty) => _owner.ClearValue(dependencyProperty);

    private void ClearAllValues()
    {
        FontFamilyName = null;
        FontSize = null;
        IsBold = null;
        IsItalic = null;
        IsUnderline = null;
        FontColor = null;
        HighlightColor = null;
    }

    public void RestoreInitialValues()
    {
        if (_owner is not IMathFormatOwnerWithInitialValues mathFormatOwner)
        {
            return;
        }
        StaticMathFormat initialMathFormat = mathFormatOwner.InitialMathFormat;
        FontFamilyName = initialMathFormat.FontFamilyName;
        FontSize = initialMathFormat.FontSize;
        IsItalic = initialMathFormat.IsItalic;
        IsBold = initialMathFormat.IsBold;
        IsUnderline = initialMathFormat.IsUnderline;
        FontColor = initialMathFormat.FontColor;
        HighlightColor = initialMathFormat.HighlightColor;
    }

    private bool IsPropertySet(DependencyProperty dependencyProperty)
    {
        if (!(_owner is IMathFormatElementCollection mathFormatElementCollection))
        {
            return WpfUtils.HasLocalValue(_owner, dependencyProperty);
        }
        return mathFormatElementCollection.MathFormatElements.Any(item => item.CurrentMathFormat != null && item.CurrentMathFormat.IsPropertySet(dependencyProperty));
    }

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    public bool IsFontFamilyNameSet => IsPropertySet(MathFormatAttachedProperties.FontFamilyNameProperty);

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    public bool IsFontSizeSet => IsPropertySet(MathFormatAttachedProperties.FontSizeProperty);

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    public bool IsItalicSet => IsPropertySet(MathFormatAttachedProperties.ItalicProperty);

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    public bool IsBoldSet => IsPropertySet(MathFormatAttachedProperties.BoldProperty);

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    public bool IsUnderlineSet => IsPropertySet(MathFormatAttachedProperties.UnderlineProperty);

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    public bool IsFontColorSet => IsPropertySet(MathFormatAttachedProperties.FontColorProperty);

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    public bool IsHighlightColorSet => IsPropertySet(MathFormatAttachedProperties.HighlightColorProperty);

    public bool AreAnyPropertiesSet
    {
        get
        {
            if (!IsFontFamilyNameSet && !IsFontSizeSet && !IsItalicSet && !IsBoldSet && !IsUnderlineSet && !IsFontColorSet)
            {
                return IsHighlightColorSet;
            }
            return true;
        }
    }

    public void CopyTo(IMathFormat target, bool copyInheritedProperties = false)
    {
        using (new CommandScope(new CompoundCommand()))
        {
            if (copyInheritedProperties || IsFontFamilyNameSet)
            {
                target.FontFamilyName = FontFamilyName;
            }
            if (copyInheritedProperties || IsFontSizeSet)
            {
                target.FontSize = FontSize;
            }
            if (copyInheritedProperties || IsItalicSet)
            {
                target.IsItalic = IsItalic;
            }
            if (copyInheritedProperties || IsBoldSet)
            {
                target.IsBold = IsBold;
            }
            if (copyInheritedProperties || IsUnderlineSet)
            {
                target.IsUnderline = IsUnderline;
            }
            if (copyInheritedProperties || IsFontColorSet)
            {
                target.FontColor = FontColor;
            }
            if (copyInheritedProperties || IsHighlightColorSet)
            {
                target.HighlightColor = HighlightColor;
            }
        }
    }

    public StaticMathFormat ToStaticMathFormat()
    {
        return new StaticMathFormat(
            IsFontFamilyNameSet ? FontFamilyName : null,
            IsFontSizeSet ? FontSize : (double?)null,
            IsItalicSet ? IsItalic : (bool?)null,
            IsBoldSet ? IsBold : (bool?)null,
            IsUnderlineSet ? IsUnderline : (bool?)null,
            IsFontColorSet ? FontColor : (Color?)null,
            IsHighlightColorSet ? HighlightColor : (Color?)null);
    }

    public void FromStaticMathFormat(StaticMathFormat mathFormat)
    {
        using (new CommandScope(new CompoundCommand()))
        {
            if (mathFormat.FontFamilyName != null)
            {
                FontFamilyName = mathFormat.FontFamilyName;
            }
            if (mathFormat.FontSize.HasValue)
            {
                FontSize = mathFormat.FontSize.Value;
            }
            if (mathFormat.IsItalic.HasValue)
            {
                IsItalic = mathFormat.IsItalic.Value;
            }
            if (mathFormat.IsBold.HasValue)
            {
                IsBold = mathFormat.IsBold.Value;
            }
            if (mathFormat.IsUnderline.HasValue)
            {
                IsUnderline = mathFormat.IsUnderline.Value;
            }
            if (mathFormat.FontColor.HasValue)
            {
                FontColor = mathFormat.FontColor;
            }
            if (mathFormat.HighlightColor.HasValue)
            {
                HighlightColor = mathFormat.HighlightColor;
            }
        }
    }

    private void HandleOwnerPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, e);
        }
    }

    [MirrorVerified("PtcControlsCoreMathFormatMechanicalDifferentialTests")]
    private bool IsPropertyReadonly(DependencyProperty dependencyProperty) => _readonlyProperties.Contains(dependencyProperty);

    public void SetReadonlyProperty(DependencyProperty dependencyProperty, object val)
    {
        SetValue(dependencyProperty, val);
        _readonlyProperties.Add(dependencyProperty);
    }

    public mathFormattingType GetDataObject(bool copyInheritedProperties = false)
    {
        return new mathFormattingType
        {
            FontFamily = (copyInheritedProperties || IsFontFamilyNameSet) ? FontFamilyName : null,
            FontSize = FontSize.GetValueOrDefault(),
            FontSizeSpecified = copyInheritedProperties || (IsFontSizeSet && FontSize.HasValue),
            BoldSpecified = copyInheritedProperties || (IsBoldSet && IsBold.HasValue),
            Bold = IsBold == true,
            ItalicSpecified = copyInheritedProperties || (IsItalicSet && IsItalic.HasValue),
            Italic = IsItalic == true,
            UnderlineSpecified = copyInheritedProperties || (IsUnderlineSet && IsUnderline.HasValue),
            Underline = IsUnderline == true,
            ForegroundColor = ((copyInheritedProperties || IsFontColorSet) && FontColor.HasValue) ? FontColor.Value.ToString(CultureInfo.InvariantCulture) : null,
            BackgroundColor = ((copyInheritedProperties || IsHighlightColorSet) && HighlightColor.HasValue) ? HighlightColor.Value.ToString(CultureInfo.InvariantCulture) : null
        };
    }

    public void SetDataObject(mathFormattingType formattingOverride)
    {
        if (formattingOverride == null)
        {
            return;
        }
        if (formattingOverride.FontFamily != null)
        {
            FontFamilyName = formattingOverride.FontFamily;
        }
        if (formattingOverride.FontSizeSpecified)
        {
            FontSize = formattingOverride.FontSize;
        }
        if (formattingOverride.BoldSpecified)
        {
            IsBold = formattingOverride.Bold;
        }
        if (formattingOverride.UnderlineSpecified)
        {
            IsUnderline = formattingOverride.Underline;
        }
        if (formattingOverride.Italic)
        {
            IsItalic = formattingOverride.Italic;
        }
        if (formattingOverride.ForegroundColor != null)
        {
            FontColor = WpfUtils.WpfColorFromName(formattingOverride.ForegroundColor);
        }
        if (formattingOverride.BackgroundColor != null)
        {
            HighlightColor = WpfUtils.WpfColorFromName(formattingOverride.BackgroundColor);
        }
    }
}
