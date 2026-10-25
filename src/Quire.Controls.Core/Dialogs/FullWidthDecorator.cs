#nullable disable
using System;
using System.Windows;
using Quire;
using Quire.Mirror;

namespace Quire.Controls.Core.Dialogs;

public partial class FullWidthDecorator
{
    [MirrorVerified("PtcControlsCoreDialogsFullWidthDecoratorMechanicalDifferentialTests")]
    public FullWidthDecorator() { }

    protected override Size MeasureOverride(Size constraint)
    {
        Size result = base.MeasureOverride(constraint);
        result = new Size(constraint.Width.IsValid() ? Math.Max(constraint.Width, result.Width) : result.Width, result.Height);
        return result;
    }
}
