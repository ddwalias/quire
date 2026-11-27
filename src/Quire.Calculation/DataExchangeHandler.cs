using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Threading;
using Quire.Automation;
using Quire.Calculation.Core;
using Quire.Mpl.Run;
using IValue = Mathcad.IValue;

namespace Quire.Calculation;

internal abstract partial class DataExchangeHandler
{
    protected readonly IDataExchangeModel _dataExchangeModel;
    protected readonly Action _handleEngineNeedsUIThreadMessagesProcessed;
    protected readonly CalculationController.EngineOptions _engineOptions;
    protected bool _isDirty;
    protected bool _isCommitted;

    public DataExchangeHandler(IDataExchangeModel model, Dispatcher dispatcher, Action handleEngineNeedsUIThreadMessagesProcessed, CalculationController.EngineOptions engineOptions)
    {
        _engineOptions = engineOptions;
        _dataExchangeModel = model;
        _handleEngineNeedsUIThreadMessagesProcessed = handleEngineNeedsUIThreadMessagesProcessed;
    }

    public bool LockComponent(bool wait)
    {
        DebugTools.LogData("ComponentController.LockComponent: Start");
        _isDirty = false;
        _isCommitted = false;
        return _dataExchangeModel.Lock(wait);
    }

    public bool LockComponent()
    {
        return LockComponent(wait: true);
    }

    public void UnlockComponent()
    {
        _isDirty = false;
        _isCommitted = false;
        DebugTools.LogData("ComponentController.UnlockComponent: Start");
        _dataExchangeModel.Unlock();
    }

    protected abstract void SetValueDispatch(string id, IValue value, bool isAutoCalc);

    protected abstract IValue GetValueDispatch(string id, bool isCalculate, bool isAutoCalc);

    public void SetValue(string id, IValue value)
    {
        _isDirty = true;
        DateTime? refTime = DebugTools.LogData(string.Format("ComponentController.SetValue: {0}: Start", id));
        try
        {
            ExecuteWithDispatcher(() =>
            {
                SetValueDispatch(id, value, (bool)_engineOptions.Statuses[CalculationController.EngineStatuses.IsAutoCalc]);
            });
        }
        finally
        {
            DebugTools.LogData(string.Format("ComponentController.SetValue: {0}: End", id), ref refTime);
        }
    }

    public IValue GetValue(string id)
    {
        IValue value = ConvertToIValue(null);
        DateTime? refTime = DebugTools.LogData(string.Format("ComponentController.GetValue: {0}: Start", id));
        try
        {
            CommitIfNeeded();
            ExecuteWithDispatcher(() =>
            {
                value = GetValueDispatch(id,
                    (bool)_engineOptions.Statuses[CalculationController.EngineStatuses.IsRunScriptingOnRecalculate]
                        && (bool)_engineOptions.Statuses[CalculationController.EngineStatuses.IsCalculateAll],
                    (bool)_engineOptions.Statuses[CalculationController.EngineStatuses.IsAutoCalc]);
            });
        }
        finally
        {
            DebugTools.LogData(string.Format("ComponentController.GetValue: {0}: End", id), ref refTime);
        }
        return value;
    }

    public bool ShouldSkipUnchangedValue(string id)
    {
        return _dataExchangeModel.ShouldSkipUnchangedValueMultiThreaded(id);
    }

    public void Commit()
    {
        CommitIfNeeded();
    }

    protected virtual void CommitIfNeeded()
    {
        DebugTools.LogData("ComponentController.CommitIfNeeded: Start");
        if ((_dataExchangeModel.ShouldCommitIfDirty && _isDirty) || (_dataExchangeModel.ShouldCommitAtLeastOnce && !_isCommitted))
        {
            _isDirty = false;
            _isCommitted = true;
            ExecuteWithDispatcher(() =>
            {
                _dataExchangeModel.Commit();
            });
        }
    }

