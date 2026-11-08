#nullable disable
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Quire.Calculation.Core;
using Quire.Engine;

namespace Quire.Calculation;

public partial class EngineWarningHandler
{
    private readonly Dictionary<int, ICalculatable> _handleToICalculatableMap;

    public EngineWarningHandler(Dictionary<int, ICalculatable> handleToICalculatableMap)
    {
        _handleToICalculatableMap = handleToICalculatableMap;
    }

    public IEnumerable<EngineRedefInfo> HandleWarnings(IList list)
    {
        CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
        CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
        if (Application.Current != null && ((DispatcherObject)Application.Current).Dispatcher != null)
        {
            Thread.CurrentThread.SyncWithUICulture();
        }
        foreach (RedefInfo item in list)
        {
            EngineRedefInfo warning = HandleWarning(item);
            yield return warning;
        }
        Thread.CurrentThread.CurrentCulture = currentCulture;
        Thread.CurrentThread.CurrentUICulture = currentUICulture;
    }

    private EngineRedefInfo HandleWarning(RedefInfo redefInfo)
    {
        return new EngineRedefInfo
        {
            Name = redefInfo.Name,
            OldType = ParseTypeString(redefInfo.OldType),
            OldLocation = redefInfo.OldLocation
        };
    }

    private static EngineDefType ParseTypeString(string engineTypeStr)
    {
        EngineDefType result = EngineDefType.Value;
        if (engineTypeStr.Contains("->"))
        {
            result = EngineDefType.Function;
        }
        else if (engineTypeStr.Contains("Matrix") || engineTypeStr.Contains("MatVal"))
        {
            result = EngineDefType.Matrix;
        }
        else if (engineTypeStr.Contains("Value"))
        {
            result = EngineDefType.Value;
        }
        else if (engineTypeStr == "a")
        {
            result = EngineDefType.Value;
        }
        else if (new Regex("^V\\s*{\\s*([^0-9\\.\\s\\p{C}]|[\\u0086\\u0087])([^\\s\\p{C}]|[\\u0086\\u0087])*$|^(\\.([0-9]*([^0-9\\s\\p{C}]|[\\u0086\\u0087])[0-9]*)*)\\s*}\\s*$").IsMatch(engineTypeStr))
        {
            result = EngineDefType.Value;
        }
        else
        {
            DebugTools.FailAssertion("couldn't parse valid type from string: " + engineTypeStr);
        }
        return result;
    }
}
