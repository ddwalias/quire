#nullable disable
#pragma warning disable

namespace Quire.Wpf;

partial interface INotifyProgress
{
    bool IsTrackingInProgress { get; }
    event global::System.ComponentModel.ProgressChangedEventHandler ProgressChanged ;
    event global::System.Action<bool> ProgressStopped ;
    void SetProgressAbortTimeout(int msThreshold) ;
    void StartProgressTracking() ;
    void AbortProgressTracking() ;
    bool IsProgressTrackingAborted() ;
}
