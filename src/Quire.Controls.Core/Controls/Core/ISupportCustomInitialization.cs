#nullable disable
namespace Quire.Controls.Core;

public partial interface ISupportCustomInitialization
{
    bool InCustomInitialization { get; set; }
}