    protected void ExecuteWithDispatcher(Action action)
    {
        Exception exception = null;
        Action actionOnDispatcher = () =>
        {
            try
            {
                action();
            }
            catch (AddressErrorException ex2)
            {
                exception = new AppException(ex2.Message, 1u);
            }
            catch (Exception ex3)
            {
                exception = new AppException(ex3.Message);
            }
        };
        Dispatcher dispatcher = _dataExchangeModel.Dispatcher;
        if (dispatcher == null || dispatcher.CheckAccess())
        {
            actionOnDispatcher();
        }
        else
        {
            DispatcherOperation val = null;
            try
            {
                val = dispatcher.BeginInvoke(CalculationController.WaitForCalculationInterruptPriority_Send, actionOnDispatcher);
                _handleEngineNeedsUIThreadMessagesProcessed();
                val.Wait();
            }
            catch (Exception ex)
            {
                if (val != null && val.Status == DispatcherOperationStatus.Pending)
                {
                    val.Abort();
                }
                exception = new AppException(ex.Message);
            }
        }
        if (exception != null)
        {
            throw exception;
        }
    }

    protected static object ConvertFromIValue(IValue value)
    {
        // The case sections declare their locals in the switch block (no braces): the IL numbers them before the switch temp.
        switch (value.Type)
        {
            case "String":
                return ((StringValue)value).Value;
            case "Numeric":
                double real = ((NumericValue)value).Real;
                _ = ((NumericValue)value).Imag;
                if (double.IsNaN(real))
                {
                    return null;
                }
                return real;
            case "Matrix":
                MatrixValue matrixValue = (MatrixValue)value;
                object[,] array = new object[matrixValue.Rows, matrixValue.Cols];
                for (int i = 0; i < matrixValue.Rows; i++)
                {
                    for (int j = 0; j < matrixValue.Cols; j++)
                    {
                        array[i, j] = ConvertFromIValue(matrixValue.GetElement(i, j) as IValue);
                    }
                }
                return array;
            default:
                return value.AsString;
        }
    }

    protected static IValue ConvertToIValue(object value)
    {
        if (value is List<object> list)
        {
            if (list.Count == 1)
            {
                value = list[0];
            }
            else if (list.Count > 1)
            {
                value = new object[list.Count, 1];
                int num = 0;
                foreach (object item in list)
                {
                    ((object[,])value)[num, 0] = item;
                    num++;
                }
            }
            else
            {
                value = null;
            }
        }
        if (value == null)
        {
            return new NumericValue
            {
                Real = double.NaN
            };
        }
        if (value is double)
        {
            return new NumericValue
            {
                Real = (double)value
            };
        }
        if (value is long)
        {
            return new NumericValue
            {
                Real = (long)value
            };
        }
        if (value is float)
        {
            return new NumericValue
            {
                Real = (float)value
            };
        }
        if (value is short)
        {
            return new NumericValue
            {
                Real = (short)value
            };
        }
        if (value is int integer)
        {
            return new NumericValue
            {
                Integer = integer
            };
        }
        if (value is uint integer2)
        {
            return new NumericValue
            {
                Integer = (int)integer2
            };
        }
        if (value is ushort integer3)
        {
            return new NumericValue
            {
                Integer = integer3
            };
        }
        if (value is DateTime dateTime)
        {
            value = dateTime.ToString(CultureInfo.CurrentCulture);
        }
        if (value is bool flag)
        {
            value = flag.ToString();
        }
        if (value is string)
        {
            return new StringValue
            {
                Value = (string)value
            };
        }
        if (value is object[,] array)
        {
            MatrixValue matrixValue = new MatrixValue();
            int length = array.GetLength(0);
            int columnCount = array.GetLength(1);
            if (columnCount == 0 || length == 0)
            {
                matrixValue.SetElement(0, 0, new NumericValue
                {
                    Real = double.NaN
                });
            }
            else
            {
                for (int i = 0; i < length; i++)
                {
                    for (int j = 0; j < columnCount; j++)
                    {
                        matrixValue.SetElement(i, j, ConvertToIValue(array[i, j]));
                    }
                }
            }
            return matrixValue;
        }
        if (value is object[] vectorValues)
        {
            MatrixValue vectorMatrix = new MatrixValue();
            int valueCount = vectorValues.Length;
            if (valueCount == 0)
            {
                vectorMatrix.SetElement(0, 0, new NumericValue
                {
                    Real = double.NaN
                });
            }
            else
            {
                for (int k = 0; k < valueCount; k++)
                {
                    vectorMatrix.SetElement(k, 0, ConvertToIValue(vectorValues[k]));
                }
            }
            return vectorMatrix;
        }
        return new NumericValue
        {
            Real = double.NaN
        };
    }
}
