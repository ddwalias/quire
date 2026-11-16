#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using Quire.Controls.Core;
using Quire.Controls.Core.AttachedBehavior;
using Quire.Controls.Core.Ole;
using Quire.Controls.ExcelComponent.ViewModel;
using Quire.PersistentData;
using Quire.Serialization;

namespace Quire.Controls.ExcelComponent.View;

public partial class OleObject
{
    private static readonly Type _typeOfThis;

    private Guid _guidFromXaml = Guid.Empty;

    public event Action<DependencyObject, DependencyObject> VisualParentChanged;

    public Guid ClassId
    {
        get
        {
            Guid classId = ViewModel.PublicModel.ClassId;
            if (!(classId == Guid.Empty))
            {
                return classId;
            }
            return _guidFromXaml;
        }
    }

    public Guid ClassIdForXamlSerialization
    {
        get
        {
            if (!IsInPrinting)
            {
                return Guid.Empty;
            }
            return ClassId;
        }
        set
        {
            _guidFromXaml = value;
        }
    }

    public IEnumerable<NodeMessageAdorner> NodeMessageAdorners => AttachedBehavior.GetMessagesDecorator(this).AdornerList;

    static OleObject()
    {
        _typeOfThis = typeof(OleObject);
        FrameworkElement.DefaultStyleKeyProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(_typeOfThis));
        FrameworkElement.WidthProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(OnSizePropertyChanged));
        FrameworkElement.HeightProperty.OverrideMetadata(_typeOfThis, new FrameworkPropertyMetadata(OnSizePropertyChanged));
    }

    public OleObject()
        : this(autoResize: true)
    {
    }

    public OleObject(bool autoResize)
        : base(new OleObjectViewModel(autoResize), new OleObjectErrorsDictionary())
    {
        AttachedProperties.SetRegisterCommandBindings(this, ViewModel.CommandBindings);
        SetBinding(FrameworkElement.MinWidthProperty, new Binding
        {
            Path = new PropertyPath("MinWidth"),
            Mode = BindingMode.OneWay
        });
        SetBinding(FrameworkElement.MinHeightProperty, new Binding
        {
            Path = new PropertyPath("MinHeight"),
            Mode = BindingMode.OneWay
        });
    }

    protected override bool ShouldSerializeProperty(DependencyProperty dp)
    {
        if (dp == FrameworkElement.MinWidthProperty || dp == FrameworkElement.MinHeightProperty)
        {
            return false;
        }
        return base.ShouldSerializeProperty(dp);
    }

    private static void OnSizePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        OleObject oleObject = sender as OleObject;
        if (DebugTools.AssertNotNull(oleObject, null).Failed)
        {
            return;
        }
        oleObject.ViewModel.UpdateOleSizeIfNeeded();
    }

    public bool CreateSiteWithDialog(IntPtr winHandler, string dialogTitle, string oleContainerPath)
    {
        bool isNew;
        bool isLink;
        bool isCanceled;
        bool flag = ViewModel.CreateOleWithDialog(winHandler, dialogTitle, oleContainerPath, out isNew, out isLink, out isCanceled);
        OnCreationFailure(flag, isCanceled, isNew);
        return flag;
    }

    public bool CreateSiteWithProgId(string progId, string oleContainerPath)
    {
        bool flag = ViewModel.CreateOleWithProgId(progId, oleContainerPath);
        OnCreationFailure(flag, isCancelled: false, isNew: true);
        return flag;
    }

    public bool CreateSiteFromFile(string filePath, bool link, string oleContainerPath)
    {
        bool flag = ViewModel.CreateOleFromFile(filePath, link, oleContainerPath);
        OnCreationFailure(flag, isCancelled: false, isNew: false);
        return flag;
    }

    private void OnCreationFailure(bool created, bool isCancelled, bool isNew)
    {
        if (!created && !isCancelled)
        {
            OnCreationError(isNew);
        }
    }

    public override oleObjectRegionType GetOlePersistentData(IS11NProvider s11NProvider, string uniqueId)
    {
        return base.GetOlePersistentData(s11NProvider, uniqueId);
    }

    protected override void OnVisualParentChanged(DependencyObject obj)
    {
        base.OnVisualParentChanged(obj);
        if (VisualParentChanged != null)
        {
            VisualParentChanged(obj, VisualParent);
        }
    }
}
