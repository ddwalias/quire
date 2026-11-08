#nullable disable
#pragma warning disable
using System;
using System.Collections;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.Core.Ole;
using Quire.Controls.ExcelComponent.Model;
using Quire.PersistentData;

namespace Quire.Controls.ExcelComponent.ViewModel;

public partial class ChartContainerViewModel : OleContainerItemViewModel<ChartContainerModel, IPublicChartContainerModel, chartContainerType>, IChartContainerViewModel, IOleContainerItemViewModel, IOleContainerViewModel
{
    private ChartContainerModel _model;

    public ChartContainerViewModel(ChartContainerModel model)
        : base(model, new Size(96.00000000000001, 96.00000000000001))
    {
        _model = model;
    }

    public override Size EstablishInitialSize()
    {
        Size size = new Size(480.00000000000006, 288.00000000000006);
        Size oleViewportSize = OleViewportSize;
        if (oleViewportSize.IsEmpty)
        {
            OleViewportSize = size;
            DebugTools.FailAssertionDoNotThrow((string)null);
        }
        return ApplySizeRestrictions(size);
    }

    public override void OnWorksheetPropertiesChanged(string[] propertyNames, IWorksheetProperties oldProperties, IWorksheetProperties newProperties)
    {
        base.OnWorksheetPropertiesChanged(propertyNames, oldProperties, newProperties);
        if (((IList)propertyNames).Contains("HelpServerPath"))
        {
            if (Uri.TryCreate(newProperties.HelpServerPath, UriKind.Absolute, out Uri result))
            {
                try
                {
                    _model.HelpServerPath = (result.IsFile ? result.LocalPath : result.AbsoluteUri);
                    return;
                }
                catch (Exception ex)
                {
                    DebugTools.FailAssertion(ex);
                    return;
                }
            }
            DebugTools.FailAssertion("Failed to create URI: " + newProperties.HelpServerPath);
        }
    }
}
