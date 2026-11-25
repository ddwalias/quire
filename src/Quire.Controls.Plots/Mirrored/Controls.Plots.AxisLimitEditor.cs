#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Xml;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.Presenter;
using Quire.Controls.Plots.ViewModel;
using Quire.Core;
using Quire.PersistentDataObjects;
using Quire.Undo;

namespace Quire.Controls.Plots;

public partial class AxisLimitEditor : IAxisLimitEditor, ICalculatable, ICalculationHideableChild, INotifyPropertyChanged, ICalculationErrorOwner, ICalculatableResultOwner, ICalculatableInfo
{
    private const double CalculatedValueDefault = double.NaN;

    private readonly IEquationControl _eq;

    private readonly ICurrentNumberFormatProperty _currentNumberFormatProperty;

    private bool _hasErrors;

    private bool _isDefaultValue = true;

    private double _calculatedValue = double.NaN;

    private bool _hasCustomError;

    private bool _hideFromCalculationContext;

    private bool _hideFromCalculationContextOverride = true;

    public bool IsWaitingForEngineResponse { get; private set; }

    public double CalculatedValue
    {
        get
        {
            return _calculatedValue;
        }
        private set
        {
            if (_calculatedValue != value)
            {
                SetCalculatedValueField(value);
                if (!IsDefaultValue)
                {
                    FireValueChanged();
                }
            }
        }
    }

    public double PreCalculatedValue { get; private set; }

    public bool IsValueValid
    {
        get
        {
            if (!IsDefaultValue)
            {
                return !double.IsNaN(CalculatedValue);
            }
            return false;
        }
    }

    public bool IsDefaultValue
    {
        get
        {
            return _isDefaultValue;
        }
        set
        {
            SetIsDefaultValueInternal(value);
            UpdateHideFromCalculationContextToDefaultValue();
        }
    }

    public bool HasErrors
    {
        get
        {
            return _hasErrors;
        }
        private set
        {
            if (_hasErrors != value)
            {
                _hasErrors = value;
                FireHasErrorsChanged();
            }
        }
    }

    public XmlNode RegionXmlNode
    {
        get
        {
            return _eq.GetRegionXmlNode().FirstChild;
        }
        set
        {
            UnhookEquationChangedEvents();
            try
            {
                _eq.SetRegionXmlNode(value);
            }
            finally
            {
                HookupEquationChangedEvents();
            }
            IsDefaultValue = (value.LocalName == "placeholder");
        }
    }

    public string Rl => string.Format("(eval {0})", _eq.Rl);

    public bool CanHideFromCalculationContext => true;



    public EngineError CurrentError => Error;

    public EngineResult EngineResult { get; set; }

    public bool InterceptAndUpdateResult => true;

    public bool UseICalculatableToRestoreResult => true;

    public SynchronizationStatus SynchronizationStatus => _eq.SynchronizationStatus;

    public CalculatingStatus CalculatingStatus => _eq.CalculatingStatus;

    public int ErrorCount => _eq.ErrorCount;

    public string ErrorSExpression => _eq.ErrorSExpression;

    public string ErrorCode => _eq.ErrorCode;

    public string ErrorText => _eq.ErrorText;

    public bool IsCommitted
    {
        get
        {
            if (_eq.IsCommitted)
            {
                if (!IsDefaultValue && EngineResult.IsEmpty())
                {
                    return SynchronizationStatus == SynchronizationStatus.Pending;
                }
                return true;
            }
            return false;
        }
    }

    public EngineError Error => _eq.Error;

    public event Action ValueChanged;

    public event Action HasErrorsChanged;

    public event Action<ICalculatable> RequestEvaluation;

    public event Func<ICalculatable, string, IResultNode> GetValueOfIdentifierBefore;

    public event Action<ICalculatable> TreatAsUpToDate;

    public event PropertyChangedEventHandler PropertyChanged;

    public AxisLimitEditor(IEquationControl equationControl, ICurrentNumberFormatProperty currentNumberFormatProperty)
    {
        _eq = equationControl;
        _currentNumberFormatProperty = currentNumberFormatProperty;
        _eq.RequestEvaluation += EqRequestEvaluation;
        _eq.AllowExpressionsOnly = true;
        _eq.ProhibitComboBox = true;
        PreCalculatedValue = double.NaN;
        HookupEquationChangedEvents();
    }

    private void HookupEquationChangedEvents()
    {
        _eq.Changed += EquationChanged;
        _eq.SExpressionChanging += EquationSExpressionChanging;
    }

    private void UnhookEquationChangedEvents()
    {
        _eq.Changed -= EquationChanged;
        _eq.SExpressionChanging -= EquationSExpressionChanging;
    }

    private void SetCalculatedValueField(double value)
    {
        _calculatedValue = value;
    }

