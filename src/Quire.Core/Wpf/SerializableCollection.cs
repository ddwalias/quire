#nullable disable
using Quire.Mirror;

namespace Quire.Wpf;

public partial class SerializableCollection<T>
{
    [MirrorVerified("PtcWpfSerializableCollection1MechanicalDifferentialTests")]
    protected SerializableCollection() { }

    [MirrorVerified("PtcWpfSerializableCollection1MechanicalDifferentialTests")]
    protected SerializableCollection(IEnumerable<T> items) : base(items) { }
}
