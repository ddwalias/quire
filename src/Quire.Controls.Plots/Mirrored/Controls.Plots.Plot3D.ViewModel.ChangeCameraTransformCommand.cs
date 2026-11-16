using System;
using System.Windows.Media.Media3D;
using Quire.Undo;

namespace Quire.Controls.Plots.Plot3D.ViewModel;

public partial class ChangeCameraTransformCommand : ICollectableCommand, IUndoableCommand
{
    private readonly IHasCameraTransform _transformOwner;

    private readonly Matrix3D _previousTransform;

    private readonly bool _applyTransformInDo;

    private DateTime _executed;

    private const int CollectionTime = 500;

    private bool IsCollectable { get; set; }

    private Matrix3D AppliedTransform { get; set; }

    public string Name => "Apply camera transform";

    public ChangeCameraTransformCommand(IHasCameraTransform transformOwner, Matrix3D? newTransform, Matrix3D previousTransform, bool isCollectable)
    {
        _transformOwner = transformOwner;
        _applyTransformInDo = newTransform.HasValue;
        AppliedTransform = (_applyTransformInDo ? newTransform.Value : transformOwner.CameraTransform.Value);
        IsCollectable = isCollectable && !_applyTransformInDo;
        DebugTools.DevAssert(!isCollectable || !_applyTransformInDo, null);
        _previousTransform = previousTransform;
    }

    public bool CollectNewCommand(ICollectableCommand nextCommand)
    {
        ChangeCameraTransformCommand changeCameraTransformCommand = nextCommand as ChangeCameraTransformCommand;
        if (IsCollectable && changeCameraTransformCommand != null && changeCameraTransformCommand.IsCollectable)
        {
            double totalMilliseconds = (DateTime.Now - _executed).TotalMilliseconds;
            DebugTools.DevAssert(totalMilliseconds >= 0.0, null);
            if (totalMilliseconds < 500.0)
            {
                AppliedTransform = changeCameraTransformCommand.AppliedTransform;
                _executed = DateTime.Now;
                return true;
            }
        }
        return false;
    }

    private void SetTransform(Matrix3D transform)
    {
        _transformOwner.CameraTransform = new MatrixTransform3D(transform);
        _transformOwner.RequestResetTrackball();
    }

    public bool Do()
    {
        _executed = DateTime.Now;
        if (_applyTransformInDo)
        {
            SetTransform(AppliedTransform);
        }
        return true;
    }

    public void Undo()
    {
        _transformOwner.FireRequestActivation();
        SetTransform(_previousTransform);
    }

    public void Redo()
    {
        _transformOwner.FireRequestActivation();
        SetTransform(AppliedTransform);
    }
}
