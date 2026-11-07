#nullable disable
using System.Collections.Generic;

namespace Quire.Controls.Core.IORegions;

public partial interface ISetValueResults
{
    int Count { get; }
    IEnumerable<IOManagerErrorCode> AllResults { get; }
    IOManagerErrorCode GetResult(int index);
    IOManagerErrorCode GetResult(string alias);
}