    private void SetIsDefaultValueInternal(bool newValue)
    {
        if (_isDefaultValue == newValue)
        {
            return;
        }
        _isDefaultValue = newValue;
        CreateNewSExpIfIsDefaultValueIsTrue();
        if (_isDefaultValue)
        {
            EngineResult = default(EngineResult);
            FireValueChanged();
        }
        else
        {
            ResetCalculatedValue();
        }
    }

    private void ResetCalculatedValue()
    {
        SetCalculatedValueField(double.NaN);
    }

    public void SetPreCalculatedValue(double defaultValue)
    {
        PreCalculatedValue = defaultValue;
        CreateNewSExpIfIsDefaultValueIsTrue();
    }

    public void SetNonCalculationRelatedCustomError(string message)
    {
        SetCustomError(message, false);
    }

    public void SetCalculationRelatedCustomError(string message)
    {
        SetCustomError(message, true);
    }

    private void SetCustomError(string message, bool calculationRelated)
    {
        DebugTools.DevAssert(calculationRelated == PlotErrorCodes.IsLimitsCalculationRelatedCustomError(message), null);
        _hasCustomError = true;
        EngineError error = new EngineError
        {
            Message = message
        };
        _eq.SetError(error);
        HasErrors = true;
    }

    public void ClearNonCalculationRelatedCustomErrors()
    {
        if (_hasCustomError && !PlotErrorCodes.IsLimitsCalculationRelatedCustomError(_eq.ErrorCode))
        {
            ClearAllCustomErrors();
        }
    }

    private void ClearAllCustomErrors()
    {
        _hasCustomError = false;
        _eq.SetError(null);
        HasErrors = false;
    }

    public void SetResult(IResultNode result)
    {
        IsWaitingForEngineResponse = false;
        if (result is IEmptyResultNode)
        {
            return;
        }
        DebugTools.DevAssert(_hasCustomError || !HasErrors, null);
        if (result is IRealResultNode realResultNode)
        {
            CalculatedValue = realResultNode.Value;
        }
        else if (result is IUnitedValueResultNode)
        {
            SetCalculationRelatedCustomError("Ptc.Plots.TICKMARKS_HAS_UNITS");
        }
        else
        {
            SetCalculationRelatedCustomError("RESULT_NOT_REAL_NUMBER");
        }
    }

    public void SetError(EngineError error)
    {
        IsWaitingForEngineResponse = false;
        if (_eq.EquationSExpression == "@PLACEHOLDER")
        {
            HasErrors = false;
            IsDefaultValue = true;
            return;
        }
        bool flag = true;
        if (_hasCustomError)
        {
            if (error == null)
            {
                flag = false;
            }
            else
            {
                ClearAllCustomErrors();
            }
        }
        if (flag)
        {
            _eq.SetError(error);
            HasErrors = (error != null);
        }
    }

    public void SetIdentifierLabels(IDictionary<ResolvedId, string[]> idLabelMap)
    {
    }

    public void SetAutoSelectAlgorithm(IDictionary<int, string> autoAlgorithmMap)
    {
    }

    public void SetRedefinitionWarnings(IList<EngineRedefInfo> warnings)
    {
        if (warnings.IsNotEmpty())
        {
            throw new NotImplementedException();
        }
    }

    private void EqRequestEvaluation(ICalculatable obj)
    {
        Commit();
    }

    public void Commit()
    {
        _eq.RequestEvaluation -= EqRequestEvaluation;
        _eq.Commit();
        _eq.RequestEvaluation += EqRequestEvaluation;
        if (!IsDefaultValue)
        {
            ResetCalculatedValue();
        }
        UpdateHideFromCalculationContextToDefaultValue();
        if (RequestEvaluation != null)
        {
            IsWaitingForEngineResponse = true;
            RequestEvaluation(this);
        }
    }

    public void SetStatusSynchronized()
    {
        if (TreatAsUpToDate != null)
        {
            TreatAsUpToDate(this);
        }
    }

    public void SetSynchronizationStatus(SynchronizationStatus syncStatus)
    {
        _eq.SetSynchronizationStatus(syncStatus);
    }

    public void SetCalculatingStatus(CalculatingStatus computingStatus)
    {
        _eq.SetCalculatingStatus(computingStatus);
    }

    private void SetHideFromCalculationContext(bool value)
    {
        bool hideFromCalculationContext = HideFromCalculationContext;
        if (_hideFromCalculationContext != value)
        {
            _hideFromCalculationContext = value;
            if (HideFromCalculationContext != hideFromCalculationContext)
            {
                FirePropertyChanged(new PropertyChangedEventArgs("HideFromCalculationContext"));
            }
        }
    }

