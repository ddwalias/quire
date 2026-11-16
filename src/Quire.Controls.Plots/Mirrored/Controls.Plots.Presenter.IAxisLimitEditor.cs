using System;
using System.ComponentModel;
using System.Xml;
using Quire.Calculation.Core;
using Quire.PersistentDataObjects;

namespace Quire.Controls.Plots.Presenter;

public partial interface IAxisLimitEditor : ICalculatable, ICalculationHideableChild, INotifyPropertyChanged, ICalculationErrorOwner, ICalculatableResultOwner, ICalculatableInfo
{
    double CalculatedValue { get; }

    double PreCalculatedValue { get; }

    bool IsDefaultValue { get; set; }

    bool IsValueValid { get; }

    XmlNode RegionXmlNode { get; set; }

    bool HasErrors { get; }

    bool IsWaitingForEngineResponse { get; }

    event Action ValueChanged;

    event Action HasErrorsChanged;

    void SetPreCalculatedValue(double defaultValue);

    void SetNonCalculationRelatedCustomError(string errorMessage);

    void ClearNonCalculationRelatedCustomErrors();

    valueType GetDataObject(IResultManager resultManager);

    void SetDataObject(valueType dataObject, IResultManager resultManager);
}
