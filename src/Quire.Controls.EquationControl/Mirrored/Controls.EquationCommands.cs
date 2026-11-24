using System;
using Quire.Controls.Core;
using Quire.FunctionalitiesLimitation;
using Quire.Wpf;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using System.Windows.Input;

namespace Quire.Controls;

public partial class EquationCommands
{
    private static readonly Type _typeOfThis;
    public static readonly RoutedUICommand Backspace;
    public static readonly RoutedUICommand Delete;
    public static readonly RoutedUICommand ScopeLeft;
    public static readonly RoutedUICommand ScopeRight;
    public static readonly RoutedUICommand NavigateLeft;
    public static readonly RoutedUICommand NavigateRight;
    public static readonly RoutedUICommand NavigateUp;
    public static readonly RoutedUICommand NavigateDown;
    public static readonly RoutedUICommand NavigateLeftByTerm;
    public static readonly RoutedUICommand NavigateRightByTerm;
    public static readonly RoutedUICommand NavigateToEquationStart;
    public static readonly RoutedUICommand NavigateToEquationEnd;
    public static readonly RoutedUICommand Traverse;
    public static readonly RoutedUICommand BackwardsTraverse;
    public static readonly RoutedUICommand SplitLinearNaryLine;
    public static readonly RoutedUICommand RegionHistory;
    public static readonly RoutedUICommand TransformCharacter;
    public static readonly RoutedUICommand ChangeAlgorithm;
    public static readonly RoutedUICommand ToggleAlgorithmOptionMultiStart;
    public static readonly RoutedUICommand ToggleAlgorithmOptionOptimize;
    public static readonly RoutedUICommand ToggleSymbolicEvalKeywordsVisibilityState;
    public static readonly RoutedUICommand ToggleSubscript;
    public static readonly RoutedUICommand ToggleVisibleLhsOfDefinition;
    public static readonly RoutedUICommand InsertOperator;
    public static readonly RoutedUICommand SwitchLineBreak;
    public static readonly RoutedUICommand InsertEvaluation;
    public static readonly RoutedUICommand InsertSymbolicEvaluation;
    public static readonly RoutedUICommand InsertValue;
    public static readonly RoutedUICommand InsertFunction;
    public static readonly RoutedUICommand InsertId;
    public static readonly RoutedUICommand InsertKeyword;
    public static readonly RoutedUICommand InsertModifier;
    public static readonly RoutedUICommand InsertMatrix;
    public static readonly RoutedUICommand InsertString;
    public static readonly RoutedUICommand InsertInputControl;
    public static readonly RoutedUICommand InsertPi;
    public static readonly RoutedUICommand InsertInfinity;
    public static readonly RoutedUICommand InsertRow;
    public static readonly RoutedUICommand InsertColumn;
    public static readonly RoutedUICommand DeleteRow;
    public static readonly RoutedUICommand DeleteColumn;
    public static readonly RoutedUICommand ClearCells;
    public static readonly RoutedUICommand ExpandScope;
    public static readonly RoutedUICommand ContractScope;
    public static readonly RoutedUICommand FullyExpandScope;
    public static readonly RoutedUICommand FullyContractScope;
    public static readonly RoutedUICommand NavigateToStartOfScope;
    public static readonly RoutedUICommand NavigateToEndOfScope;
    public static readonly RoutedUICommand NavigateToOppositeOfScope;
    public static readonly RoutedUICommand Commit;
    public static readonly RoutedUICommand ToggleHideFromCalculationContext;
    public static readonly RoutedUICommand SetIdLabel;
    public static readonly RoutedUICommand UnsetIdLabel;
    public static readonly RoutedUICommand ToggleIdLabel;
    public static readonly RoutedUICommand ToggleUnitIdLabel;
    public static readonly RoutedUICommand ClearResultFormat;
    public static readonly RoutedCommand SetNumberFormatFieldCommand;
    public static readonly RoutedCommand SetMatrixFormatFieldCommand;
    public static readonly RoutedUICommand ConvertToOperator;
    public static readonly RoutedCommand ShowDebugPropertyWindow;

