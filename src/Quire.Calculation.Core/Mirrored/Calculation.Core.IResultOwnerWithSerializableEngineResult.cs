#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface IResultOwnerWithSerializableEngineResult
{
    global::Quire.Calculation.Core.EngineResult EngineResultForSerialization { get; }
}
