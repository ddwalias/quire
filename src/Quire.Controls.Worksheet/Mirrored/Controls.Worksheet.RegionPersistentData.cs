using System;
using System.Collections.Generic;
using Quire.Controls.Core.BookmarkTags;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Core.Whiteboard;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.Core;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls.Worksheet;

public partial class RegionPersistentData
{
    public RegionPersistentData(UIElement element)
    {
        Control = element;
    }

    public IRegionType RegionData { get; private set; }

    public Point Location { get; set; }

    public IS11NProvider S11NProvider { get; set; }

    public IDeS11NProvider DeS11NProvider { get; set; }

    public Exception DeserializationException { private get; set; }

    public IList<IRegionPersistentData> Children { private get; set; }

    public UIElement Control { get; private set; }

    public virtual bool HasDeserializationErrors => !AllDeserializationExceptions.IsNullOrEmpty();

    protected bool HideFromCalculationContext
    {
        set
        {
            if (Control is ICalculationHideableChild)
            {
                (Control as ICalculationHideableChild).HideFromCalculationContext = value;
            }
        }
    }

    protected PackagePart PackagePart
    {
        get
        {
            if (BaseS11NProvider == null)
            {
                return null;
            }
            return BaseS11NProvider.PackagePart;
        }
    }

    protected ISerializationHelper Helper
    {
        get
        {
            if (BaseS11NProvider == null)
            {
                return null;
            }
            return BaseS11NProvider.Helper;
        }
    }

    protected XmlDocument XmlContentDocument
    {
        get
        {
            if (S11NProvider == null)
            {
                return null;
            }
            return S11NProvider.XmlContentDocument;
        }
    }

    protected XmlNamespaceManager NameSpaceManager
    {
        get
        {
            if (BaseS11NProvider == null)
            {
                return null;
            }
            return BaseS11NProvider.NameSpaceManager;
        }
    }

    private IBaseS11NProvider BaseS11NProvider
    {
        get
        {
            if (S11NProvider != null)
            {
                return S11NProvider;
            }
            if (DeS11NProvider != null)
            {
                return DeS11NProvider;
            }
            DebugTools.DevAssert(false, "RegionPersistentData should has or S11NProvider either DS11NProvider");
            return null;
        }
    }

    public void Deserialize(IRegionType regionData)
    {
        RegionData = regionData;
        Deserialize();
    }

    private void DeserializeLocation()
    {
        if (RegionData is IRegionInWhiteboardType regionInWhiteboard)
        {
            Location = regionInWhiteboard.Location;
        }
    }

    protected virtual void DeserializeDimensions()
    {
        DeserializeHeight();
        DeserializeWidth();
    }

    public virtual void DeserializeEpilog()
    {
    }

    protected virtual void Deserialize()
    {
        DeserializeLocation();
        DeserializeCommonRegionProperties();
        Children = ((IPersistentDataProvider)Control).SetRegionPersistentData(DeS11NProvider, RegionData.Item);
    }

    protected void DeserializeHeight()
    {
        FrameworkElement frameworkElement = GetFrameworkElementForDimensionsSerialization(Control);
        if (DebugTools.AssertNotNull(frameworkElement, null).Passed && RegionData.heightSpecified)
        {
            frameworkElement.Height = RegionData.height;
        }
    }

    protected void DeserializeWidth()
    {
        FrameworkElement frameworkElement = GetFrameworkElementForDimensionsSerialization(Control);
        if (DebugTools.AssertNotNull(frameworkElement, null).Passed && RegionData.widthSpecified)
        {
            frameworkElement.Width = RegionData.width;
        }
    }

    protected void DeserializeCommonRegionProperties()
    {
        if (long.TryParse(RegionData.regionid, out long regionId))
        {
            Helper.SetRegionId(Control, regionId);
        }
        if (RegionData.ConvertedFromMC14)
        {
            SerializationHelperUtils.SetConvertedFromMathcad14(Control);
        }
        if (RegionData.Tag.IsNotEmpty())
        {
            BookmarkTag.SetBookmarkTag(Control, RegionData.Tag);
        }
        DeserializeConversionLossAnotations(RegionData.mc14conversionlossannotations, RegionData.mc14conversionloss);
        if (RegionData.ShowBorder)
        {
            DependencyObject control = Control;
            if (control != null)
            {
                WhiteboardDependencyProperties.SetIsRegionBorderVisible(control, true);
            }
        }
    }

    private void DeserializeConversionLossAnotations(string[] annotations, bool isChangedByConversionFromMC14)
    {
        if (annotations != null && annotations.Length != 0)
        {
            AnnotationTypeList annotationTypes = new AnnotationTypeList();
            foreach (string annotation in annotations)
            {
                if (EnumHelper.TryParse(annotation, out MC14ConversionAnnotationType annotationType, false))
                {
                    annotationTypes.AddOnce(annotationType);
                }
            }
            MC14ConversionAnnotation.SetConversionAnnotations(Control, annotationTypes);
        }
        else if (isChangedByConversionFromMC14)
        {
            MC14ConversionAnnotation.SetOldConversionAnnotation(Control);
        }
    }

    public static FrameworkElement GetFrameworkElementForDimensionsSerialization(UIElement control)
    {
        if (control is IResizableElementInfo)
        {
            SubElementToResizeData subElementToResize = (control as IResizableElementInfo).SubElementToResizeData;
            if (subElementToResize != null && subElementToResize.SerializeDimensions)
            {
                return subElementToResize.Element;
            }
        }
        return control as FrameworkElement;
    }
}
