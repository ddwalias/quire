#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Quire;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core.Dialogs;

public abstract partial class MessageDialogBase<T> : Window where T : struct
{
    private readonly Dictionary<string, Button> _buttonsDict = new Dictionary<string, Button>();

    [MirrorVerified("PtcControlsCoreDialogsMessageDialogBase1MechanicalDifferentialTests")]
    private Button CancelButton { get; set; }

    [MirrorVerified("PtcControlsCoreDialogsMessageDialogBase1MechanicalDifferentialTests")]
    protected Nullable<T> Result { get; set; }

    protected MessageDialogBase(Window ownerWindow, string caption)
    {
        if (ownerWindow != null)
        {
            Owner = ownerWindow;
        }
        Title = caption;
    }

    protected void SetMessageImageAndButtons(string messageText, MessageBoxImage imageName, string defaultButtonKey, string cancelButtonKey)
    {
        if (Message != null)
        {
            Message.Text = messageText;
        }
        else
        {
            DebugTools.DevAssert(messageText.IsNullOrEmpty(), null);
        }
        if (Image != null)
        {
            if (imageName != MessageBoxImage.None)
            {
                Image.Visibility = Visibility.Visible;
                Image.Source = ImageUtils.GetImageSourceFromIcon(ImageUtils.MessageBoxImageToIcon(imageName));
            }
        }
        else
        {
            DebugTools.DevAssert(imageName == MessageBoxImage.None, null);
        }
        if (DebugTools.AssertNotNull(_buttonsDict, "MessageDialogWithOptions without buttons is not supported").Passed)
        {
            if (defaultButtonKey != null)
            {
                Button value;
                if (_buttonsDict.TryGetValue(defaultButtonKey, out value))
                {
                    value.IsDefault = true;
                }
                else
                {
                    DebugTools.FailAssertion("MessageDialogWithOptions default button key must be null or in range of buttonsData");
                }
            }
            if (cancelButtonKey != null)
            {
                Button value2;
                if (_buttonsDict.TryGetValue(cancelButtonKey, out value2))
                {
                    value2.IsCancel = true;
                }
                else
                {
                    DebugTools.FailAssertion("MessageDialogWithOptions cancel button key must be null or in range of buttonsData");
                }
                CancelButton = value2;
            }
        }
        Result = null;
    }

    protected void SetDialogResultAndClose(T result)
    {
        Result = result;
        Close();
    }

    protected void OnButtonClick(object sender, RoutedEventArgs e)
    {
        Button button = sender as Button;
        if (button == null)
        {
            return;
        }
        DialogResult = button.IsDefault;
        SetDialogResultAndClose(GetButtonValue(button));
    }

    protected Button AddButton(string key, T result)
    {
        Button button = new Button
        {
            Content = FindResource(key)
        };
        SetButtonValue(button, result);
        _buttonsDict.Add(key, button);
        Buttons.Items.Add(button);
        return button;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        if (!Result.HasValue && CancelButton != null)
        {
            Result = GetButtonValue(CancelButton);
        }
    }

    private static void SetButtonValue(Button button, T result)
    {
        button.Tag = result;
    }

    private static T GetButtonValue(Button button)
    {
        return (T)button.Tag;
    }
}
