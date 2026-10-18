#nullable disable
using System.IO;
using System.Runtime.Serialization.Json;
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public static partial class VectorsListJSonSerializer<T>
{
    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsListJSonSerializer1MechanicalDifferentialTests")]
    private static readonly DataContractJsonSerializer _jSonVectorsListSerializer;

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsListJSonSerializer1MechanicalDifferentialTests")]
    internal static void Write(Stream jsonStream, T[] flatList) => _jSonVectorsListSerializer.WriteObject(jsonStream, flatList);
}
