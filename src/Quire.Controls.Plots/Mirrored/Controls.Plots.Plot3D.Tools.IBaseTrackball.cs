using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using Quire.Controls.Plots.Plot3D.ViewModel;

namespace Quire.Controls.Plots.Plot3D.Tools;

internal partial interface IBaseTrackball
{
    Transform3D CameraTransform { get; }

    Viewport3D Viewport { get; }

    Rect3D Model3DBoundingBox { get; }

    event Action<Matrix3D> MouseUp;

    event Action<Matrix3D> MouseWheel;

    void Attach(FrameworkElement mouseEventsSource, IHasCameraTransform hasCameraTransform, Viewport3D viewport);

    void Reset();

    void Detach();

    void HandleMouseDown(Point rawLocation, MouseButton mouseButton, bool useTimer);

    void HandleMouseUp(Point rawLocation, MouseButton mouseButton);

    void HandleMouseMove(Point rawLocation);

    void HandleMouseWheel(int ticks);
}
