#nullable disable
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.MathInterface, interop.mathcad")]
[ComImport]
[CoClass(typeof(MathInterfaceClass))]
[Guid("7BDE1B35-0A1A-11E0-9EAB-11F8E2A8C36C")]
public interface MathInterface : IMathInterface
{
}
