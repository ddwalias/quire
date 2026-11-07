#nullable disable
namespace Quire.Controls.Core.IORegions;

public partial interface IErrorState
{
    IOManagerErrorCode ErrorCode { get; }
}
