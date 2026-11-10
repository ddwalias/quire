#nullable disable

using System.Windows;
using System.Windows.Controls;
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
}
