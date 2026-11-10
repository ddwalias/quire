#nullable disable

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Quire.Controls.Core.Dialogs;
using Quire.Wpf;

namespace Quire.MessageBoxes;

public partial class MessageDialogWithProgressBar
{
    public string ProgressBarMessageFormat { get; set; }

    protected override TextBlock Message => _message;

    protected override Image Image => null;

    protected override ItemsControl Buttons => _buttons;

    public static SaveAndSyncDialogResult Show(Window ownerWindow, string message, string title, ProgressBarData progressBarData)
    {
        MessageDialogWithProgressBar messageDialogWithProgressBar = new MessageDialogWithProgressBar(ownerWindow, title, progressBarData);
        messageDialogWithProgressBar.AddButton("SaveWithoutSync", SaveAndSyncDialogResult.SaveWithoutSync);
        messageDialogWithProgressBar.AddButton("Cancel", SaveAndSyncDialogResult.Cancel);
        messageDialogWithProgressBar.SetMessageImageAndButtons(message, MessageBoxImage.None, "SaveWithoutSync", "Cancel");
        if (progressBarData != null && !progressBarData.ProgressNotifier.IsTrackingInProgress)
        {
            progressBarData.ProgressNotifier.StartProgressTracking();
        }

        if (progressBarData == null || progressBarData.ProgressNotifier.IsTrackingInProgress)
        {
            messageDialogWithProgressBar.ShowDialog();
        }

        if (!DebugTools.AssertNotNull(messageDialogWithProgressBar.Result, null).Failed)
        {
            return messageDialogWithProgressBar.Result.Value;
        }

        return SaveAndSyncDialogResult.None;
    }

    private void HandleProgressStopped(bool finished)
    {
        SetDialogResultAndClose(SaveAndSyncDialogResult.SaveAndSync);
    }

    private void HandleProgressValueChanged(object sender, ProgressChangedEventArgs progressArgs)
    {
        _progressBar.Value = progressArgs.ProgressPercentage;
        if (!string.IsNullOrEmpty(ProgressBarMessageFormat))
        {
            _progressBarMessage.Text = string.Format(ProgressBarMessageFormat, progressArgs.ProgressPercentage);
        }
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        if (_progressBarProgressNotifier != null)
        {
            _progressBarProgressNotifier.ProgressChanged -= HandleProgressValueChanged;
            _progressBarProgressNotifier.ProgressStopped -= HandleProgressStopped;
        }
    }
}
