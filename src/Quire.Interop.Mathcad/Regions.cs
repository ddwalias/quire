#nullable disable
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.Regions, interop.mathcad")]
[ComImport]
[Guid("7BDE4246-0A1A-11E0-99B7-11F8E2A8C36C")]
[CoClass(typeof(RegionsClass))]
public interface Regions : IMathcadRegions
{
}
