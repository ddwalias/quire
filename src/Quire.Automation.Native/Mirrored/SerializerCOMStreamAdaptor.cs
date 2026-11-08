using System.Runtime.InteropServices.ComTypes;

namespace Quire.Automation;

public partial class SerializerCOMStreamAdaptor
{
    public SerializerCOMStreamAdaptor(IStream stream)
        : base(stream)
    {
    }

    protected override void Commit()
    {
        try
        {
            base.Commit();
        }
        catch (NotImplementedException)
        {
        }
    }
}
