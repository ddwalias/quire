#nullable disable
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace Quire.Controls.Core;

public abstract partial class CopyWidgetBase : Popup, IDisposable
{
    public abstract double ParentWidth { get; set; }

    public abstract double IncludeWidth { get; set; }

    public abstract Border ChildBorder { get; }

    public void Dispose()
    {
        try
        {
            IsOpen = false;
            HwndSource hwndSource = PresentationSource.FromVisual(Child) as HwndSource;
            if (DebugTools.AssertNotNullDoNotThrow(hwndSource, null).Passed && !hwndSource.IsDisposed)
            {
                hwndSource.Dispose();
            }
        }
        catch (Exception ex)
        {
            DebugTools.FailAssertionDoNotThrow(ex);
        }
    }

    protected CopyWidgetBase()
    {
    }
}
