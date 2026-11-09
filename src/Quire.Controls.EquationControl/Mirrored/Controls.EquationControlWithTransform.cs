using System;
using System.Windows;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Core.AttachedBehavior;

namespace Quire.Controls;

public partial class EquationControlWithTransform
{
    private const double InnerMargin = 4d;

    private double _desiredNumericHeight;
    private HorizontalAlignment _hAlignment;
    private VerticalAlignment _vAlignment;
    private ScaleTransform _scaleTransform = new ScaleTransform();
    private readonly TranslateTransform _translateTransform = new TranslateTransform();
    private EquationControl _control;

    public EquationControlWithTransform()
        : this(null, 0.0)
    {
    }

    public EquationControlWithTransform(EquationControl control)
        : this(control, 0.0)
    {
    }

    public EquationControlWithTransform(double desiredNumericHeight)
        : this(new EquationControl(), desiredNumericHeight)
    {
    }

    public EquationControlWithTransform(EquationControl control, double desiredNumericHeight)
    {
        DesiredNumericHeight = desiredNumericHeight;
        HorizontalAlignment = HorizontalAlignment.Center;
        VerticalAlignment = VerticalAlignment.Center;
        control.IsMathFormatHandledLocally = false;
        control.MathFormat.FromStaticMathFormat(FontConstants.DefaultMathFormat);
        control.SetValue(AttachedProperties.LabeledIdFormatProperty, null);
        Control = control;
    }

    public EquationControl Control
    {
        get => _control;
        set
        {
            this.OneTimeSetExt(ref _control, value, nameof(Control));
            SetTransform();
        }
    }

    public double DesiredNumericHeight
    {
        get => _desiredNumericHeight;
        set
        {
            _desiredNumericHeight = value;
            SetTransform();
        }
    }

    public HorizontalAlignment HorizontalAlignment
    {
        get => _hAlignment;
        set
        {
            _hAlignment = value;
            ChangeTranslateTransformHorizontally();
        }
    }

    public VerticalAlignment VerticalAlignment
    {
        get => _vAlignment;
        set
        {
            _vAlignment = value;
            ChangeTranslateTransformVertically();
        }
    }

    private void SetTransform()
    {
        if (Control == null)
        {
            return;
        }

        if (_desiredNumericHeight == 0.0)
        {
            _scaleTransform = new ScaleTransform(1, 1);
        }
        else
        {
            _scaleTransform = Control.GetTransformBasedOnNumericNodeHeight(_desiredNumericHeight);
        }

        Control.GetAdornedElement().LayoutTransform = _scaleTransform;
        ChangeTranslateTransformHorizontally();
        ChangeTranslateTransformVertically();
        Control.LayoutTransform = _translateTransform;
    }

    private void ChangeTranslateTransformHorizontally()
    {
        switch (HorizontalAlignment)
        {
            case HorizontalAlignment.Center:
                _translateTransform.X = 0.0;
                return;
            case HorizontalAlignment.Left:
                _translateTransform.X = -InnerMargin * _scaleTransform.ScaleX;
                return;
            case HorizontalAlignment.Right:
                _translateTransform.X = InnerMargin * _scaleTransform.ScaleX;
                return;
            default:
                throw new NotSupportedException("Render transform(s) for maintaining the horizontal alignment is not yet implemented.");
        }
    }

    private void ChangeTranslateTransformVertically()
    {
        switch (VerticalAlignment)
        {
            case VerticalAlignment.Center:
                _translateTransform.Y = 0.0;
                return;
            case VerticalAlignment.Top:
                _translateTransform.Y = -InnerMargin * _scaleTransform.ScaleY;
                return;
            case VerticalAlignment.Bottom:
                _translateTransform.Y = InnerMargin * _scaleTransform.ScaleY;
                return;
            default:
                throw new NotSupportedException("Render transform(s) for maintaining the vertical alignment is not yet implemented.");
        }
    }
}
