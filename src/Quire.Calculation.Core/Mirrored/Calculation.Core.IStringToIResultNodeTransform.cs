#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface IStringToIResultNodeTransform
{
    global::Quire.Calculation.Core.IResultNode Transform(string s);
}
