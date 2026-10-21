#nullable enable

namespace System.Reflection.Emit.Quire.Mpl;

public partial class MetadataReferenceTracker
{
    public static MetadataReferenceTracker? GetInstance()
    {
        return _instance;
    }
}
