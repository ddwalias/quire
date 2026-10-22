#nullable disable
#pragma warning disable

namespace Quire.PersistentDataObjects;

partial interface IRegionInWhiteboardType
{
    double top { get; set; }
    double left { get; set; }
    global::System.Windows.Point Location { get; set; }
}
