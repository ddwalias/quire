using System;
using System.Windows;
using Quire.Controls.Core;
using Quire.Controls.ExcelComponent.Model;
using Quire.Controls.ExcelComponent.ViewModel;
using Quire.FunctionalitiesLimitation;
using Quire.PersistentData;

namespace Quire.Controls.ExcelComponent.ViewModel;

public partial class OleObjectViewModel
{
    public OleObjectViewModel(bool autoResize)
        : base(new OleObjectModel(autoResize), new Size(4.800000000000001, 4.800000000000001))
    {
    }

    public bool CreateOleWithDialog(nint winHandler, string dialogTitle, string oleContainerPath)
    {
        bool isNew;
        bool isLink;
        bool isCanceled;
        return base.CreateOleWithDialog(winHandler, dialogTitle, oleContainerPath, out isNew, out isLink, out isCanceled);
    }

    public bool CreateOleWithProgId(string progId, string oleContainerPath)
    {
        return base.CreateOleFromProgId(progId, activate: true, oleContainerPath);
    }

    protected override void PostCreation(bool isNew)
    {
        base.PostCreation(isNew);
        Size size = EstablishInitialSize();
        Width = size.Width;
        Height = size.Height;
        Guid classId = Model.ClassId;
        if (isNew && classId != Guid.Empty && OleUtils.IsSupportedExcelGuid(classId))
        {
            DebugTools.DevAssert(IsOleSiteRunning, null);
            Model.SetVisibleSize(forceRun: false, isPostCreate: true);
        }
        else
        {
            UpdateOleSizeIfNeeded();
        }
    }

    public bool RoundedInitialSizeIsZero()
    {
        Size oleViewportSize = OleViewportSize;
        if (!oleViewportSize.IsEmpty)
        {
            oleViewportSize = OleViewportSize;
            if (Math.Round(oleViewportSize.Width) != 0.0)
            {
                oleViewportSize = OleViewportSize;
                return Math.Round(oleViewportSize.Height) == 0.0;
            }
        }
        return true;
    }

    public override Size EstablishInitialSize()
    {
        Size val = new Size(230.40000000000003, 153.60000000000002);
        Size size = OleViewportSize;
        if (RoundedInitialSizeIsZero())
        {
            OleViewportSize = (size = val);
        }
        if (!AutoResize)
        {
            size = new Size(Math.Min(size.Width, val.Width), Math.Min(size.Height, val.Height));
        }
        return ApplySizeRestrictions(size);
    }

    public IResizeData StoreResizeData(Size oldSize, Size newSize)
    {
        double undoDiagonal = CalculateDiagonalLength(oldSize);
        double redoDiagonal = CalculateDiagonalLength(newSize);
        return new OleObjectResizeData(oldSize, undoDiagonal, redoDiagonal);
    }

    public void RestoreResizeData(IResizeData resizeData)
    {
        OleObjectResizeData oleObjectResizeData = resizeData as OleObjectResizeData;
        if (DebugTools.AssertNotNull(oleObjectResizeData, null).Failed)
        {
            return;
        }

        Size oldSize = oleObjectResizeData.OldSize;
        Size size = CoerceSize(oldSize, oleObjectResizeData.DiagonalForUndo);
        if (size.IsEmpty)
        {
            return;
        }
        Width = size.Width;
        Height = size.Height;
    }

    public Size CoerceNewSize(Size newSize, IResizeData resizeData)
    {
        OleObjectResizeData oleObjectResizeData = resizeData as OleObjectResizeData;
        if (DebugTools.AssertNotNull(oleObjectResizeData, null).Failed)
        {
            return newSize;
        }
        Size coercedSize = CoerceSize(newSize, oleObjectResizeData.DiagonalForRedo);
        if (coercedSize.IsEmpty)
        {
            return newSize;
        }
        return coercedSize;
    }
}
