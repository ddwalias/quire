using Quire.Calculation.Core;
using Quire.Engine;

namespace Quire.Calculation;

public partial class CalculationController
{
    private void HandleEngineNodeCallback(int handle, NodeStatus status, int clientCookie, int triggerHandle, int triggerCookie)
    {
        _handleToICalculatableMap.TryGetValue(handle, out var calculatable);
        if (calculatable != null)
        {
            DebugTools.LogData(string.Format("HandleEngineNodeCallback: {0}: {1}", handle, status));
            var handler = HandlersFactory.CreateEngineCallbackHandler(
                calculatable, _contextThreadDispatcher, IsCalculatableInContext, _engineOptions);
            switch (status)
            {
                case NodeStatus.NODE_COMPUTING:
                case NodeStatus.NODE_NOT_COMPUTING:
                    handler.SetCalculatingStatus((CalculatingStatus)status);
                    break;
                case NodeStatus.NODE_INVALID:
                    handler.SetSynchronizationStatus(SynchronizationStatus.Pending);
                    break;
                case NodeStatus.NODE_TREATED_READY:
                    // The original Synchronized value is 3; the legacy enum still declares it as 1.
                    handler.SetSynchronizationStatus((SynchronizationStatus)3);
                    break;
                case NodeStatus.NODE_READY:
                    handler.SetResult(handle, _engine, _handleToICalculatableMap);
                    break;
            }
        }
    }
}
