#nullable disable
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.MatrixValue, interop.mathcad")]
[ComImport]
[Guid("7BDE9063-0A1A-11E0-BA2A-11F8E2A8C36C")]
[CoClass(typeof(MatrixValueClass))]
public interface MatrixValue : IMatrixValue
{
}
