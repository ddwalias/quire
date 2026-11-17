using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.Whiteboard;
using Quire.Core;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

public partial class WorksheetPageBody : WhiteboardControl, IWorksheetPageBody, IWorksheetPageSection, IDisposable, IWhiteboardControl, IWhiteboardItemContainer, IHasRegionsWithConverterAnnotations, IMathFormatDeserializationProvider, IMaybeHasPointThatSnapsToPixel, IHasChildArrangeInfoProvider, IMathFormatOwner, INotifyPropertyChanged
{
    public static readonly DependencyProperty WaterMarkProperty;

    public static readonly DependencyProperty LabeledIdFormatProperty;

    private readonly PageBodyCanvas _canvas;

    public Brush WaterMark
    {
        get => (Brush)GetValue(WaterMarkProperty);
        set => SetValue(WaterMarkProperty, value);
    }

    public IObservableRangeCollection<IWhiteboardItemZombie> Zombies => _canvas.Zombies;

    Geometry IWorksheetPageBody.Clip
    {
        get => Clip;
        set => Clip = value;
    }

    public event PropertyChangedEventHandler PropertyChanged;

    static WorksheetPageBody()
    {
        WaterMarkProperty = DependencyProperty.Register("WaterMark", typeof(Brush), typeof(WorksheetPageBody), new PropertyMetadata(Brushes.Transparent));
        LabeledIdFormatProperty = AttachedProperties.LabeledIdFormatProperty.AddOwner(typeof(WorksheetPageBody), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Inherits, HandleLabelIdChanged));
    }

    public WorksheetPageBody(Size? gridCellSize, bool isPrinting)
        : base(new PageBodyViewModel(gridCellSize))
    {
        _canvas = new PageBodyCanvas();
        if (isPrinting)
        {
            Content = new WhiteboardCanvas(DataContext as IWhiteboardViewModel);
        }
        else
        {
            Content = _canvas;
        }
        MathFormatDobjController mathFormatDobjController = new MathFormatDobjController((DependencyObject)Content);
        MathFormatDobjController mathFormat = new MathFormatDobjController(this);
        mathFormatDobjController.SetPropertiesFrom(mathFormat);
        ((DependencyObject)Content).SetValue(LabeledIdFormatProperty, ((DependencyObject)this).GetValue(LabeledIdFormatProperty));
        ViewModel.WhiteboardItems.RangeChanged += HandleItemsCollectionChanged;
        SetupInputGestures();
    }


    private void HandleItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
    {
        switch (args.Action)
        {
            case NotifyCollectionChangedAction.Add:
                Zombies.AddRange(args.NewItems.OfType<IWhiteboardItemZombie>());
                break;
            case NotifyCollectionChangedAction.Remove:
                Zombies.RemoveRange(args.OldItems.OfType<IWhiteboardItemZombie>());
                break;
            case NotifyCollectionChangedAction.Replace:
                Zombies.RemoveRange(args.OldItems.OfType<IWhiteboardItemZombie>());
                Zombies.AddRange(args.NewItems.OfType<IWhiteboardItemZombie>());
                break;
            case NotifyCollectionChangedAction.Reset:
                Zombies.Clear();
                Zombies.AddRange(WhiteboardItems.OfType<IWhiteboardItemZombie>());
                break;
            case NotifyCollectionChangedAction.Move:
                break;
        }
    }

    private void SetupInputGestures()
    {
        InputBindings.Add(new KeyBinding(EditingCommands.MoveDownByPage, Gestures.PageDown));
        InputBindings.Add(new KeyBinding(EditingCommands.MoveUpByPage, Gestures.PageUp));
        InputBindings.Add(new KeyBinding(EditingCommands.MoveDownByLine, Gestures.DownArrow));
        InputBindings.Add(new KeyBinding(EditingCommands.MoveUpByLine, Gestures.UpArrow));
        InputBindings.Add(new KeyBinding(EditingCommands.MoveLeftByCharacter, Gestures.LeftArrow));
        InputBindings.Add(new KeyBinding(EditingCommands.MoveRightByCharacter, Gestures.RightArrow));
        InputBindings.Add(new KeyBinding(EditingCommands.EnterParagraphBreak, Gestures.Enter));
    }

    public double GetItemHeight(IWhiteboardItem item)
    {
        Size itemDesiredSize = ViewModel.GetItemDesiredSize(item);
        return itemDesiredSize.Height;
    }

    public void HandleMathFormatChanged(DependencyPropertyChangedEventArgs e)
    {
        ((DependencyObject)Content).SetValue(e.Property, e.NewValue);
    }

    private static void HandleLabelIdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is WorksheetPageBody worksheetPageBody)
        {
            ((DependencyObject)worksheetPageBody.Content).SetValue(e.Property, e.NewValue);
        }
    }

    public bool IsAssociatedWithPage(IWorksheetPage worksheetPage)
    {
        if (worksheetPage == null)
        {
            return false;
        }

        return worksheetPage.PageBody == this;
    }
}
