#nullable disable
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.Value, interop.mathcad")]
[ComImport]
[CoClass(typeof(ValueClass))]
[Guid("7BDE6955-0A1A-11E0-91D4-11F8E2A8C36C")]
public interface Value : IValue
{
}
