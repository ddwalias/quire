#nullable disable
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Quire.Mirror;

namespace Quire.Controls;

public partial class CopyWidget : IComponentConnector
{
    private readonly CopyWidgetViewModel _viewModel = new CopyWidgetViewModel();
    private Border _childBorder;
    private bool _contentLoaded;

    public CopyWidget()
    {
        DataContext = _viewModel;
        InitializeComponent();
    }

    public int TotalPages { get => _viewModel.TotalPages; set => _viewModel.TotalPages = value; }

    public Nullable<DateTime> DisplaySavedDate { get => _viewModel.DisplaySavedDate; set => _viewModel.DisplaySavedDate = value; }

    public string DisplayFileName { get => _viewModel.DisplayFileName; set => _viewModel.DisplayFileName = value; }

    public string DisplayFilePath { get => _viewModel.DisplayFilePath; set => _viewModel.DisplayFilePath = value; }

    public string DisplayDocumentName { get => _viewModel.DisplayDocumentName; set => _viewModel.DisplayDocumentName = value; }

    public int PageNumber { get => _viewModel.PageNumber; set => _viewModel.PageNumber = value; }

    public override double ParentWidth { get => _viewModel.ParentWidth; set => _viewModel.ParentWidth = value; }

    public override double IncludeWidth { get => _viewModel.IncludeWidth; set => _viewModel.IncludeWidth = value; }

    public override Border ChildBorder => _childBorder;

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls;component/copywidget.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        if (connectionId == 1)
        {
            _childBorder = (Border)target;
            return;
        }
        _contentLoaded = true;
    }
}
