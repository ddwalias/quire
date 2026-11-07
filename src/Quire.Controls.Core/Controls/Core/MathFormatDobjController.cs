#nullable disable
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class MathFormatDobjController
{
    public MathFormatDobjController(DependencyObject owner)
    {
        Owner = owner;
    }

    [MirrorVerified("PtcControlsCoreMathFormatDobjControllerMechanicalDifferentialTests")]
    public DependencyObject Owner { get; private set; }

    public string FontFamilyName
    {
        get
        {
            return (string)Owner.GetValue(MathFormatAttachedProperties.FontFamilyNameProperty);
        }
        set
        {
            if (value == null)
            {
                Owner.ClearValue(MathFormatAttachedProperties.FontFamilyNameProperty);
            }
            else
            {
                Owner.SetValue(MathFormatAttachedProperties.FontFamilyNameProperty, value);
            }
        }
    }

    public double? FontSize
    {
        get
        {
            return (double?)Owner.GetValue(MathFormatAttachedProperties.FontSizeProperty);
        }
        set
        {
            if (!value.HasValue)
            {
                Owner.ClearValue(MathFormatAttachedProperties.FontSizeProperty);
            }
            else
            {
                Owner.SetValue(MathFormatAttachedProperties.FontSizeProperty, value);
            }
        }
    }

    public bool? IsBold
    {
        get
        {
            return (bool?)Owner.GetValue(MathFormatAttachedProperties.BoldProperty);
        }
        set
        {
            if (!value.HasValue)
            {
                Owner.ClearValue(MathFormatAttachedProperties.BoldProperty);
            }
            else
            {
                Owner.SetValue(MathFormatAttachedProperties.BoldProperty, value);
            }
        }
    }

    public bool? IsItalic
    {
        get
        {
            return (bool?)Owner.GetValue(MathFormatAttachedProperties.ItalicProperty);
        }
        set
        {
            if (!value.HasValue)
            {
                Owner.ClearValue(MathFormatAttachedProperties.ItalicProperty);
            }
            else
            {
                Owner.SetValue(MathFormatAttachedProperties.ItalicProperty, value);
            }
        }
    }

    public bool? IsUnderline
    {
        get
        {
            return (bool?)Owner.GetValue(MathFormatAttachedProperties.UnderlineProperty);
        }
        set
        {
            if (!value.HasValue)
            {
                Owner.ClearValue(MathFormatAttachedProperties.UnderlineProperty);
            }
            else
            {
                Owner.SetValue(MathFormatAttachedProperties.UnderlineProperty, value);
            }
        }
    }

    public Color? FontColor
    {
        get
        {
            return (Color?)Owner.GetValue(MathFormatAttachedProperties.FontColorProperty);
        }
        set
        {
            if (!value.HasValue)
            {
                Owner.ClearValue(MathFormatAttachedProperties.FontColorProperty);
            }
            else
            {
                Owner.SetValue(MathFormatAttachedProperties.FontColorProperty, value);
            }
        }
    }

    public Color? HighlightColor
    {
        get
        {
            return (Color?)Owner.GetValue(MathFormatAttachedProperties.HighlightColorProperty);
        }
        set
        {
            if (!value.HasValue)
            {
                Owner.ClearValue(MathFormatAttachedProperties.HighlightColorProperty);
            }
            else
            {
                Owner.SetValue(MathFormatAttachedProperties.HighlightColorProperty, value);
            }
        }
    }

    public void SetPropertiesFrom(IMathFormat mathFormat, bool overwriteLocalValues = true)
    {
        if (overwriteLocalValues || !IsPropertySet(MathFormatAttachedProperties.FontFamilyNameProperty))
        {
            FontFamilyName = mathFormat.FontFamilyName;
        }
        if (overwriteLocalValues || !IsPropertySet(MathFormatAttachedProperties.FontSizeProperty))
        {
            FontSize = mathFormat.FontSize;
        }
        if (overwriteLocalValues || !IsPropertySet(MathFormatAttachedProperties.ItalicProperty))
        {
            IsItalic = mathFormat.IsItalic;
        }
        if (overwriteLocalValues || !IsPropertySet(MathFormatAttachedProperties.BoldProperty))
        {
            IsBold = mathFormat.IsBold;
        }
        if (overwriteLocalValues || !IsPropertySet(MathFormatAttachedProperties.UnderlineProperty))
        {
            IsUnderline = mathFormat.IsUnderline;
        }
        if (overwriteLocalValues || !IsPropertySet(MathFormatAttachedProperties.FontColorProperty))
        {
            FontColor = mathFormat.FontColor;
        }
        if (overwriteLocalValues || !IsPropertySet(MathFormatAttachedProperties.HighlightColorProperty))
        {
            HighlightColor = mathFormat.HighlightColor;
        }
    }

    private bool IsPropertySet(DependencyProperty dependencyProperty)
    {
        return Owner.HasLocalValue(dependencyProperty);
    }
}
