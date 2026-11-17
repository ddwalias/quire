#nullable disable
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls.Worksheet;

public partial class WorksheetPageBreak : Control, IWorksheetPageBreak, IPersistentDataProvider, IComponentConnector
{
    private double _verticalPosition;

    public const double PageBreakHeight = 2.0;

    private bool _contentLoaded;

    public double VerticalPosition
    {
        private get => _verticalPosition;
        set
        {
            _verticalPosition = value;
            RenderTransform = new TranslateTransform(0.0, VerticalPosition);
        }
    }

    public int NextPage { get; set; }

    public WorksheetPageBreak()
    {
        InitializeComponent();
        Height = 2.0;
    }

    public IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, object data)
    {
        return null;
    }

    public object GetRegionPersistentData(IS11NProvider s11NProvider)
    {
        return new worksheetRegionTypePageBreak();
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Worksheet;component/worksheetpagebreak.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
