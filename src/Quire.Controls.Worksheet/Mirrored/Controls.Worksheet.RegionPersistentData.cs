using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Windows;
using System.Xml;
using Quire.Calculation.Core;
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
}
