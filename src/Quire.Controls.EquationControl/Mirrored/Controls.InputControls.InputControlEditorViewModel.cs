using System;
using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Quire.EquationEditor.Tree;
using Quire.Wpf;

namespace Quire.Controls.InputControls;

public partial class InputControlEditorViewModel
{
    protected IInputControlEditorModel _model;
    protected string _scriptError;
    protected string _scriptEditor;
    protected string _scriptLastKnownGood;
    protected bool _applyEnabled;
    protected bool _applyVisibility = true;
    protected int _selectedTabIndex;
    protected InputControlScriptingLanguageTypes _selectedScriptingLanguage;
    protected InputControlEngineTypes _engineTypeLastKnownGood;
    private int _lineNumber;
    private int _colNumber;
    private bool _canRedo;
    private bool _canUndo;
    private Visibility _infoMessageVisibility;
    private string _infoMessageText;
    private bool _isInfoMsgHide;
    private bool _scriptingEditorEnabled = true;
    private bool _isPythonIsUnavailable = true;
    protected Hashtable cachedInputScript = new Hashtable();

    public InputControlEditorViewModel(IInputControlEditorModel model)
        : base(ResourceProvider<InputControlEditorResourceDirectory>.Instance)
    {
        _model = model;
        _model.PropertyChanged += HandleModelPropertyChanged;
        Init();
    }

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

    public InputControlScriptingLanguageTypes SelectedScriptingLanguage
    {
        get => _selectedScriptingLanguage;
        set
        {
            cachedInputScript[_selectedScriptingLanguage] = InputScript;
            _selectedScriptingLanguage = value;
            if (cachedInputScript.ContainsKey(_selectedScriptingLanguage))
            {
                InputScript = (string)cachedInputScript[_selectedScriptingLanguage];
            }
            else
            {
                InputScript = ((IInputControlEditorScriptableModel)_model).DefaultScript((InputControlEngineTypes)_selectedScriptingLanguage);
            }

            if (_selectedScriptingLanguage == InputControlScriptingLanguageTypes.VBScript && !IsInfoMsgHide)
            {
                InfoMessageText = _errorsDictionary[InputControlEditorErrors.VBScriptDeprecated].ToString();
                InfoMessageVisibility = Visibility.Visible;
            }
            else
            {
                InfoMessageVisibility = Visibility.Collapsed;
            }

            if (_selectedScriptingLanguage == InputControlScriptingLanguageTypes.Python && !InputControlEnginePython.IsPythonPathSet)
            {
                HandlePythonIsUnavailable();
                return;
            }

            ScriptingEditorEnabled = true;
            IsPythonIsUnavailable = true;
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

    public virtual void ApplyScript()
    {
        _model.EngineType = (InputControlEngineTypes)_selectedScriptingLanguage;
        bool applied = ((IInputControlEditorScriptableModel)_model).Apply(_scriptEditor, out _, out string error, null, null);
        if (error != null)
        {
            ValidationError = ShowUserFriendlyError(error);
        }
        else
        {
            if (ValidationError != null)
            {
                ValidationError = null;
            }

            _scriptLastKnownGood = _scriptEditor;
            _engineTypeLastKnownGood = _model.EngineType;
        }

        if (applied)
        {
            IsApplyEnabled = false;
        }
    }

    private string ShowUserFriendlyError(string exception)
    {
        if (!exception.IsNullOrEmpty())
        {
            if (exception.Contains("Object required: 'Worksheet'"))
            {
                return FullExceptionErrorFormat(exception, _errorsDictionary[InputControlEditorErrors.WorksheetNotSupported].ToString());
            }

            if (exception.Contains("Type mismatch: 'Inputs'") || exception.Contains("Object required: 'Inputs(...)'"))
            {
                return FullExceptionErrorFormat(exception, _errorsDictionary[InputControlEditorErrors.InputsMissing].ToString());
            }

            if (exception.Contains("is null or not an object") || exception.Contains("'NoneType' object is not"))
            {
                return FullExceptionErrorFormat(exception, _errorsDictionary[InputControlEditorErrors.ObjectMissing].ToString());
            }

            if (exception.Contains("Subscript out of range: '[number"))
            {
                return FullExceptionErrorFormat(exception, _errorsDictionary[InputControlEditorErrors.OutputsSizeNotMatch].ToString());
            }

            if (exception.Contains("Type mismatch: 'UBound'"))
            {
                return FullExceptionErrorFormat(exception, _errorsDictionary[InputControlEditorErrors.InputsUBound].ToString());
            }
        }

        return exception;
    }
    public string CapitalizeFirstLetter(string value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            return char.ToUpper(value[0]) + value.Substring(1);
        }

        return value;
    }
}
