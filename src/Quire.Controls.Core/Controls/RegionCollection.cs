#nullable disable
using System;
using System.Runtime.Serialization;
using System.Windows;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls;

public partial class RegionCollection : SerializableCollection<UIElement>
{
    [MirrorVerified("PtcControlsRegionCollectionMechanicalDifferentialTests")]
    public RegionCollection() { }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        int count = Count;
        info.AddValue("count", count);
        for (int i = 0; i < Count; i++)
        {
            info.AddValue(i.ToString(), base[i]);
        }
    }

    protected RegionCollection(SerializationInfo info, StreamingContext context)
    {
        int num = (int)info.GetValue("count", typeof(int));
        for (int i = 0; i < num; i++)
        {
            Add(info.GetValue(i.ToString(), info.GetType()) as UIElement);
        }
    }
}
