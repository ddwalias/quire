#nullable disable
#pragma warning disable

namespace Quire.Services;

partial interface IExceptionLogger
{
    string LogException(global::System.Exception exception) ;
    string GetExceptionInfo(global::System.Collections.Generic.IEnumerable<global::System.Exception> exceptions) ;
    string LogExceptions(global::System.Collections.Generic.IEnumerable<global::System.Exception> exceptions) ;
}
