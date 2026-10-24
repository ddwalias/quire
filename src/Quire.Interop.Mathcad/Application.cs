#nullable disable
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.Application, interop.mathcad")]
[ComImport]
[CoClass(typeof(ApplicationClass))]
[Guid("7BDE4247-0A1A-11E0-A89F-11F8E2A8C36C")]
public interface Application : IMathcadApplication, _IApplicationEvents_Event
{
}
