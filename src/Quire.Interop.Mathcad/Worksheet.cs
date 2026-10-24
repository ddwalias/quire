#nullable disable
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.Worksheet, interop.mathcad")]
[ComImport]
[Guid("7BDDF425-0A1A-11E0-AD8E-11F8E2A8C36C")]
[CoClass(typeof(WorksheetClass))]
public interface Worksheet : IMathcadWorksheet, _IWorksheetEvents_Event
{
}
