#nullable disable
namespace Quire.Controls.Core.Serialization;

public partial interface IMetadataModel
{
    string Creator { get; set; }
    string LastModified { get; set; }
    int Version { get; set; }
}
