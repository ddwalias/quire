using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public partial class InputControlEditorViewModel
{
    public bool IsApplyEnabled
    {
        get => _applyEnabled;
        set
        {
            _applyEnabled = value;
            OnPropertyChanged(nameof(IsApplyEnabled));
        }
    }

    public bool ScriptingEditorEnabled
    {
        get => _scriptingEditorEnabled;
        set
        {
            if (_scriptingEditorEnabled != value)
            {
                _scriptingEditorEnabled = value;
                OnPropertyChanged(nameof(ScriptingEditorEnabled));
            }
        }
    }

    public int SelectedTabIndex
    {
        get => _selectedTabIndex;
        set
        {
            _selectedTabIndex = value;
            IsApplyVisibility = _selectedTabIndex == 0;
        }
    }

    public bool IsApplyVisibility
    {
        get => _applyVisibility;
        set
        {
            _applyVisibility = value;
            OnPropertyChanged(nameof(IsApplyVisibility));
        }
    }

    public bool IsInfoMsgHide
    {
        get => _isInfoMsgHide;
        set
        {
            _isInfoMsgHide = true;
            InfoMessageVisibility = Visibility.Collapsed;
        }
    }

    public string InputScript
    {
        get => _scriptEditor;
        set
        {
            _scriptEditor = value;
            OnPropertyChanged(nameof(InputScript));
        }
    }

    public bool CanUndo
    {
        get => _canUndo;
        set
        {
            if (_canUndo != value)
            {
                _canUndo = value;
                OnPropertyChanged(nameof(CanUndo));
            }
        }
    }

    public bool IsEnabled => _model.IsEnabled;

    public bool IsReadOnlyScript
    {
        get
        {
            if (_model.IsEnabled)
            {
                return !IsPythonIsUnavailable;
            }

            return true;
        }
    }

    protected bool IsPythonIsUnavailable
    {
        get => _isPythonIsUnavailable;
        set
        {
            _isPythonIsUnavailable = value;
            OnPropertyChanged(nameof(IsReadOnlyScript));
        }
    }

    public Brush EditorBackground => new SolidColorBrush(IsReadOnlyScript ? Colors.WhiteSmoke : Colors.White);

    public bool AutoRecalculate
    {
        get => _model.AutoRecalculate;
        set => _model.AutoRecalculate = value;
    }

    public bool CanRedo
    {
        get => _canRedo;
        set
        {
            if (_canRedo != value)
            {
                _canRedo = value;
                OnPropertyChanged(nameof(CanRedo));
            }
        }
    }

    public int LineNumber
    {
        get => _lineNumber;
        set
        {
            _lineNumber = value;
            OnPropertyChanged(nameof(LineNumber));
        }
    }

    public int ColNumber
    {
        get => _colNumber;
        set
        {
            _colNumber = value;
            OnPropertyChanged(nameof(ColNumber));
        }
    }

    public Visibility InfoMessageVisibility
    {
        get => _infoMessageVisibility;
        set
        {
            _infoMessageVisibility = value;
            OnPropertyChanged(nameof(InfoMessageVisibility));
        }
    }

    public string InfoMessageText
    {
        get => _infoMessageText;
        set
        {
            _infoMessageText = value;
            OnPropertyChanged(nameof(InfoMessageText));
        }
    }

    public Visibility ErrorMessageVisibility => _scriptError == null ? Visibility.Collapsed : Visibility.Visible;

    public string ValidationError
    {
        get => _scriptError;
        set
        {
            _scriptError = CapitalizeFirstLetter(value);
            IInputControlEditorScriptableModel scriptable = _model as IInputControlEditorScriptableModel;
            if (scriptable != null && value == null)
            {
                scriptable.HandlePostError(null);
            }

            OnPropertyChanged(nameof(ValidationError));
            OnPropertyChanged(nameof(ErrorMessageVisibility));
        }
    }

    protected virtual void HandleModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
    }

    private void Init()
    {
        InitInfoMessages();
        InitScriptingProperties();
    }

    private void InitInfoMessages()
    {
        _infoMessageVisibility = Visibility.Collapsed;
        _infoMessageText = _errorsDictionary[InputControlEditorErrors.VBScriptDeprecated].ToString();
    }

    private void InitScriptingProperties()
    {
        IInputControlEditorScriptableModel scriptable = _model as IInputControlEditorScriptableModel;
        if (scriptable != null)
        {
            _scriptEditor = scriptable.Script;
            _scriptLastKnownGood = scriptable.Script;
            _engineTypeLastKnownGood = scriptable.EngineType;
            _scriptError = ShowUserFriendlyError(scriptable.EngineError);
            if (scriptable.EngineType != InputControlEngineTypes.DefaultScript)
            {
                _selectedScriptingLanguage = (InputControlScriptingLanguageTypes)scriptable.EngineType;
                if (_selectedScriptingLanguage == InputControlScriptingLanguageTypes.VBScript && !IsInfoMsgHide)
                {
                    _infoMessageVisibility = Visibility.Visible;
                }

                if (_selectedScriptingLanguage == InputControlScriptingLanguageTypes.Python && !InputControlEnginePython.IsPythonPathSet)
                {
                    HandlePythonIsUnavailable();
                }
            }
        }
    }

    private string FullExceptionErrorFormat(string exception, string errorMessage)
    {
        return exception + Environment.NewLine + errorMessage;
    }

    private void HandlePythonIsUnavailable()
    {
        ScriptingEditorEnabled = false;
        IsPythonIsUnavailable = false;
        IsApplyEnabled = false;
        InfoMessageText = _errorsDictionary[InputControlEditorErrors.PythonDetectionError].ToString();
        InfoMessageVisibility = Visibility.Visible;
    }

    public void OnPreviewKeyDown(object sender, InputEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        if (textBox != null)
        {
            int caretIndex = textBox.CaretIndex;
            int lineIndex = textBox.GetLineIndexFromCharacterIndex(caretIndex);
            int colIndex = caretIndex - textBox.GetCharacterIndexFromLineIndex(lineIndex);
            LineNumber = lineIndex + 1;
            ColNumber = colIndex;
        }
    }

    public virtual void CancelScript()
    {
        if (ValidationError != null)
        {
            _model.EngineType = _engineTypeLastKnownGood;
            IInputControlEditorScriptableModel scriptable = _model as IInputControlEditorScriptableModel;
            if (scriptable != null)
            {
                scriptable.Script = _scriptLastKnownGood;
            }
        }
    }
}
