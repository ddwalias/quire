#nullable disable
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.MathcadOld, interop.mathcad")]
[ComImport]
[Guid("7BDEB776-0A1A-11E0-882B-11F8E2A8C36C")]
[CoClass(typeof(MathcadOldClass))]
public interface MathcadOld : IMathcad
{
}
