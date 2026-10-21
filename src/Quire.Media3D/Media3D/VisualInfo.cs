using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;

namespace Quire.Media3D;

public partial class VisualInfo : Animatable
{
    public static readonly Matrix3D ZeroMatrix = new Matrix3D(0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0);

    public static readonly DependencyProperty ModelVisual3DProperty;

    private static readonly DependencyPropertyKey TotalTransformKey;

    public static readonly DependencyProperty TotalTransformProperty;

    public ModelVisual3D ModelVisual3D
    {
        get => (ModelVisual3D)GetValue(ModelVisual3DProperty);
        set => SetValue(ModelVisual3DProperty, value);
    }

    public Matrix3D TotalTransform
    {
        get => (Matrix3D)GetValue(TotalTransformProperty);
        protected set => SetValue(TotalTransformKey, value);
    }

    private static void ModelVisual3DChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        (obj as VisualInfo).ModelVisual3DChanged(args);
    }

    private void ModelVisual3DChanged(DependencyPropertyChangedEventArgs args)
    {
        TotalTransform = GetTotalTransform(args.NewValue as ModelVisual3D);
    }

    public static Matrix3D GetTotalTransform(DependencyObject obj)
    {
        Matrix3D identity = Matrix3D.Identity;
        while (!(obj is Viewport3DVisual))
        {
            if (obj == null)
            {
                return ZeroMatrix;
            }
            if (obj is ModelVisual3D)
            {
                if ((obj as ModelVisual3D).Transform != null)
                {
                    identity.Append((obj as ModelVisual3D).Transform.Value);
                }
            }
            else
            {
                throw new ApplicationException("didn't end in Viewport3DVisual");
            }
            obj = VisualTreeHelper.GetParent(obj);
        }
        Matrix3D totalTransform = ViewportInfo.GetTotalTransform(obj as Viewport3DVisual);
        identity.Append(totalTransform);
        return identity;
    }

    public override string ToString()
    {
        return TotalTransform.ToString();
    }

    protected override Freezable CreateInstanceCore()
    {
        return new VisualInfo();
    }

    public VisualInfo()
    {
    }

    static VisualInfo()
    {
        ModelVisual3DProperty = DependencyProperty.Register("ModelVisual3D", typeof(ModelVisual3D), typeof(VisualInfo), new PropertyMetadata(null, new PropertyChangedCallback(ModelVisual3DChanged)));
        TotalTransformKey = DependencyProperty.RegisterReadOnly("TotalTransform", typeof(Matrix3D), typeof(VisualInfo), new PropertyMetadata(default(Matrix3D)));
        TotalTransformProperty = TotalTransformKey.DependencyProperty;
    }
}
