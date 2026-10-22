#nullable disable
#pragma warning disable

namespace Quire.Wpf;

partial interface IMessageBoxManager
{
    bool IsDisabled { get; set; }
    global::System.Action<string> ShowExceptionMessage { get; set; }
    global::System.Func<global::System.Windows.Window, string, string, global::System.Windows.MessageBoxResult> ShowCriticalErrorMessage { get; set; }
    global::System.Func<global::System.Windows.Window, string, string, global::System.Windows.MessageBoxResult> ShowCriticalWarningMessage { get; set; }
    global::System.Func<global::System.Windows.Window, string, string, global::System.Windows.MessageBoxButton, global::System.Windows.MessageBoxResult> ShowCriticalQuestionMessage { get; set; }
    global::System.Windows.MessageBoxResult ShowWarning(global::System.Windows.Window window, string msg, string title) ;
    global::System.Windows.MessageBoxResult ShowQuestion(global::System.Windows.Window window, string msg, string title, global::System.Windows.MessageBoxButton button) ;
    global::System.Windows.MessageBoxResult ShowError(global::System.Windows.Window window, string msg, string title) ;
    global::System.Windows.MessageBoxResult ShowInfo(global::System.Windows.Window window, string msg, string title) ;
    global::System.Windows.MessageBoxResult ShowWarning(string msg, string title) ;
    global::System.Windows.MessageBoxResult ShowQuestion(string msg, string title, global::System.Windows.MessageBoxButton button) ;
    global::System.Windows.MessageBoxResult ShowError(string msg, string title) ;
    global::System.Windows.MessageBoxResult ShowInfo(string msg, string title) ;
    int ShowQuestionWithOptions(global::System.Windows.Window window, string prompt, string title, global::System.Windows.MessageBoxImage icon, global::Quire.Wpf.CheckBoxData[] checkBoxesData, string[] buttonsKey, string defaultButtonKey, string cancelButtonKey) ;
    global::System.Windows.MessageBoxResult ShowOkDialog(global::System.Windows.Window ownerWindow, string message, string caption, global::System.Windows.MessageBoxImage imageName) ;
}
