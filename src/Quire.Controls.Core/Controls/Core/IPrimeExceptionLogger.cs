#nullable disable
using System;
using System.Collections.Generic;

namespace Quire.Controls.Core;

public partial interface IPrimeExceptionLogger
{
    Func<IEnumerable<string>> OpenFilesListGetter { set; }
    string GetOpenFilesData();
    string GetUndoStackData();
}
