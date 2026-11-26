#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Quire.Calculation;
using Quire.Controls.Core;
using Quire.Controls.Core.AttachedBehavior;
using Quire.Controls.Core.Ole;
using Quire.Controls.ExcelComponent.Model;
using Quire.Controls.ExcelComponent.View;
using Quire.Controls.ExcelComponent.ViewModel;
using Quire.Core;
using Quire.PersistentData;
using Quire.Serialization;

namespace Quire.Controls.ExcelComponent;

public partial class ExcelContainer
{
    private static readonly Type _typeOfThis;

    static ExcelContainer()
    {
        _typeOfThis = typeof(ExcelContainer);
        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(_typeOfThis));
    }

    public ExcelContainer()
        : this(new ExcelContainerViewModel(new ExcelContainerModel()))
    {
    }

    public ExcelContainer(ExcelContainerViewModel viewModel)
        : base(viewModel, new ExcelContainerErrorsDictionary())
    {
        AttachedProperties.SetRegisterCommandBindings(this, ViewModel.CommandBindings);
    }

    IExcelContainerViewModel IExcelContainer.ViewModel
    {
        get
        {
            return ViewModel;
        }
    }

    protected override bool ShouldSerializeProperty(DependencyProperty property)
    {
        if (Control.TemplateProperty.Equals(property))
        {
            return false;
        }
        return base.ShouldSerializeProperty(property);
    }

    protected override Size MeasureOverride(Size constraint)
    {
        double height = double.IsNaN(Height) ? constraint.Height / 2.0 : Height;
        double width = double.IsNaN(Width) ? constraint.Width : Width;
        return ViewModel.Measure(new Size(width, height));
    }

    public bool? IsAsIconForXamlSerialization
    {
        get => ViewModel.IsAsIcon;
        set => ViewModel.IsAsIcon = value;
    }

    public bool SetValues(IEnumerable<Pair<string, object>> valuesToSet, bool forceRun)
    {
        return PublicModel.DoActionWithLock(() =>
        {
            valuesToSet.Each((Pair<string, object> pair) =>
            {
                PublicModel.SetValue(pair.First, pair.Second, forceRun);
            });
        });
    }

    public object[] GetValues(string[] cellIdxs, bool forceRun)
    {
        List<object> results = new List<object>();
        PublicModel.DoActionWithLock(() =>
        {
            results.AddRange(cellIdxs.Select((string cellIdx) => (dynamic)PublicModel.GetRange(cellIdx, forceRun).get_Value(Type.Missing)));
        });
        return results.ToArray();
    }

    protected override void OnCreationError()
    {
        base.OnCreationError();
        if (Environment.Is64BitProcess && OleUtils.IsExcel8Installed)
        {
            ThrowExceptionWithMessage("server_not_found_error");
        }
        else
        {
            base.OnCreationError(isNew: true);
        }
    }

    public IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, object data)
    {
        return SetOlePersistentData(d11NProvider, (excelContainerType)data);
    }

    public object GetRegionPersistentData(IS11NProvider s11NProvider)
    {
        return GetOlePersistentData(s11NProvider, s11NProvider.Helper.GetRegionId(this).ToString());
    }

    public override excelContainerType GetOlePersistentData(IS11NProvider s11NProvider, string uinqueId)
    {
        excelContainerType olePersistentData = base.GetOlePersistentData(s11NProvider, uinqueId);
        if (olePersistentData != null && !IsInCopy && !IsInPrinting && PublicModel.NeedRecalculateOnClose && PublicModel.HasAttachedInputsOrOutputs(checkOutputOnly: false))
        {
            ViewModel.SetMessage(MessageCategory.UnsynchronizedResults, NodeMessageAdornerType.Warning, "excel_results_unexpected_on_save");
        }
        return olePersistentData;
    }

    public override ICalculationComponent Component
    {
        get
        {
            return PublicModel;
        }
    }

}
