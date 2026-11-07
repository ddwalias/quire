#nullable disable
using System;
using System.Windows;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls.Core;

public partial interface IRegionFactory
{
    IRegionType ConvertToIRegionType(DelegateFunction0<IRegionType> typeCreator, IS11NProvider s11NProvider, UIElement element);
    IRegionPersistentData ConvertFromIRegionType(IDeS11NProvider deS11NProvider, Action<UIElement> deserializationProlog, IRegionType regionData, Func<IRegionType, bool> skip);
    IRegionPersistentData CreateFailedRegionPersistentData();
}
