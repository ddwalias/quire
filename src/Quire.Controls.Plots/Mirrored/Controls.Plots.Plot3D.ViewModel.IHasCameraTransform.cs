using System;
using System.Windows.Media.Media3D;

namespace Quire.Controls.Plots.Plot3D.ViewModel;

public partial interface IHasCameraTransform
{
    Transform3D CameraTransform { get; set; }
    event Action ResetTrackball;
    void RequestResetTrackball();
    void FireRequestActivation();
}
