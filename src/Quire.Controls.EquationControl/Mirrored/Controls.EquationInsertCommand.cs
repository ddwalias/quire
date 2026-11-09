using System;
using System.Collections.Generic;
using System.Linq;
using Quire.FunctionalitiesLimitation;

namespace Quire.Controls;

public partial class EquationInsertCommand
{
    public const string InsertValue = "InsertValue";
    public const string InsertId = "InsertId";

    private static readonly Dictionary<object, EnumFunctionalityKey> _sExpressionToFunctionalityKeyDictionary = new Dictionary<object, EnumFunctionalityKey>();

    public EquationInsertCommand()
    {
    }

    public EquationInsertCommand(string text, string name, Type ownerType)
        : base(text, name, ownerType)
    {
    }

    public EquationInsertCommand(string text, string name, Type ownerType, IEqualityComparer<object> comparer)
        : base(text, name, ownerType, comparer)
    {
    }

    public override IEnumerable<Dictionary<object, EnumFunctionalityKey>> ParameterToFunctionalityKeyDictionaryList
    {
        get
        {
            List<Dictionary<object, EnumFunctionalityKey>> list = base.ParameterToFunctionalityKeyDictionaryList.ToList();
            list.Add(_sExpressionToFunctionalityKeyDictionary);
            return list;
        }
    }

    public static bool ContainsSExpression(string sExpression)
    {
        return _sExpressionToFunctionalityKeyDictionary.ContainsKey(sExpression);
    }

    public static void AddFunctionalityKeyAccordingSExpression(string sExpression, EnumFunctionalityKey functionalityKey)
    {
        _sExpressionToFunctionalityKeyDictionary.Add(sExpression, functionalityKey);
    }

    public static EnumFunctionalityKey GetFunctionalityKeyAccordingSExpression(string sExpression)
    {
        if (!_sExpressionToFunctionalityKeyDictionary.TryGetValue(sExpression, out EnumFunctionalityKey value))
        {
            value = EnumFunctionalityKey.GlobalCategory;
            DebugTools.FailAssertion("SExpression \"{0}\" not found in the functionality key dictionary.", sExpression);
        }

        return value;
    }
}
