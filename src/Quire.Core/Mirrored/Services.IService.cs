#nullable disable
#pragma warning disable

namespace Quire.Services;

partial interface IService
{
    bool IsConnected { get; set; }
}
