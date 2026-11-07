#nullable disable
using System.Collections.Generic;
using System.Linq;
using Quire.Core;
using Quire.Undo;
using Quire.Wpf;

namespace Quire.Controls.Core;

public static partial class PropertyAggregateExtension
{
    private static void SetPropertyValueWithCommandUndoable(IPropertyAggregate propertyAggregate, string propertyName, object oldValue, object newValue)
    {
        CompoundCommand nestedCommand = new CompoundCommand();
        CommandBuffer.Execute(new UndoableCommand("Set Property Value", DoAction, UndoAction, RedoAction));
        bool DoAction()
        {
            using (new CommandScope(nestedCommand))
            {
                propertyAggregate.SetValueByName(propertyName, newValue);
            }
            return true;
        }
        void UndoAction()
        {
            nestedCommand.Undo();
            propertyAggregate.SetValueByName(propertyName, oldValue);
        }
        void RedoAction()
        {
            nestedCommand.Redo();
            propertyAggregate.SetValueByName(propertyName, newValue);
        }
    }

    public static void SetPropertyValueWithCommand(this IPropertyAggregate propertyAggregate, SetPropertyValueCommandParameter parameter)
    {
        string propertyName = parameter.PropertyName;
        object valueByName = propertyAggregate.GetValueByName(propertyName);
        object dependencyPropVal = parameter.DependencyPropVal;
        SetPropertyValueWithCommandUndoable(propertyAggregate, propertyName, valueByName, dependencyPropVal);
    }

    public static void SetPropertyValueWithCommand(this IPropertyAggregate propertyAggregate, SetPropertyValueCommandParameter parameter, object newValue)
    {
        string propertyName = parameter.PropertyName;
        object valueByName = propertyAggregate.GetValueByName(propertyName);
        SetPropertyValueWithCommandUndoable(propertyAggregate, propertyName, valueByName, newValue);
    }

    public static T ValidateReferenceType<T>(this IPropertyAggregate propertyAggregate, object value, string propertyName) where T : class
    {
        T val = value as T;
        if (!DebugTools.DevAssert(val != null || value == null, "Type mismatch").Passed)
        {
            return null;
        }
        return val;
    }

    public static T? ValidateNullableType<T>(this IPropertyAggregate propertyAggregate, object value, string propertyName) where T : struct
    {
        T? result = value as T?;
        if (!DebugTools.DevAssert(result.HasValue || value == null, "Type mismatch").Passed)
        {
            return null;
        }
        return result;
    }

    public static bool IsValidateValueType<T>(this IPropertyAggregate propertyAggregate, string propertyName, object value)
    {
        return DebugTools.DevAssert(value is T, "Type mismatch").Passed;
    }

    private const string ErrorMessageTypeMissmatch = "Type mismatch";

    public static object GetCommonPropertyValueForList(IEnumerable<IPropertyAggregate> items, IPropertyAggregate compareTo, string propertyName)
    {
        IPropertyAggregate propertyAggregate = compareTo;
        if (propertyAggregate == null)
        {
            propertyAggregate = items.FirstOrDefault((IPropertyAggregate item) => item != null);
            if (propertyAggregate == null)
            {
                return null;
            }
        }
        object compareValue = propertyAggregate.GetValueByName(propertyName);
        if (!items.All((IPropertyAggregate nf) => nf == null || object.Equals(nf.GetValueByName(propertyName), compareValue)))
        {
            return null;
        }
        return compareValue;
    }

    public static T GetCommonFormat<T>(IEnumerable<T> formats, T compareTo, params string[] propertyNames) where T : class, IPropertyAggregate, new()
    {
        IEnumerable<T> items = formats.Where((T item) => item != null);
        if (items.IsNullOrEmpty())
        {
            return null;
        }
        T val = new T();
        foreach (string propertyName in propertyNames)
        {
            object commonPropertyValueForList = GetCommonPropertyValueForList(items, compareTo, propertyName);
            val.SetValueByName(propertyName, commonPropertyValueForList);
        }
        return val;
    }
}
