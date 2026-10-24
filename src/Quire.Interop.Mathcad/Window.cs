#nullable disable
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.Window, interop.mathcad")]
[ComImport]
[Guid("7BDE9064-0A1A-11E0-9E91-11F8E2A8C36C")]
[CoClass(typeof(WindowClass))]
public interface Window : IMathcadWindow
{
}
