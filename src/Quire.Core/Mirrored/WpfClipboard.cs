#nullable disable
#pragma warning disable

namespace Quire;

partial class WpfClipboard
{
    public static global::Quire.IClipboard Current { get => _current ?? Default; }
}
