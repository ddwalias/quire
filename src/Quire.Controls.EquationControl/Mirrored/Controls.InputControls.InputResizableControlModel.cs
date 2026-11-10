using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public abstract partial class InputResizableControlModel
{
    public InputResizableControlModel(IInputControlEngineFactory factory, IInputControlDataCalculatedValue data)
    {
        _data = data;
        _engineFactory = factory;
    }

    private IInputResizableControlData Data => (IInputResizableControlData)_data;

    public double Height
    {
        get => Data.Height;
        set => Data.Height = value;
    }

    public double Width
    {
        get => Data.Width;
        set => Data.Width = value;
    }

    public override void Dispose()
    {
    }
}
