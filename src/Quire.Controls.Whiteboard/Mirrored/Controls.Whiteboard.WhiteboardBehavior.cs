using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.Text;
using Quire.Services;
using Quire.Wpf;

namespace Quire.Controls.Whiteboard;

public static partial class WhiteboardBehavior
{
    public static DependencyProperty UIEventsHandlerProperty = DependencyProperty.RegisterAttached(
        "UIEventsHandler",
        typeof(IWhiteboardViewModel),
        typeof(WhiteboardControl),
        new PropertyMetadata((object)null, (PropertyChangedCallback)OnUIEventsHandlerChanged));

    private static IWhiteboardItemManager _itemManager;

    public static void SetUIEventsHandler(UIElement element, IWhiteboardViewModel value)
    {
        if (element != null)
        {
            element.SetValue(UIEventsHandlerProperty, value);
        }
    }

    public static IWhiteboardViewModel GetUIEventsHandler(UIElement element)
    {
        if (element != null)
        {
            return (IWhiteboardViewModel)element.GetValue(UIEventsHandlerProperty);
        }
        return null;
    }

    public static IWhiteboardItemManager ItemManager
    {
        get
        {
            if (_itemManager == null || !_itemManager.IsConnected)
            {
                return _itemManager = AppServiceRegistry.GetService<IWhiteboardItemManager>();
            }
            return _itemManager;
        }
    }

    private static void OnUIEventsHandlerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        FrameworkElement element = d as FrameworkElement;
        if (element == null)
        {
            return;
        }
        IWhiteboardViewModel viewModel = e.NewValue as IWhiteboardViewModel;
        if (viewModel == null)
        {
            return;
        }
        element.SizeChanged += (object sender, SizeChangedEventArgs eventArgs) =>
        {
            IWhiteboardViewModel whiteboardViewModel = viewModel;
            Size newSize = eventArgs.NewSize;
            whiteboardViewModel.ContentWidth = newSize.Width;
            IWhiteboardViewModel whiteboardViewModel2 = viewModel;
            newSize = eventArgs.NewSize;
            whiteboardViewModel2.ContentHeight = newSize.Height;
        };
        element.GotKeyboardFocus += (object sender, KeyboardFocusChangedEventArgs eventArgs) =>
        {
            // IL requires goto to match the beq/ret/call/ret shape
            if (eventArgs.NewFocus == element)
            {
                goto HandleFocus;
            }
            return;
        HandleFocus:
            ImeHandler.HandleImeFocus();
        };
        element.KeyDown += (object sender, KeyEventArgs eventArgs) =>
        {
            if (eventArgs.OriginalSource == element)
            {
                switch (eventArgs.ImeProcessedKey)
                {
                    case Key.ImeProcessed:
                        break;
                }
            }
        };
        if (viewModel.HandleUserInput)
        {
            element.InputBindings.Add(new KeyBinding(EditingCommands.MoveDownByLine, Gestures.DownArrow));
            element.InputBindings.Add(new KeyBinding(EditingCommands.MoveUpByLine, Gestures.UpArrow));
            element.InputBindings.Add(new KeyBinding(EditingCommands.MoveLeftByCharacter, Gestures.LeftArrow));
            element.InputBindings.Add(new KeyBinding(EditingCommands.MoveRightByCharacter, Gestures.RightArrow));
            element.InputBindings.Add(new KeyBinding(EditingCommands.EnterParagraphBreak, Gestures.Enter));
            HookupMouseEvents(element, viewModel);
            element.AddHandler(RegionBorder.RequestMoveAndResizeEvent, (RegionBorder.RequestMoveAndResizeHandler)((object sender, ResizeEventMouseArgs eventArgs) =>
            {
                viewModel.OnRequestMoveAndResize(sender as IWhiteboardControl, eventArgs);
            }));
            element.AddHandler(RichTextBase.CaretLeavesTextAreaEvent, (CommonRoutedEvents.CaretLeavesTextRegionEventHandler)((object sender, CommonRoutedEvents.CaretLeavesTextRegionEventArgs eventArgs) =>
            {
                viewModel.HandleCaretLeavesTextArea(eventArgs);
            }));
            element.AddHandler(RichTextWithEmbeddedItem.TabOutOfTextEvent, (CustomRoutedEventHandler<FocusNavigationDirection>)((object sender, CustomRoutedEventArgs<FocusNavigationDirection> eventArgs) =>
            {
                viewModel.HandleTabOutOfText(eventArgs);
            }));
        }
    }

    private static void HookupMouseEvents(UIElement element, IWhiteboardViewModel viewModel)
    {
        void PreviewMouseDownHandler(object sender, MouseButtonEventArgs args)
        {
            Exception ex = null;
            try
            {
                if (args.LeftButton == MouseButtonState.Pressed)
                {
                    viewModel.PreviewMouseLeftButtonDownHandler(sender, args);
                }
                else if (args.RightButton == MouseButtonState.Pressed)
                {
                    viewModel.PreviewMouseRightButtonDownHandler(sender, args);
                }
            }
            catch (Exception ex2)
            {
                ex = ex2;
                DebugTools.FailAssertionDoNotThrow(ex2);
            }
            finally
            {
                if (args.Handled || ex != null)
                {
                    element.RaiseEvent(new CustomRoutedEventArgs<MouseButtonEventArgs>(CommonRoutedEvents.BlockedPreviewMouseDownEvent, args));
                }
            }
        }

        element.PreviewMouseLeftButtonDown += PreviewMouseDownHandler;
        element.PreviewMouseRightButtonDown += PreviewMouseDownHandler;
        element.PreviewMouseLeftButtonUp += viewModel.PreviewMouseLeftButtonUpHandler;
        element.PreviewMouseRightButtonUp += viewModel.PreviewMouseRightButtonUpHandler;
        element.MouseDown += (object sender, MouseButtonEventArgs eventArgs) =>
        {
            Point position = eventArgs.GetPosition((IInputElement)sender);
            eventArgs.Handled = viewModel.HandleMouseDown(sender as IWhiteboardControl, position, eventArgs.ChangedButton);
        };
        element.MouseUp += (object sender, MouseButtonEventArgs eventArgs) =>
        {
            Point position = eventArgs.GetPosition((IInputElement)sender);
            eventArgs.Handled = viewModel.HandleMouseUp(sender as IWhiteboardControl, position, eventArgs.ChangedButton);
        };
    }
}