    internal void UpdateHideFromCalculationContextToDefaultValue()
    {
        bool hideFromCalculationContext = HideFromCalculationContext;
        if (_hideFromCalculationContextOverride != IsDefaultValue)
        {
            _hideFromCalculationContextOverride = IsDefaultValue;
            if (HideFromCalculationContext != hideFromCalculationContext)
            {
                FirePropertyChanged(new PropertyChangedEventArgs("HideFromCalculationContext"));
            }
        }
    }

    private void EquationChanged(object sender, RoutedEventArgs e)
    {
        SetIsDefaultValueInternal(false);
        if (EngineResult.Errors != null)
        {
            EngineResult = new EngineResult(EngineResult, (EngineError)null);
        }
    }

    private void EquationSExpressionChanging(string obj)
    {
        IsDefaultValue = false;
    }

    private void CreateNewSExpIfIsDefaultValueIsTrue()
    {
        if (!IsDefaultValue)
        {
            return;
        }
        UnhookEquationChangedEvents();
        try
        {
            using (new CommandScope(new CompoundCommand()))
            {
                NumberFormat numberFormat = (_currentNumberFormatProperty == null) ? null : _currentNumberFormatProperty.CurrentNumberFormat;
                if (numberFormat == null)
                {
                    numberFormat = new NumberFormat();
                }
                SExpressionFormatter sExpressionFormatter = new SExpressionFormatter(numberFormat);
                string equationSExpression = _eq.EquationSExpression;
                string text = sExpressionFormatter.FormatReal(PreCalculatedValue);
                _eq.EquationSExpression = text;
                if (equationSExpression != text)
                {
                    _eq.NavigateRegionEnd();
                }
            }
        }
        finally
        {
            HookupEquationChangedEvents();
        }
        CalculatedValue = PreCalculatedValue;
    }

    private void FireValueChanged()
    {
        if (ValueChanged != null)
        {
            ValueChanged();
        }
    }

    private void FireHasErrorsChanged()
    {
        if (HasErrorsChanged != null)
        {
            HasErrorsChanged();
        }
    }

    private void FirePropertyChanged(PropertyChangedEventArgs e)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, e);
        }
    }

    public bool ContainsError(EngineError error)
    {
        if (Equals(error.ErrorSource))
        {
            return true;
        }
        return false;
    }

    public void ShowError(EngineError error)
    {
        ICalculationErrorOwner calculationErrorOwner = _eq;
        if (calculationErrorOwner != null)
        {
            calculationErrorOwner.ShowError(error);
        }
    }

    public void RestoreResult()
    {
        throw new NotImplementedException();
    }

    public valueType GetDataObject(IResultManager resultManager)
    {
        bool flag = !IsDefaultValue;
        bool flag2 = flag && resultManager != null;
        valueType valueType = new valueType();
        valueType.resultRefSpecified = flag2;
        valueType.resultRef = (flag2 ? resultManager.CreateResultRefId(this) : (-1));
        valueType.InnerXml = ((resultManager == null) ? null : (flag ? new XmlNode[1] { _eq.GetRegionXmlNode().FirstChild } : null));
        valueType.EquationSExpression = ((resultManager != null) ? null : (flag ? new string[1] { _eq.EquationSExpression } : null));
        return valueType;
    }

    private void SetEquationExpressionXml(XmlNode value)
    {
        XmlNode xmlNode = value.OwnerDocument.CreateElement("math", "ws");
        xmlNode.AppendChild(value);
        _eq.SetRegionXmlNode(xmlNode);
    }

    public void SetDataObject(valueType dataObject, IResultManager resultManager)
    {
        if (dataObject == null)
        {
            return;
        }
        UnhookEquationChangedEvents();
        bool isDefaultValue;
        try
        {
            if (dataObject.InnerXml != null && dataObject.InnerXml.Length != 0)
            {
                DebugTools.DevAssert(dataObject.InnerXml.Length == 1, null);
                XmlNode xmlNode = dataObject.InnerXml[0];
                SetEquationExpressionXml(xmlNode);
                isDefaultValue = (xmlNode.LocalName == "placeholder" && !dataObject.resultRefSpecified);
            }
            else if (dataObject.EquationSExpression != null && dataObject.EquationSExpression.Length != 0)
            {
                DebugTools.DevAssert(dataObject.EquationSExpression.Length == 1, null);
                _eq.EquationSExpression = dataObject.EquationSExpression[0];
                isDefaultValue = false;
            }
            else
            {
                isDefaultValue = true;
            }
        }
        finally
        {
            HookupEquationChangedEvents();
        }
        IsDefaultValue = isDefaultValue;
        resultManager.SetResultIfRefIdSpecified(dataObject.resultRef, dataObject.resultRefSpecified, this);
    }
}
