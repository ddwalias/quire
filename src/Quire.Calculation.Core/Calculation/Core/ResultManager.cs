#nullable disable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Xml;
using Quire;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class ResultManager : IResultManager
{
    private const int ResultRefIdMinusOne = -1;

    private static readonly IResultManager _instance = new ResultManager();

    private int _resultKey;

    private int _oldResultKey;

    private int forceAddIndex;

    private Dictionary<int, Pair<EngineResult, ICalculatableResultOwner>> _results;

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    private ResultManager()
    {
        Clear();
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public void Clear()
    {
        _resultKey = (_oldResultKey = -1);
        _results = new Dictionary<int, Pair<EngineResult, ICalculatableResultOwner>>();
    }

    public static IResultManager Instance
    {
        [DebuggerStepThrough]
        get => _instance;
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    private void AddResult(int key, Pair<EngineResult, ICalculatableResultOwner> value)
    {
        _resultKey = key;
        _results[_resultKey] = value;
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    private int AddResult(ICalculatableResultOwner calculatable)
    {
        Pair<EngineResult, ICalculatableResultOwner> value = new Pair<EngineResult, ICalculatableResultOwner>(GetEngineResultForSerializationFromCalculatable(calculatable), calculatable);
        _resultKey++;
        AddResult(_resultKey, value);
        return _resultKey;
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public void ReplaceCalculatableAndResult(ICalculatable oldCalculatable, ICalculatableResultOwner newCalculatable)
    {
        KeyValuePair<int, Pair<EngineResult, ICalculatableResultOwner>> keyValuePair = _results.FirstOrDefault((KeyValuePair<int, Pair<EngineResult, ICalculatableResultOwner>> pair) => pair.Value.Second == oldCalculatable);
        if (keyValuePair.Value.Second != null)
        {
            int resultKey = _resultKey;
            AddResult(keyValuePair.Key, new Pair<EngineResult, ICalculatableResultOwner>(GetEngineResultForSerializationFromCalculatable(newCalculatable), newCalculatable));
            _resultKey = resultKey;
        }
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public void ReplaceCalculatable(ICalculatable oldCalculatable, ICalculatableResultOwner newCalculatable)
    {
        KeyValuePair<int, Pair<EngineResult, ICalculatableResultOwner>> keyValuePair = _results.FirstOrDefault((KeyValuePair<int, Pair<EngineResult, ICalculatableResultOwner>> pair) => pair.Value.Second == oldCalculatable);
        if (keyValuePair.Value.Second != null)
        {
            int resultKey = _resultKey;
            AddResult(keyValuePair.Key, new Pair<EngineResult, ICalculatableResultOwner>(keyValuePair.Value.First, newCalculatable));
            _resultKey = resultKey;
        }
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public static EngineResult GetEngineResultForSerializationFromCalculatable(ICalculatableResultOwner resultOwner)
    {
        if (!(resultOwner is IResultOwnerWithSerializableEngineResult resultOwnerWithSerializableEngineResult))
        {
            return resultOwner.EngineResult;
        }
        return resultOwnerWithSerializableEngineResult.EngineResultForSerialization;
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public static void ErrorHandlesToSerialNumber(EngineError error, string rl)
    {
        if (error == null || rl == null)
        {
            return;
        }
        string[] array = rl.Split(':');
        bool found = false;
        for (uint num = 0u; num < array.Length; num++)
        {
            if (array[num].StartsWith(string.Format("0x{0:x8}", error.NodeHandle)))
            {
                error.NodeHandle = num;
                found = true;
                break;
            }
        }
        VerifyNodeHandle(found, error, rl);
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    private static void SerialNumberToErrorHandles(EngineError error, string rl)
    {
        if (error == null || rl == null)
        {
            return;
        }
        bool found = false;
        string[] array = rl.Split(':');
        if (error.NodeHandle > 0 && error.NodeHandle < array.Length)
        {
            string text = array[error.NodeHandle];
            if (text.Length > 9 && uint.TryParse(text.Substring(2, 8), NumberStyles.HexNumber, null, out var result))
            {
                error.NodeHandle = result;
                found = true;
            }
        }
        VerifyNodeHandle(found, error, rl);
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    private static void VerifyNodeHandle(bool found, EngineError error, string rl)
    {
        if (found)
        {
            DebugTools.DevAssert(error.NodeHandle != 0, "Handle found, but it is zero. RL:{0}", rl);
        }
        else if (error.NodeHandle != 0)
        {
            DebugTools.FailAssertion("Handle ({0}) not found at RL: {1}", error.NodeHandle, rl);
        }
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public static void DispatchResult(ICalculatableResultOwner resultOwner)
    {
        EngineResult engineResult = resultOwner.EngineResult;
        if (engineResult.IsEmpty())
        {
            return;
        }
        if (!resultOwner.UseICalculatableToRestoreResult)
        {
            resultOwner.RestoreResult();
            return;
        }
        if (engineResult.Warnings.IsNotEmpty())
        {
            resultOwner.SetRedefinitionWarnings(engineResult.Warnings);
        }
        resultOwner.SetSynchronizationStatus(engineResult.SynchronizationStatus);
        resultOwner.SetResult(engineResult.ResultNode);
        if (engineResult.Errors != null)
        {
            foreach (EngineError error2 in engineResult.Errors)
            {
                EngineError error = error2.Clone();
                SerialNumberToErrorHandles(error, resultOwner.Rl);
                resultOwner.SetError(error);
            }
        }
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public static void DispatchResultToDataExchange(ICalculatableResultOwner resultOwner, IDataExchangeScriptingModel dataExchangeScriptingModel, EngineResult result, bool isScriptingRunWorksheetOpen = false)
    {
        if (result.Warnings.IsNotEmpty())
        {
            resultOwner.SetRedefinitionWarnings(result.Warnings);
        }
        resultOwner.SetSynchronizationStatus(result.SynchronizationStatus);
        try
        {
            dataExchangeScriptingModel.PushResult(result.ResultNode, isScriptingRunWorksheetOpen);
        }
        catch (Exception)
        {
        }
        if (result.Errors != null)
        {
            foreach (EngineError error2 in result.Errors)
            {
                EngineError error = error2.Clone();
                resultOwner.SetError(error);
            }
        }
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public static void DispatchResult(ICalculatableResultOwner resultOwner, EngineResult result, bool isScriptingRunWorksheetOpen)
    {
        ICalculatableWithDataExchange calculatableWithDataExchange = resultOwner as ICalculatableWithDataExchange;
        if (calculatableWithDataExchange != null)
        {
            IDataExchangeScriptingModel dataExchangeScriptingModel = calculatableWithDataExchange.DataExchangeModel as IDataExchangeScriptingModel;
            if (dataExchangeScriptingModel != null)
            {
                DispatchResultToDataExchange(resultOwner, dataExchangeScriptingModel, result, isScriptingRunWorksheetOpen);
                return;
            }
        }
        resultOwner.EngineResult = result;
        DispatchResult(resultOwner);
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public static void DispatchResults(EngineResult[] results, IEnumerable<ICalculatableResultOwner> calculatables)
    {
        if (results != null)
        {
            for (int i = 0; i < results.Length; i++)
            {
                ICalculatableResultOwner calculatableResultOwner = calculatables.ElementAt(i);
                calculatableResultOwner.EngineResult = results[i];
                DispatchResult(calculatableResultOwner);
            }
        }
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public int CreateResultRefId(ICalculatableResultOwner calculatableResultOwner)
    {
        if (calculatableResultOwner != null)
        {
            return AddResult(calculatableResultOwner);
        }
        return -1;
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public void CreateResultRef(XmlElement xmlNode, ICalculatableResultOwner calculatableResultOwner)
    {
        int resultRefId = CreateResultRefId(calculatableResultOwner);
        if (IResultManagerHelper.IsResultRefIdSpecified(resultRefId))
        {
            xmlNode.SetAttribute("resultRef", resultRefId.ToString());
        }
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public int CreateOldResultRefId(IResultNode resultNode, ICalculatableResultOwner calculatable)
    {
        return AddOldResult(resultNode, calculatable);
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public void SetResultByRefId(int resultId, ICalculatableResultOwner calculatable, bool isForceAdd = false)
    {
        Pair<EngineResult, ICalculatableResultOwner> value;
        if (isForceAdd)
        {
            resultId += forceAddIndex++;
        }
        if (_results.ContainsKey(resultId))
        {
            value = _results[resultId];
            value.Second = calculatable;
            AddResult(resultId, value);
        }
        else
        {
            value = new Pair<EngineResult, ICalculatableResultOwner>(default(EngineResult), calculatable);
            AddResult(resultId, value);
        }
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public bool UpdateResultRef(XmlNode xmlNode, ICalculatableResultOwner calculatable)
    {
        if (xmlNode.Attributes["resultRef"] == null)
        {
            return false;
        }
        int resultId = int.Parse(xmlNode.Attributes["resultRef"].Value);
        SetResultByRefId(resultId, calculatable);
        return true;
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    private int AddOldResult(IResultNode resultNode, ICalculatableResultOwner calculatable)
    {
        _oldResultKey--;
        _results.Add(_oldResultKey, new Pair<EngineResult, ICalculatableResultOwner>(new EngineResult(resultNode, null, (SynchronizationStatus)3), calculatable));
        return _oldResultKey;
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public void SetEngineResult(int resultId, EngineResult engineResult)
    {
        if (_results.ContainsKey(resultId))
        {
            Pair<EngineResult, ICalculatableResultOwner> value = _results[resultId];
            value.First = engineResult;
            AddResult(resultId, value);
        }
    }

    public IEnumerable<KeyValuePair<int, Pair<EngineResult, ICalculatableResultOwner>>> Results => _results;

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public ICalculatable FindCalculatable(int resultId)
    {
        if (_results.ContainsKey(resultId))
        {
            return _results[resultId].Second;
        }
        return null;
    }

    [MirrorVerified("PtcCalculationCoreResultManagerMechanicalDifferentialTests")]
    public void PushResultsToOwners(bool isScriptingRunWorksheetOpen = false)
    {
        bool flag = false;
        foreach (KeyValuePair<int, Pair<EngineResult, ICalculatableResultOwner>> result in _results)
        {
            ICalculatableResultOwner second = result.Value.Second;
            if (second == null)
            {
                flag = true;
                continue;
            }
            try
            {
                DispatchResult(second, result.Value.First, isScriptingRunWorksheetOpen);
                if (result.Value.First.SynchronizationStatus == (SynchronizationStatus)3)
                {
                    second.SetStatusSynchronized();
                }
            }
            catch (Exception ex)
            {
                DebugTools.FailAssertion(ex.Message);
            }
        }
        DebugTools.DevAssert(!flag, "Pushing results to empty owners is wrong! Is this called inside the Converter ?");
    }
}
