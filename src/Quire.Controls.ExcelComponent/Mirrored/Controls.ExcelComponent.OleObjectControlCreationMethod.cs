#nullable disable
#pragma warning disable

namespace Quire.Controls.ExcelComponent;

public abstract partial class OleObjectControlCreationMethod
{
    public abstract bool Visit(OleObjectControl control, string oleContainerPath);

    protected OleObjectControlCreationMethod() { }
}
