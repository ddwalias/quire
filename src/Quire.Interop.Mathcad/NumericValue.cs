#nullable disable
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.NumericValue, interop.mathcad")]
[ComImport]
[CoClass(typeof(NumericValueClass))]
[Guid("7BDE6956-0A1A-11E0-8F53-11F8E2A8C36C")]
public interface NumericValue : INumericValue
{
}
