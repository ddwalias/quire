#nullable disable
using System;
using System.ComponentModel;
using System.Windows;
using Quire;
using Quire.Core;
using Quire.PersistentData;

namespace Quire.Controls.Core.Ole;

public partial class OleSizeDataWithNotification : IOleSizeData, INotifyPropertyChanged
{
    public static partial class PropertyNames
    {
        public const string AutoResize = "AutoResize";
        public const string AutomaticSize = "AutomaticSize";
    }

    private bool _autoResize;
    private Size _oleViewportSize = Size.Empty;

    public event PropertyChangedEventHandler PropertyChanged;

    public void CopyFrom(IOleSizeData data, bool notify)
    {
        UpdateAndNotify(() =>
        {
            _autoResize = data.AutoResize;
            _oleViewportSize = data.OleViewportSize;
            HasFakeOleViewportSize = data.HasFakeOleViewportSize;
        }, notify);
    }

    public Size OleViewportSize
    {
        get => _oleViewportSize;
        set
        {
            UpdateAndNotify(() =>
            {
                _oleViewportSize = value;
            }, true);
            HasFakeOleViewportSize = false;
        }
    }

    public bool HasFakeOleViewportSize { get; set; }

    public bool AutoResize
    {
        get => _autoResize;
        set
        {
            UpdateAndNotify(() =>
            {
                _autoResize = value;
            }, true);
        }
    }

    public Size AutomaticSize
    {
        get
        {
            if (!AutoResize)
            {
                return Size.Empty;
            }
            return OleViewportSize;
        }
    }

    public OleSizeDataWithNotification(bool autoResize)
    {
        _autoResize = autoResize;
    }

    private void OnPropertyChanged(string propName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propName));
        }
    }

    private void UpdateAndNotify(Action doUpdate, bool notify)
    {
        bool autoResize = AutoResize;
        Size automaticSize = AutomaticSize;
        doUpdate();
        if (notify)
        {
            if (autoResize != AutoResize)
            {
                OnPropertyChanged("AutoResize");
            }
            if (automaticSize.IsEmpty != AutomaticSize.IsEmpty || (!automaticSize.IsEmpty && !automaticSize.IsEqualTo(AutomaticSize)))
            {
                OnPropertyChanged("AutomaticSize");
            }
        }
        DebugTools.DevAssert(!AutoResize || !OleViewportSize.IsEmpty, null);
    }
}
