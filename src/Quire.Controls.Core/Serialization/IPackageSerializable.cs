#nullable disable
using System.IO.Packaging;

namespace Quire.Controls.Core.Serialization;

public partial interface IPackageSerializable
{
    void Write(Package package, bool useTempFile = false);
    void Read(Package package);
}
