#nullable disable
using System.Runtime.InteropServices;

namespace Quire;

[global::Quire.Mirror.MirrorOf("Mathcad.Region, interop.mathcad")]
[ComImport]
[CoClass(typeof(RegionClass))]
[Guid("7BDE1B34-0A1A-11E0-A16D-11F8E2A8C36C")]
public interface Region : IMathcadRegion
{
}
