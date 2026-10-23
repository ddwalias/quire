#nullable disable
using Quire.Mirror;

namespace Quire.Wpf;

public partial class SerializableCollection<T>
{
    [MirrorVerified("PtcWpfSerializableCollection1MechanicalDifferentialTests")]
    protected SerializableCollection() { }

    [MirrorVerified("PtcWpfSerializableCollection1MechanicalDifferentialTests")]
    protected SerializableCollection(IEnumerable<T> items) : base(items) { }

    public virtual void GetObjectData(global::System.Runtime.Serialization.SerializationInfo info, global::System.Runtime.Serialization.StreamingContext context)
    {
        int count = Count;
        info.AddValue("count", count);
        for (int i = 0; i < Count; i++)
        {
            info.AddValue(i.ToString(), base[i]);
        }
    }
}
