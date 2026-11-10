#nullable disable

using System;
using System.Windows;
using Quire.Wpf;

namespace Quire.MessageBoxes;

public partial class MessageBoxManager
{
    public bool IsDisabled { get; set; }

    private MessageBoxResult Show(Window window, string format, string title, MessageBoxButton button, MessageBoxImage icon)
    {
        return MessageBox.Show(window, format, title, button, icon);
    }

    private MessageBoxResult Show(Window window, string format, string title, MessageBoxImage icon)
    {
        return MessageBox.Show(window, format, title, MessageBoxButton.OK, icon);
    }

    private MessageBoxResult Show(string format, string title, MessageBoxButton button, MessageBoxImage icon)
    {
        return MessageBox.Show(format, title, button, icon);
    }

    private MessageBoxResult Show(string format, string title, MessageBoxImage icon)
    {
        return MessageBox.Show(format, title, MessageBoxButton.OK, icon);
    }

    public MessageBoxResult ShowWarning(Window parent, string msg, string title)
    {
        if (!IsDisabled)
        {
            return Show(parent, msg, title, MessageBoxImage.Exclamation);
        }

        return MessageBoxResult.None;
    }

    public MessageBoxResult ShowQuestion(Window parent, string msg, string title, MessageBoxButton button)
    {
        if (!IsDisabled)
        {
            return Show(parent, msg, title, button, MessageBoxImage.Exclamation);
        }

        return MessageBoxResult.None;
    }

    public MessageBoxResult ShowError(Window parent, string msg, string title)
    {
        return Show(parent, msg, title, MessageBoxImage.Hand);
    }

    public MessageBoxResult ShowInfo(Window parent, string msg, string title)
    {
        return Show(parent, msg, title, MessageBoxImage.None);
    }

    public MessageBoxResult ShowWarning(string msg, string title)
    {
        return Show(msg, title, MessageBoxImage.Exclamation);
    }

    public MessageBoxResult ShowQuestion(string msg, string title, MessageBoxButton button)
    {
        return Show(msg, title, button, MessageBoxImage.Exclamation);
    }

    public MessageBoxResult ShowError(string msg, string title)
    {
        return Show(msg, title, MessageBoxImage.Hand);
    }

    public MessageBoxResult ShowInfo(string msg, string title)
    {
        return Show(msg, title, MessageBoxImage.None);
    }

    public int ShowQuestionWithOptions(Window parent, string prompt, string title, MessageBoxImage imageName, CheckBoxData[] checkBoxesData, string[] buttons, string defaultButtonIdx, string cancelButtonIdx)
    {
        return MessageDialogWithOptions.Show(parent, prompt, title, imageName, checkBoxesData, buttons, defaultButtonIdx, cancelButtonIdx);
    }

    public MessageBoxResult ShowOkDialog(Window ownerWindow, string message, string caption, MessageBoxImage imageName)
    {
        if (!IsDisabled)
        {
            return MessageDialogWithOptions.ShowOkDialog(ownerWindow, message, caption, imageName);
        }

        return MessageBoxResult.None;
    }

    public Action<string> ShowExceptionMessage { get; set; }

    public Func<Window, string, string, MessageBoxResult> ShowCriticalErrorMessage { get; set; }

    public Func<Window, string, string, MessageBoxResult> ShowCriticalWarningMessage { get; set; }

    public Func<Window, string, string, MessageBoxButton, MessageBoxResult> ShowCriticalQuestionMessage { get; set; }

    public bool IsConnected { get; set; }

    public MessageBoxManager()
    {
    }
}
