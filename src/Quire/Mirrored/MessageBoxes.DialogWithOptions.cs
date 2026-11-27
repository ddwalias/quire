#nullable disable

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;

namespace Quire.MessageBoxes;

public partial class DialogWithOptions
{
    protected override TextBlock Message => null;

    protected override Image Image => null;

    protected override ItemsControl Buttons => _buttons;

    public DialogWithOptions(Window ownerWindow, string caption, FrameworkElement frameworkElement, string[] buttons)
        : base(ownerWindow, caption)
    {
        InitializeComponent();
        if (frameworkElement != null)
        {
            Grid obj = Content as Grid;
            Grid.SetColumn(frameworkElement, 0);
            Grid.SetRow(frameworkElement, 0);
            obj?.Children.Add(frameworkElement);
            DataContext = frameworkElement.DataContext;
        }
        for (int num = 0; num < buttons.Length; num++)
        {
            Button button = AddButton(buttons[num], num);
            if (buttons[num] == "Ok")
            {
                button.IsDefault = true;
                Binding binding = new Binding("OkIsEnabled");
                binding.Mode = BindingMode.OneWay;
                binding.Source = DataContext;
                button.SetBinding(UIElement.IsEnabledProperty, binding);
            }
            else if (buttons[num] == "Cancel")
            {
                button.IsCancel = true;
            }
        }
    }

    [global::System.Diagnostics.DebuggerNonUserCode]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        global::System.Uri resourceLocator = new global::System.Uri("/Quire;component/messageboxes/dialogwithoptions.xaml", global::System.UriKind.Relative);
        global::System.Windows.Application.LoadComponent(this, resourceLocator);
    }

    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
    {
        if (connectionId == 1)
        {
            _buttons = (ItemsControl)target;
            return;
        }
        _contentLoaded = true;
    }

    void global::System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target)
    {
        if (connectionId == 2)
        {
            EventSetter eventSetter = new EventSetter();
            eventSetter.Event = ButtonBase.ClickEvent;
            eventSetter.Handler = new RoutedEventHandler(OnButtonClick);
            ((Style)target).Setters.Add(eventSetter);
        }
    }
}
