#nullable disable
#pragma warning disable

using System;
using System.ComponentModel;
using System.Windows;
using Quire.Calculation.Core.Support;
using Quire.Controls.Core;
using Quire.Controls.Whiteboard;
using Quire.PersistentData;
using Quire.Services;

namespace Quire.Controls.CollapsibleArea;

public partial class CollapsibleAreaModel
{
    public static partial class PropertyNames
    {
        public const string ProtectionData = "ProtectionData";

        public const string IsCollapsed = "IsCollapsed";

        public const string IsBorderVisible = "IsBorderVisible";
    }

    private bool _isCollapsed;

    private bool _isBorderVisible = true;

    public bool CanToggleCollapseState => !ProtectionData.ProtectionLevel().IsDisplayStateProtected();

    public bool IsCollapsed
    {
        get
        {
            return _isCollapsed;
        }
        private set
        {
            // Release IL returns early on an unchanged value; a block-if compiles to a branch to the exit instead.
            if (_isCollapsed == value)
            {
                return;
            }
            DebugTools.DevAssert(!PrintViewport.HasValue, null);
            _isCollapsed = value;
            WhiteboardItems.ContainedInCollapsedWhiteboard = value;
            FirePropertyChanged("IsCollapsed");
        }
    }

    public bool IsBorderVisible
    {
        get
        {
            return _isBorderVisible;
        }
        set
        {
            // Release IL returns early on an unchanged value; a block-if compiles to a branch to the exit instead.
            if (_isBorderVisible == value)
            {
                return;
            }
            _isBorderVisible = value;
            FirePropertyChanged("IsBorderVisible");
        }
    }

    public ProtectionData ProtectionData { get; private set; }

    bool Quire.Controls.Core.Protection.IProtectableModel.HasOpenDescendantOleObjects => HasOpenDescendantOleObjects;

    public Rect? PrintViewport { get; set; }

    public override bool IsInPrinting
    {
        get
        {
            return base.IsInPrinting;
        }
        set
        {
            if (value)
            {
                DebugTools.DevAssert(PrintViewport.HasValue, null);
            }
            base.IsInPrinting = value;
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public CollapsibleAreaModel()
        : base(null)
    {
    }

    public bool ToggleCollapseState()
    {
        if (DebugTools.DevAssert(CanToggleCollapseState, null).Failed)
        {
            return false;
        }
        IsCollapsed = !IsCollapsed;
        return true;
    }

    public bool ToggleBorderVisibleState()
    {
        IsBorderVisible = !IsBorderVisible;
        return true;
    }

    public void GetPersistentData(IAreaType data)
    {
        data.IsCollapsed = IsCollapsed;
        data.IsBorderVisible = IsBorderVisible;
        data.ProtectionDataWrapper = ProtectionData;
    }

    public void SetPersistentData(IAreaType data, Version formatVersion)
    {
        IsCollapsed = data.IsCollapsed;
        IsBorderVisible = data.IsBorderVisible;
        if (data.ProtectionDataWrapper != null)
        {
            SetProtection(data.ProtectionDataWrapper, formatVersion);
        }
    }

    public void SetProtection(ProtectionData protectionData, Version formatVersion = null)
    {
        DebugTools.DevAssert(ProtectionData == null, null);
        ProtectionData = ((protectionData.HasPassword && formatVersion != null && AppServiceRegistry.GetService<IEncryptor>().ShouldUpdateEncryption(formatVersion)) ? new ProtectionData(protectionData, formatVersion) : protectionData);
        FirePropertyChanged("ProtectionData");
    }

    public void RemoveProtection()
    {
        DebugTools.AssertNotNull(ProtectionData, null);
        ProtectionData = null;
        FirePropertyChanged("ProtectionData");
    }

    public override object Clone()
    {
        CollapsibleAreaModel collapsibleAreaModel = new CollapsibleAreaModel();
        CopyStateTo(collapsibleAreaModel);
        return collapsibleAreaModel;
    }

    protected void CopyStateTo(CollapsibleAreaModel copy)
    {
        copy.IsCollapsed = IsCollapsed;
        if (ProtectionData != null)
        {
            copy.ProtectionData = ProtectionData;
        }
        CopyStateTo((WhiteboardModel)copy);
    }

    protected override CalculatableChildHider InitializeChildManager()
    {
        return new CalculatableChildHider(WhiteboardItems, FireCalculatableAdded, FireCalculatableRemoved, FireCalculatableMoved, adoptCalculatableContainerChildren: true);
    }

    private void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    protected override bool ShouldCloneItem(IWhiteboardItem item)
    {
        if (PrintViewport.HasValue)
        {
            if (DebugTools.DevAssert(IsInPrinting, null).Passed)
            {
                return WhiteboardModel.WhiteboardItemMgr.GetItemIsInPrinting(item);
            }
            return false;
        }
        return true;
    }
}