    static EquationCommands()
    {
        _typeOfThis = typeof(EquationCommands);
        Backspace = EditingCommands.Backspace;
        Delete = EditingCommands.Delete;
        ScopeLeft = EditingCommands.SelectLeftByWord;
        ScopeRight = EditingCommands.SelectRightByWord;
        NavigateLeft = EditingCommands.MoveLeftByCharacter;
        NavigateRight = EditingCommands.MoveRightByCharacter;
        NavigateUp = EditingCommands.MoveUpByLine;
        NavigateDown = EditingCommands.MoveDownByLine;
        NavigateLeftByTerm = EditingCommands.MoveLeftByWord;
        NavigateRightByTerm = EditingCommands.MoveRightByWord;
        NavigateToEquationStart = EditingCommands.MoveToLineStart;
        NavigateToEquationEnd = EditingCommands.MoveToLineEnd;
        Traverse = EditingCommands.TabForward;
        BackwardsTraverse = EditingCommands.TabBackward;
        SplitLinearNaryLine = EditingCommands.EnterParagraphBreak;
        RegionHistory = new RoutedUICommand();
        TransformCharacter = new RoutedUICommand();
        ChangeAlgorithm = new RoutedUICommandWithFunctionalityKey("Change Algorithm", "ChangeAlgorithm", _typeOfThis, EnumFunctionalityKey.ChangeAlgorithmCommand);
        ToggleAlgorithmOptionMultiStart = new RoutedUICommandWithFunctionalityKey("Toggle Algorithm Options MultiStart", "Toggle AlgorithmOption MultiStart", _typeOfThis, EnumFunctionalityKey.ToggleAlgorithmOptionMultiStartCommand);
        ToggleAlgorithmOptionOptimize = new RoutedUICommandWithFunctionalityKey("Toggle Algorithm Options Optimize", "Toggle AlgorithmOption Optimize", _typeOfThis, EnumFunctionalityKey.ToggleAlgorithmOptionOptimizeCommand);
        ToggleSymbolicEvalKeywordsVisibilityState = new RoutedUICommandWithFunctionalityKey("Toggle Symbolic Evaluation Keywords Visibility State", "ToggleSymbolicEvalKeywordsVisibilityState", _typeOfThis, EnumFunctionalityKey.SymbolicEvalKeywordsVisibilityStateCmd);
        ToggleSubscript = new RoutedUICommandWithFunctionalityKey("Toggle Subscript", "ToggleSubscript", _typeOfThis, EnumFunctionalityKey.SubscriptCommand);
        ToggleVisibleLhsOfDefinition = new RoutedUICommandWithFunctionalityKey("Toggle Visible LHS of Definition", "ToggleVisibleLhsOfDefinition", _typeOfThis, EnumFunctionalityKey.ToggleVisibleLhsOfDefinition);
        InsertOperator = new EquationInsertCommand("Insert Operator", "InsertOperator", _typeOfThis);
        SwitchLineBreak = new RoutedUICommandWithFunctionalityKey("Switch Line Break", "SwitchLineBreak", _typeOfThis, EnumFunctionalityKey.SwitchLineBreakCommand);
        InsertEvaluation = new EquationInsertCommand("Insert Evaluation", "InsertEvaluation", _typeOfThis);
        InsertSymbolicEvaluation = new EquationInsertCommand("Insert Symbolic Evaluation", "InsertSymbolicEvaluation", _typeOfThis);
        InsertValue = new EquationInsertCommand("Insert Value", "InsertValue", _typeOfThis);
        InsertFunction = new EquationInsertCommand("Insert Function", "InsertFunction", _typeOfThis, new InsertFunctionCommandParameterComparer());
        InsertId = new EquationInsertCommand("Insert Id", "InsertId", _typeOfThis, new InsertIdCommandParameterComparer());
        InsertKeyword = new EquationInsertCommand("Insert Keyword", "InsertKeyword", _typeOfThis);
        InsertModifier = new EquationInsertCommand("Insert Modifier", "InsertModifier", _typeOfThis);
        InsertMatrix = new RoutedUICommand("Insert Matrix", "InsertMatrix", _typeOfThis);
        InsertString = new RoutedUICommand("Insert String", "InsertString", _typeOfThis);
        InsertInputControl = new RoutedUICommandWithFunctionalityKey("Insert input Control", "InsertInputControl", _typeOfThis, EnumFunctionalityKey.InsertInputControlCommand);
        InsertPi = new RoutedUICommand();
        InsertInfinity = new RoutedUICommand();
        InsertRow = EditingCommands.EnterLineBreak;
        InsertColumn = new RoutedUICommand();
        DeleteRow = new RoutedUICommand();
        DeleteColumn = new RoutedUICommand();
        ClearCells = new RoutedUICommand();
        ExpandScope = new RoutedUICommand();
        ContractScope = new RoutedUICommand();
        FullyExpandScope = new RoutedUICommand();
        FullyContractScope = new RoutedUICommand();
        NavigateToStartOfScope = new RoutedUICommand();
        NavigateToEndOfScope = new RoutedUICommand();
        NavigateToOppositeOfScope = new RoutedUICommand();
        Commit = new RoutedUICommand();
        ToggleHideFromCalculationContext = new RoutedUICommand();
        SetIdLabel = new RoutedUICommand();
        UnsetIdLabel = new RoutedUICommand();
        ToggleIdLabel = new RoutedUICommand();
        ToggleUnitIdLabel = new RoutedUICommand();
        ClearResultFormat = new RoutedUICommand("ClearResultFormat", "Clear result format", typeof(EquationControl));
        SetNumberFormatFieldCommand = NumberFormat.SetNumberFormatFieldCommand;
        SetMatrixFormatFieldCommand = MatrixFormat.SetMatrixFormatFieldCommand;
        ConvertToOperator = new RoutedUICommandWithFunctionalityKey("Convert To Operator", "ConvertToOperator", _typeOfThis, EnumFunctionalityKey.ProgrammingOperatorsConvertToOperatorCommand);
        ShowDebugPropertyWindow = new RoutedCommand();
        NavigateLeft.InputGestures.Add(Gestures.LeftArrow);
        NavigateRight.InputGestures.Add(Gestures.RightArrow);
        NavigateUp.InputGestures.Add(Gestures.UpArrow);
        NavigateDown.InputGestures.Add(Gestures.DownArrow);
        NavigateLeftByTerm.InputGestures.Add(Gestures.CtrlLeft);
        NavigateRightByTerm.InputGestures.Add(Gestures.CtrlRight);
        NavigateToEquationStart.InputGestures.Add(Gestures.Home);
        NavigateToEquationEnd.InputGestures.Add(Gestures.End);
        Backspace.InputGestures.Add(Gestures.Backspace);
        Delete.InputGestures.Add(Gestures.Delete);
        RegionHistory.InputGestures.Add(Gestures.CtrlH);
        ScopeLeft.InputGestures.Add(Gestures.ShiftLeft);
        ScopeRight.InputGestures.Add(Gestures.ShiftRight);
        ExpandScope.InputGestures.Add(Gestures.ShiftUp);
        ContractScope.InputGestures.Add(Gestures.ShiftDown);
        ContractScope.InputGestures.Add(Gestures.CtrlDown);
        ContractScope.InputGestures.Add(Gestures.CtrlSpace);
        FullyExpandScope.InputGestures.Add(Gestures.CtrlA);
        FullyExpandScope.InputGestures.Add(Gestures.ShiftUp);
        FullyExpandScope.InputGestures.Add(Gestures.CtrlShiftSpace);
        FullyContractScope.InputGestures.Add(Gestures.Escape);
        NavigateToStartOfScope.InputGestures.Add(Gestures.ShiftHome);
        NavigateToEndOfScope.InputGestures.Add(Gestures.ShiftEnd);
        TransformCharacter.InputGestures.Add(Gestures.CtrlG);
        InsertPi.InputGestures.Add(Gestures.CtrlShiftP);
        InsertInfinity.InputGestures.Add(Gestures.CtrlShiftZ);
        CommandManager.RegisterClassInputBinding(typeof(EquationControl), new InputBinding(new InputBindingFunctionalityCommandWrapper(ToggleSubscript), Gestures.CtrlMinus));
        CommandManager.RegisterClassInputBinding(typeof(EquationControl), new InputBinding(new InputBindingFunctionalityCommandWrapper(ToggleSubscript), Gestures.CtrlNumpadMinus));
        CommandManager.RegisterClassInputBinding(typeof(EquationControl), new InputBinding(new InputBindingFunctionalityCommandWrapper(ConvertToOperator), Gestures.CtrlJ));
        InsertMatrix.InputGestures.Add(Gestures.CtrlM);
        InsertRow.InputGestures.Add(Gestures.ShiftEnter);
        InsertColumn.InputGestures.Add(Gestures.ShiftSpace);
        DeleteColumn.InputGestures.Add(Gestures.ShiftBackspace);
        CommandManager.RegisterClassInputBinding(typeof(EquationControl), new InputBinding(new InputBindingFunctionalityCommandWrapper(SwitchLineBreak), Gestures.CtrlShiftEnter));
        SplitLinearNaryLine.InputGestures.Add(Gestures.Enter);
        Traverse.InputGestures.Add(Gestures.Tab);
        BackwardsTraverse.InputGestures.Add(Gestures.ShiftTab);
        ShowDebugPropertyWindow.InputGestures.Add(Gestures.CtrlAltShiftQ);
        ToggleIdLabel.InputGestures.Add(Gestures.CtrlQ);
        ToggleUnitIdLabel.InputGestures.Add(Gestures.CtrlU);
    }

    public partial class CommitCommandParameter
    {
        public bool IsAsynchronous { get; }

        public bool IsForce { get; set; }

        public bool IsForceControlOnly { get; set; }

        public CommitCommandParameter(bool isAsynchronous)
        {
            IsAsynchronous = isAsynchronous;
        }

        public CommitCommandParameter(bool isAsynchronous, bool force, bool isForceControlOnly)
        {
            IsAsynchronous = isAsynchronous;
            IsForce = force;
            IsForceControlOnly = isForceControlOnly;
        }
    }

    public static IEnumerable<KeyGesture> GetCommandKeyGestures(RoutedCommand command)
    {
        IEnumerable<KeyGesture> gestures = command.InputGestures.OfType<KeyGesture>();
        DebugTools.DevAssert(gestures.IsNotEmpty(), null);
        return gestures;
    }

    public static KeyGesture GetCommandFirstKeyGesture(RoutedCommand command)
    {
        return GetCommandKeyGestures(command).FirstOrDefault();
    }
}
