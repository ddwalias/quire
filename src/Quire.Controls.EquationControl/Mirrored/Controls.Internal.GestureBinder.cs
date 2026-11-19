using System;
using System.Windows.Input;
using Quire;
using Quire.FunctionalitiesLimitation;
using Quire.Services;

namespace Quire.Controls.Internal;

internal static partial class GestureBinder
{
    public static void Bind(VisualMathRegionNode editor, GestureManager manager)
    {
        manager.AddBinding(" ", editor.FireExpandScope);
        manager.AddBinding(")", editor.FireExitGroupRight);
        manager.AddBinding("[", InsertOperatorCommand(editor, "OVERLOADED_BRACKET", EnumFunctionalityKey.VectorAndMatrixOperatorsMatrixOrMatrixIndexCommand));
        manager.AddBinding("\"", InsertStringCommand(editor, null));
        foreach (VisualOperatorDescriptor value in VisualOperatorDescriptorTable.Default.Values)
        {
            if (value.Keybinding != null)
            {
                manager.AddBinding(value.Keybinding, InsertOperatorCommand(editor, value));
            }

            if (value.MultiKeyStrokesBinding != null)
            {
                manager.AddBinding(value.MultiKeyStrokesBinding, InsertOperatorCommand(editor, value));
            }
        }
    }

    public static void Bind(VisualTextNodeEditor editor, GestureManager manager)
    {
    }

    public static void Bind<T>(VisualMathNodeEditor<T> editor, GestureManager manager) where T : VisualMathNode
    {
    }

    public static void Bind(VisualScoper editor, GestureManager manager)
    {
    }

    public static void LimitSideNodeGestureBind(VisualMathRegionNode editor, GestureManager manager)
    {
        manager.AddBinding(" ", editor.FireExpandScope);
    }

    private static EditorCommand InsertOperatorCommand(VisualMathRegionNode editor, string nodeName, EnumFunctionalityKey functionalityKey)
    {
        return () =>
        {
            FireInsertOperatorWrapper(editor, functionalityKey, EquationCommands.InsertOperator, nodeName);
        };
    }

    private static EditorCommand InsertOperatorCommand(VisualMathRegionNode view, VisualOperatorDescriptor descriptor)
    {
        return () =>
        {
            FireInsertOperatorWrapper(view, descriptor.FunctionalityKey, descriptor.Command, descriptor.CommandParameter);
        };
    }

    private static void FireInsertOperatorWrapper(VisualMathRegionNode view, EnumFunctionalityKey functionalityKey, RoutedCommand command, object commandParameter)
    {
        if (DebugTools.AssertNotNull(command, null).Failed)
        {
            command = EquationCommands.InsertOperator;
        }

        if (FunctionalityServiceWrapper.Instance.IsFunctionalityEnabled(functionalityKey) && command.CanExecute(commandParameter, view))
        {
            command.Execute(commandParameter, view);
        }
    }

    private static EditorCommand InsertStringCommand(VisualMathRegionNode editor, string stringValue)
    {
        return () =>
        {
            editor.FireInsertString(stringValue);
        };
    }
}
