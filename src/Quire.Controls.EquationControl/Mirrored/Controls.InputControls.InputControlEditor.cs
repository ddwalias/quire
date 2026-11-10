using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;

namespace Quire.Controls.InputControls;

public partial class InputControlEditor
{
    private readonly IInputControlEditorViewModel _viewModel;

    public InputControlEditor(string editorTitleResourceKey, Control compositePropertyTab, IInputControlEditorModel model)
    {
        _viewModel = new InputControlEditorViewModel(model);
        DataContext = _viewModel;
        InitializeComponent();
        PostInitializeComponent(editorTitleResourceKey, compositePropertyTab, model);
        tbScript.TextChanged += (sender, e) =>
        {
            _viewModel.CanRedo = tbScript.CanRedo;
            _viewModel.CanUndo = tbScript.CanUndo;
        };
    }

    internal TabControl TabControl;
    internal ComboBox cbLanguage;
    internal TextBox tbScript;
    internal Grid gridPropertyEditor;

    private void PostInitializeComponent(string editorTitleResourceKey, Control compositePropertyTab, IInputControlEditorModel model)
    {
        Title = TryFindResource(editorTitleResourceKey) as string;
        _viewModel.IsApplyEnabled = false;
        if (compositePropertyTab != null)
        {
            gridPropertyEditor.Children.Add(compositePropertyTab);
        }
        else
        {
            TabControl.Items.RemoveAt(1);
        }

        if (model is not IInputScriptableControlModel)
        {
            TabControl.Items.RemoveAt(0);
        }
    }

    private void ButtonFileOpen_OnClick(object sender, RoutedEventArgs e)
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filter = "Script Files (*.txt)|*.txt|All files (*.*)|*.*";
        // Release IL reads Nullable.Value here, not GetValueOrDefault
        if (dialog.ShowDialog().Value)
        {
            _ = dialog.FileName;
            StreamReader reader = new StreamReader(dialog.OpenFile());
            try
            {
                string text = reader.ReadToEnd();
                tbScript.Text = text;
            }
            finally
            {
                // Release IL disposes the reader local directly; `using` adds a copy local
                if (reader != null)
                {
                    ((IDisposable)reader).Dispose();
                }
            }
        }
    }

    private void ButtonFileSave_OnClick(object sender, RoutedEventArgs e)
    {
        Stream stream;
        SaveFileDialog dialog = new SaveFileDialog();
        dialog.Filter = "Script Files (*.txt)|*.txt|All files (*.*)|*.*";
        // Release IL reads Nullable.Value here, not GetValueOrDefault
        if (dialog.ShowDialog().Value)
        {
            _ = dialog.FileName;
            if ((stream = dialog.OpenFile()) != null)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(tbScript.Text);
                stream.Write(bytes, 0, bytes.Length);
                stream.Close();
            }
        }
    }

    private void InputControlEditor_OnClosing(object sender, CancelEventArgs e)
    {
        if (DialogResult == null)
        {
            if (_viewModel.IsApplyEnabled)
            {
                _viewModel.InputScript = tbScript.Text;
                _viewModel.ApplyScript();
            }

            if (_viewModel.ValidationError != null)
            {
                e.Cancel = true;
            }
        }
    }

    protected void OnCancelButtonClick(object sender, RoutedEventArgs e)
    {
        _viewModel.CancelScript();
        DialogResult = false;
    }

    private void ButtonUndo_OnClick(object sender, RoutedEventArgs e)
    {
        tbScript.Undo();
    }

    private void ButtonRedo_OnClick(object sender, RoutedEventArgs e)
    {
        if (tbScript.CanRedo)
        {
            tbScript.Redo();
        }
    }

    private void TbScript_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        _viewModel.IsApplyEnabled = true;
    }

    private void TbScript_OnPreviewKeyDown(object sender, InputEventArgs e)
    {
        _viewModel.OnPreviewKeyDown(sender, e);
    }

    private void InfoMsgHide(object sender, RoutedEventArgs e)
    {
        _viewModel.IsInfoMsgHide = true;
    }

    protected void OnOkButtonClick(object sender, RoutedEventArgs e)
    {
        if (_viewModel.IsApplyEnabled)
        {
            _viewModel.InputScript = tbScript.Text;
            _viewModel.ApplyScript();
        }

        if (_viewModel.ValidationError == null && IsVisible)
        {
            DialogResult = true;
        }
    }

    private void OnApplyButtonClick(object sender, RoutedEventArgs e)
    {
        _viewModel.InputScript = tbScript.Text;
        _viewModel.ApplyScript();
        if (_viewModel.ValidationError == null)
        {
            _viewModel.IsApplyEnabled = false;
        }
    }

    private void ButtonFontSizeIncrease_OnClick(object sender, RoutedEventArgs e)
    {
        if (tbScript.FontSize < 25)
        {
            tbScript.FontSize += 1;
        }
    }

    private void ButtonFontSizeDecrease_OnClick(object sender, RoutedEventArgs e)
    {
        if (tbScript.FontSize > 5)
        {
            tbScript.FontSize -= 1;
        }
    }

    public void InitializeComponent() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private bool _contentLoaded;
}
