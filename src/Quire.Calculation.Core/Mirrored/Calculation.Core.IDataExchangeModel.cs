#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface IDataExchangeModel
{
    bool ShouldCommitAtLeastOnce { get; }
    bool ShouldCommitIfDirty { get; }
    global::System.Windows.Threading.Dispatcher Dispatcher { get; }
    bool Lock(bool wait);
    void Unlock();
    bool ShouldSkipUnchangedValueMultiThreaded(string id);
    void Commit();
}
