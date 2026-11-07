using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using Quire;
using Quire.Controls.Core;
using Quire.Controls.Internal;

namespace Quire.Controls.Include;

public partial class ErrorWarningHandler
{
    private readonly IncludeWorksheetControl _control;
    private readonly ErrorsList _errorsList = new ErrorsList();
    private readonly WarningsList _warningsList = new WarningsList();
    internal List<NodeMessageAdorner> AdornersList;

    public ErrorWarningHandler(IncludeWorksheetControl control)
    {
        _control = control;
        AdornersList = new List<NodeMessageAdorner>();
    }

    public void ShowErrors(string path, ErrorsList list)
    {
        _errorsList.RemoveAll((ErrorOnIncludeWorksheet item) => item.PathIndexes.Equals(path));
        _errorsList.AddRange(list);
        if (_errorsList == null || _errorsList.Count == 0)
        {
            RemoveAdorners(NodeMessageAdornerType.Error);
        }
        else
        {
            AddErrorAdorners();
        }
    }

    public void ShowWarnings(string path, WarningsList list)
    {
        _warningsList.RemoveAll((WarningOnIncludeWorksheet item) => item.PathIndexes.Equals(path));
        _warningsList.AddRange(list);
        if (_warningsList == null || _warningsList.Count == 0)
        {
            RemoveAdorners(NodeMessageAdornerType.Warning);
        }
        else
        {
            AddWarningAdorners();
        }
    }

    private void RemoveAdorners(NodeMessageAdornerType type)
    {
        if (_control == null || !_control.IsLoaded)
        {
            return;
        }
        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_control);
        if (adornerLayer != null)
        {
            Adorner[] adorners = adornerLayer.GetAdorners(_control);
            if (adorners != null)
            {
                if (type == NodeMessageAdornerType.Error)
                {
                    RubberBandHighlighter rubberBandHighlighter = adorners.OfType<RubberBandHighlighter>().FirstOrDefault();
                    if (rubberBandHighlighter != null)
                    {
                        rubberBandHighlighter.Remove();
                    }
                }
                else if (type == NodeMessageAdornerType.Warning)
                {
                    Adorner adorner = adorners.FirstOrDefault((Adorner adorner2) => adorner2 is ExclamationPointAdorner);
                    if (adorner != null)
                    {
                        adornerLayer.Remove(adorner);
                    }
                }
            }
            RemoveNodeMessageAdorners(type);
        }
    }

    private void RemoveNodeMessageAdorners(NodeMessageAdornerType type)
    {
        AdornersList.Where((NodeMessageAdorner item) => item.Type == type).Each((NodeMessageAdorner item) =>
        {
            item.Remove();
        });
        AdornersList.RemoveAll((NodeMessageAdorner item) => item.Type == type);
    }

    private bool AddAdorners(NodeMessageAdornerType type)
    {
        if (_control == null || !_control.IsLoaded)
        {
            return false;
        }
        AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(_control);
        if (adornerLayer != null)
        {
            IList list;
            IList list2;
            list = (type == NodeMessageAdornerType.Error) ? (list2 = _errorsList) : (list2 = _warningsList);
            RemoveAdorners(type);
            if (list != null && list.Count > 0)
            {
                if (type == NodeMessageAdornerType.Error)
                {
                    SetErrorFrame(_control);
                }
                else
                {
                    adornerLayer.Add(new ExclamationPointAdorner(_control));
                }
                foreach (IncludeWorksheetErrorWarningData item in list)
                {
                    AdornersList.Add(new NodeMessageAdorner(_control, item.Text, type));
                }
            }
            return true;
        }
        return false;
    }

    private void AddErrorAdorners()
    {
        if (_control.IsLoaded)
        {
            ElementIsLoadedAddErrorAdorners(_control, null);
        }
        else
        {
            _control.Loaded += ElementIsLoadedAddErrorAdorners;
        }
    }

    private static void ElementIsLoadedAddErrorAdorners(object sender, RoutedEventArgs e)
    {
        IErrorWarningOwner errorWarningOwner = sender as IErrorWarningOwner;
        FrameworkElement frameworkElement = sender as FrameworkElement;
        if (errorWarningOwner != null && frameworkElement != null && errorWarningOwner.ErrorWarningHandler.AddAdorners(NodeMessageAdornerType.Error))
        {
            frameworkElement.Loaded -= ElementIsLoadedAddErrorAdorners;
        }
    }

    private static void SetErrorFrame(UIElement element)
    {
        RubberBandHighlighter rubberBandHighlighter = new RubberBandHighlighter(element, new List<UIElement>())
        {
            Color = Colors.Transparent,
            CornerStyle = HighlightCornerStyle.RoundedWithBorder
        };
    }

    private void AddWarningAdorners()
    {
        if (_control.IsLoaded)
        {
            ElementIsLoadedAddWarningAdorners(_control, null);
        }
        else
        {
            _control.Loaded += ElementIsLoadedAddWarningAdorners;
        }
    }

    private static void ElementIsLoadedAddWarningAdorners(object sender, RoutedEventArgs e)
    {
        IErrorWarningOwner errorWarningOwner = sender as IErrorWarningOwner;
        FrameworkElement frameworkElement = sender as FrameworkElement;
        if (errorWarningOwner != null && frameworkElement != null && errorWarningOwner.ErrorWarningHandler.AddAdorners(NodeMessageAdornerType.Warning))
        {
            frameworkElement.Loaded -= ElementIsLoadedAddWarningAdorners;
        }
    }

    public WarningsList GetMergedWarningsList()
    {
        return _warningsList;
    }

    public ErrorsList GetMergedErrorsList()
    {
        return _errorsList;
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Include.ErrorWarningHandler+ExclamationPointAdorner, Ptc.Controls.Include")]
    public partial class ExclamationPointAdorner : Adorner
    {
        private readonly Pen _pen = new Pen(Brushes.Green, 2.0);
        private readonly double _height;

        public ExclamationPointAdorner(FrameworkElement element)
            : base(element)
        {
            _height = element.ActualHeight;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawLine(_pen, new Point(-10.0, 0.0), new Point(-10.0, _height - 7.0));
            drawingContext.DrawEllipse(Brushes.Green, _pen, new Point(-10.0, _height - 2.0), 1.0, 1.0);
        }
    }
}
