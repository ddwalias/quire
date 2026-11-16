using System;
using System.Collections;
using System.Windows;
using Quire.EquationEditor.Tree;
using Quire.FunctionalitiesLimitation;
using Quire.Wpf;

namespace Quire.Controls;

public partial class VisualDescriptorTableBase
{
    protected partial class InitializationInfo
    {
        public bool FunctionalityDictionaryInitialized { get; set; }

        public InitializationInfo()
        {
        }
    }

    protected void PostInitializeComponent(InitializationInfo initInfo)
    {
        UpdateElementUids();
        UpdateFunctionalityDictionary(initInfo);
    }

    public void UpdateElementUids()
    {
        string text = GetType().ToString();
        IDictionaryEnumerator enumerator = GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Value is SymbolDescriptor symbolDescriptor)
            {
                symbolDescriptor.ToolTipUid = text + "." + enumerator.Key?.ToString();
            }
            else
            {
                DebugTools.DevAssert(false, null);
            }
        }
    }

    protected virtual void UpdateFunctionalityDictionary(InitializationInfo initInfo)
    {
        if (initInfo.FunctionalityDictionaryInitialized)
        {
            return;
        }

        initInfo.FunctionalityDictionaryInitialized = true;
        IDictionaryEnumerator enumerator = GetEnumerator();
        while (enumerator.MoveNext())
        {
            SymbolDescriptor symbolDescriptor = enumerator.Value as SymbolDescriptor;
            if (!DebugTools.AssertNotNull(symbolDescriptor, null).Passed)
            {
                continue;
            }

            RoutedUICommandWithFunctionalityParameterKey command = symbolDescriptor.Command as RoutedUICommandWithFunctionalityParameterKey;
            if (command == null || !DebugTools.AssertNotNull(symbolDescriptor.CommandParameter, null).Passed)
            {
                continue;
            }

            EnumFunctionalityKey functionalityKey = symbolDescriptor.FunctionalityKey;
            object commandParameter = symbolDescriptor.CommandParameter;
            if (command.ContainsParameter(commandParameter))
            {
                if (command.GetFunctionalityKeyAccordingParameter(commandParameter) != functionalityKey)
                {
                    DebugTools.FailAssertion(null, Array.Empty<object>());
                }
            }
            else
            {
                command.AddFunctionalityKeyAccordingParameter(commandParameter, functionalityKey);
            }

            if (!(enumerator.Key is string) || !DefaultDescriptorTable.Default.TryGetValue(enumerator.Key as string, out MathNodeDescriptor descriptor))
            {
                continue;
            }

            string operatorSExpName = descriptor.OperatorSExpName;
            if (EquationInsertCommand.ContainsSExpression(operatorSExpName))
            {
                if (EquationInsertCommand.GetFunctionalityKeyAccordingSExpression(operatorSExpName) != functionalityKey)
                {
                    DebugTools.FailAssertion(null, Array.Empty<object>());
                }
            }
            else
            {
                EquationInsertCommand.AddFunctionalityKeyAccordingSExpression(operatorSExpName, functionalityKey);
            }
        }
    }

    public VisualDescriptorTableBase()
    {
    }
}
