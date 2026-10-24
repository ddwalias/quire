#nullable disable
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.Windows, interop.mathcad")]
[ComImport]
[Guid("7BDEB775-0A1A-11E0-AC76-11F8E2A8C36C")]
[CoClass(typeof(WindowsClass))]
public interface Windows : IMathcadWindows
{
}
