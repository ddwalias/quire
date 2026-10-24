#nullable disable
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.Worksheets, interop.mathcad")]
[ComImport]
[CoClass(typeof(WorksheetsClass))]
[Guid("7BDDF427-0A1A-11E0-8724-11F8E2A8C36C")]
public interface Worksheets : IMathcadWorksheets, _IWorksheetEvents_Event
{
}
