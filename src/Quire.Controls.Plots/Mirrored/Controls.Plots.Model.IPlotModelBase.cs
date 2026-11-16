using System;
using System.Collections.Generic;
using System.ComponentModel;
using Quire.Calculation.Core;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls.Plots.Model;

public partial interface IPlotModelBase : INotifyPropertyChanged
{
    event Action DeserializationEnded;

    event Action Changed;

    IEnumerable<ICalculatable> GetHiddenAndVisibleCalculatables();

    bool SetDataObject(IDeS11NProvider de11NProvider, plotType dataObject, IResultManager resultManager);

    plotType GetDataObject(IS11NProvider s11NProvider, IResultManager resultManager, bool hideFromCalculationContext);
}
