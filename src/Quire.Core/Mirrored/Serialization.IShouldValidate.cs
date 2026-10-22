#nullable disable
#pragma warning disable

namespace Quire.Serialization;

partial interface IShouldValidate
{
    bool ShouldValidateOnWrite { get; }
    bool ShouldValidateOnRead { get; }
}
