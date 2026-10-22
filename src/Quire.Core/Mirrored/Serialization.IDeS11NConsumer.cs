#nullable disable
#pragma warning disable

namespace Quire.Serialization;

partial interface IDeS11NConsumer
{
    global::Quire.Serialization.IDeS11NProvider DeS11NProvider { get; set; }
}
