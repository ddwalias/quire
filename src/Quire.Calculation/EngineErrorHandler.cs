#nullable disable
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Quire.Calculation.Core;
using Quire.Mpl.Run;

namespace Quire.Calculation;

public partial class EngineErrorHandler
{
    private readonly Dictionary<int, ICalculatable> _handleToICalculatableMap;

    public EngineErrorHandler(Dictionary<int, ICalculatable> handleToICalculatableMap)
    {
        _handleToICalculatableMap = handleToICalculatableMap;
    }

    public IEnumerable<EngineError> HandleErrors(IList list)
    {
        CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
        CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
        if (Application.Current != null && ((DispatcherObject)Application.Current).Dispatcher != null)
        {
            Thread.CurrentThread.SyncWithUICulture();
        }
        foreach (AppException item in list)
        {
            EngineError error = HandleError(item);
            yield return error;
        }
        Thread.CurrentThread.CurrentCulture = currentCulture;
        Thread.CurrentThread.CurrentUICulture = currentUICulture;
    }

    private EngineError HandleError(AppException appException)
    {
        EngineError engineError = new EngineError();
        if (_handleToICalculatableMap.TryGetValue((int)appException.Region, out var value))
        {
            engineError.ErrorSource = value;
        }
        engineError.Message = appException.Message;
        engineError.NodeHandle = appException.Tag;
        engineError.Argument = appException.Argument;
        engineError.ResourceString = EngineErrorMessages.GetStringByKey(engineError.ErrorCode);
        if (appException.InnerException == null)
        {
            engineError.OriginalError = null;
        }
        else
        {
            engineError.OriginalError = HandleError((AppException)appException.InnerException);
        }
        return engineError;
    }
}
