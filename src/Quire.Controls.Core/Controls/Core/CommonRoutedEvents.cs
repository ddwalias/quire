#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class CommonRoutedEvents
{
    [MirrorOf("Ptc.Controls.Core.CommonRoutedEvents+CaretLeavesTextRegionEventHandler, Ptc.Controls.Core")]
    public delegate void CaretLeavesTextRegionEventHandler(object sender, CaretLeavesTextRegionEventArgs args);

    [MirrorOf("Ptc.Controls.Core.CommonRoutedEvents+CaretLeavesTextRegionEventArgs, Ptc.Controls.Core")]
    public partial class CaretLeavesTextRegionEventArgs : RoutedEventArgs
    {
        [MirrorOf("Ptc.Controls.Core.CommonRoutedEvents+CaretLeavesTextRegionEventArgs+TextAreaLeavingDirection, Ptc.Controls.Core")]
        public enum TextAreaLeavingDirection
        {
            Up = -1,
            Down = 1,
            Left = 2,
            Right = 3
        }

        public TextAreaLeavingDirection LeavingDirection { get; set; }

        public CaretLeavesTextRegionEventArgs(RoutedEvent routedEvent)
        {
            RoutedEvent = routedEvent;
        }
    }

    private static readonly Type _typeOfThis = typeof(CommonRoutedEvents);

    public static readonly RoutedEvent ContentChangedEvent = EventManager.RegisterRoutedEvent("ContentChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), _typeOfThis);

    public static readonly RoutedEvent TraversedFocusEvent = EventManager.RegisterRoutedEvent("TraversedFocus", RoutingStrategy.Direct, typeof(CustomRoutedEventHandler<FocusNavigationDirection>), _typeOfThis);

    public static readonly RoutedEvent CopyWhiteboardItemsEvent = EventManager.RegisterRoutedEvent("CopyWhiteboardItems", RoutingStrategy.Bubble, typeof(CustomRoutedEventHandler<IEnumerable<IWhiteboardItem>>), _typeOfThis);

    public static readonly RoutedEvent CopySingleItemNoImageEvent = EventManager.RegisterRoutedEvent("CopySingleItemNoImage", RoutingStrategy.Bubble, typeof(CustomRoutedEventHandler<IWhiteboardItem>), _typeOfThis);

    public static readonly RoutedEvent CaretLeavesTextAreaEvent = EventManager.RegisterRoutedEvent("CaretLeavesTextArea", RoutingStrategy.Bubble, typeof(CaretLeavesTextRegionEventHandler), _typeOfThis);

    public static readonly RoutedEvent RequestRestoreInsertionPointEvent = EventManager.RegisterRoutedEvent("RequestRestoreInsertionPoint", RoutingStrategy.Bubble, typeof(RoutedEventHandler), _typeOfThis);

    public static readonly RoutedEvent BlockedPreviewMouseDownEvent = EventManager.RegisterRoutedEvent("BlockedPreviewMouseDown", RoutingStrategy.Bubble, typeof(CustomRoutedEventHandler<MouseButtonEventArgs>), _typeOfThis);

    public static readonly RoutedEvent WasMeasuredEvent = EventManager.RegisterRoutedEvent("WasMeasured", RoutingStrategy.Direct, typeof(RoutedEventArgs), _typeOfThis);
}
