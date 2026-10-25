#nullable disable
using System;
using System.Windows;
using System.Windows.Controls;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class BorderDragger : Control
{
    private static readonly Type _typeOfThis;

    static BorderDragger()
    {
        _typeOfThis = typeof(BorderDragger);
        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(_typeOfThis));
    }

    [MirrorVerified("PtcControlsCoreBorderDraggerMechanicalDifferentialTests")]
    public BorderDragger() { }
}
