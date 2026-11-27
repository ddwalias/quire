#nullable disable

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Collections.Generic;
using Quire.Wpf;

namespace Quire.MessageBoxes;

public partial class MessageDialogWithOptions
{
    protected override TextBlock Message => _message;

    protected override Image Image => _image;

    protected override ItemsControl Buttons => _buttons;

    public static int Show(Window ownerWindow, string message, string caption, MessageBoxImage imageName, CheckBoxData[] checkBoxesData, string[] buttonsKey, string defaultButtonKey, string cancelButtonKey)
    {
        MessageDialogWithOptions messageDialogWithOptions = new MessageDialogWithOptions(ownerWindow, caption, checkBoxesData);
        int num = 0;
        foreach (string key in buttonsKey)
        {
            messageDialogWithOptions.AddButton(key, num);
            num++;
        }

        messageDialogWithOptions.Topmost = true;
        messageDialogWithOptions.SetMessageImageAndButtons(message, imageName, defaultButtonKey, cancelButtonKey);
        messageDialogWithOptions.ShowDialog();
        if (!DebugTools.AssertNotNull(messageDialogWithOptions.Result, null).Failed)
        {
            return messageDialogWithOptions.Result.Value;
        }

        return -1;
    }

    public static MessageBoxResult ShowOkDialog(Window ownerWindow, string message, string caption, MessageBoxImage imageName)
    {
        string[] buttonsKey = new string[1] { "Ok" };
        if (Show(ownerWindow, message, caption, imageName, null, buttonsKey, "Ok", "Ok") != -1)
        {
            return MessageBoxResult.OK;
        }

        return MessageBoxResult.None;
    }

    private MessageDialogWithOptions(Window ownerWindow, string caption, IEnumerable<CheckBoxData> checkBoxesData)
        : base(ownerWindow, caption)
    {
        InitializeComponent();
        if (checkBoxesData != null)
        {
            _checkBoxesPanel.Visibility = Visibility.Visible;
            foreach (CheckBoxData checkBoxesDatum in checkBoxesData)
            {
                _checkBoxesItems.Items.Add(checkBoxesDatum);
            }
        }
        Mouse.OverrideCursor = Cursors.Arrow;
    }

    [global::System.Diagnostics.DebuggerNonUserCode]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        global::System.Uri resourceLocator = new global::System.Uri("/Quire;component/messageboxes/messagedialogwithoptions.xaml", global::System.UriKind.Relative);
        global::System.Windows.Application.LoadComponent(this, resourceLocator);
    }

    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                _image = (Image)target;
                return;
            case 2:
                _message = (TextBlock)target;
                return;
            case 3:
                _checkBoxesPanel = (StackPanel)target;
                return;
            case 4:
                _checkBoxesItems = (ItemsControl)target;
                return;
            case 5:
                _buttons = (ItemsControl)target;
                return;
        }
        _contentLoaded = true;
    }

    void global::System.Windows.Markup.IStyleConnector.Connect(int connectionId, object target)
    {
        if (connectionId == 6)
        {
            EventSetter eventSetter = new EventSetter();
            eventSetter.Event = ButtonBase.ClickEvent;
            eventSetter.Handler = new RoutedEventHandler(OnButtonClick);
            ((Style)target).Setters.Add(eventSetter);
        }
    }
}
