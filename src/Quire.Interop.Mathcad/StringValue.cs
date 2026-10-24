#nullable disable
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.StringValue, interop.mathcad")]
[ComImport]
[Guid("7BDE6957-0A1A-11E0-9F09-11F8E2A8C36C")]
[CoClass(typeof(StringValueClass))]
public interface StringValue : IStringValue
{
}
