#nullable disable
using System.IO.Packaging;

namespace Quire.Controls.Core.Serialization;

public partial interface IPackagePartSerializable
{
    void Write(PackagePart part, bool useTempFile);
    void Read(PackagePart part);
}
