using System;
using Quire.Calculation;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Controls.Core.AttachedBehavior;
using Quire.Controls.Core.Ole;
using Quire.Controls.ExcelComponent.Model;
using Quire.Controls.ExcelComponent.ViewModel;
using Quire.PersistentData;
using Quire.Serialization;
namespace Quire.Controls.ExcelComponent.View;

public partial class ChartContainer
{
    protected override bool IsZoomAware => true;

    private static readonly Type _typeOfThis;

    private Int32Rect _renderRectInPixels;

    IChartContainerViewModel IChartContainer.ViewModel => ViewModel;

    IPublicChartContainerModel IChartContainer.Model => PublicModel;

    public override bool IsResizing
    {
        get
        {
            return base.IsResizing;
        }
        set
        {
            if (value == IsResizing)
            {
                return;
            }
            if (value)
            {
                PublicModel.AddImageFreezeSource(ImageFreezeSources.AsynchronousResize);
            }
            base.IsResizing = value;
            if (!value)
            {
                PublicModel.RemoveImageFreezeSource(ImageFreezeSources.AsynchronousResize);
            }
        }
    }

    public override ICalculationComponent Component => PublicModel;

    public Int32Rect RenderRectInPixels
    {
        get
        {
            return _renderRectInPixels;
        }
        set
        {
            if (value == _renderRectInPixels)
            {
                return;
            }
            _renderRectInPixels = value;
        }
    }

    static ChartContainer()
    {
        _typeOfThis = typeof(ChartContainer);
        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(_typeOfThis));
    }

    public ChartContainer()
        : this(new ChartContainerViewModel(new ChartContainerModel()))
    {
    }

    public ChartContainer(ChartContainerViewModel viewModel)
        : base(viewModel, new ChartContainerErrorsDictionary())
    {
        AttachedProperties.SetRegisterCommandBindings(this, ViewModel.CommandBindings);
    }

    protected override void OnIsInPrintingPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        DebugTools.DevAssert(IsOleCreated && IsStorageEmpty != CanPrint, null);
    }

    public override void OnEngineError(EngineError error)
    {
        if (error != null)
        {
            PublicModel.ClearTracesAndLimits();
        }
    }

    public IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, object data)
    {
        return SetOlePersistentData(d11NProvider, (chartContainerType)data);
    }

    public object GetRegionPersistentData(IS11NProvider s11NProvider)
    {
        return GetOlePersistentData(s11NProvider, s11NProvider.Helper.GetRegionId(this).ToString());
    }

    public override IList<IRegionPersistentData> SetOlePersistentData(IDeS11NProvider d11NProvider, chartContainerType data)
    {
        if (data.SizeInPixels != null)
        {
            RenderRectInPixels = new Int32Rect(0, 0, data.SizeInPixels.width, data.SizeInPixels.height);
        }
        return base.SetOlePersistentData(d11NProvider, data);
    }

    protected override void ApplyPrintingZoom(double? zoom)
    {
        ForceUnfreezeImage(zoom.HasValue);
        base.ApplyPrintingZoom(zoom);
    }

    protected override void OnZoomChanged()
    {
        ApplyRect(RenderSize, allowEmpty: true);
    }

    public void ForceUnfreezeImage(bool force)
    {
        PublicModel.ForceUnfreezeImage(force);
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
        try
        {
            Size val = ArrangeOverrideAux(arrangeBounds, updateOleSizeIfNeeded: false);
            if (SiteCreated)
            {
                PublicModel.AddImageFreezeSource(ImageFreezeSources.SynchronousUpdate, RenderSize != default(Size) && !val.IsEqualTo(RenderSize));
                ViewModel.UpdateOleSizeIfNeeded();
                ApplyRect(val, allowEmpty: false);
            }
            return val;
        }
        finally
        {
            PublicModel.RemoveImageFreezeSource(ImageFreezeSources.SynchronousUpdate);
        }
    }

    private void ApplyRect(Size size, bool allowEmpty)
    {
        if (!SiteCreated)
        {
            DebugTools.LogData("ChartContainer.ApplyRect: Early return (siteCreated == false)");
            return;
        }
        // Declared before the rectangle so the out temporary takes local slot 0, as in the Release IL.
        Size scale;
        Int32Rect renderRectInPixels = GetRenderRectInPixels(size, out scale);
        DebugTools.LogData("ChartContainer.ApplyRect: Start: " + ((Int32Rect)renderRectInPixels).ToString() + " (was: " + ((Int32Rect)RenderRectInPixels).ToString() + ")");
        if (renderRectInPixels.IsEmpty)
        {
            DebugTools.DevAssert(allowEmpty, null);
        }
        else if (renderRectInPixels != RenderRectInPixels)
        {
            RenderRectInPixels = renderRectInPixels;
            PublicModel.SetRect(renderRectInPixels);
        }
        DebugTools.LogData("ChartContainer.ApplyRect: End: " + ((Int32Rect)renderRectInPixels).ToString());
    }

}
