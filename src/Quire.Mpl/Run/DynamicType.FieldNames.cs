#nullable disable
using System.Collections;
using Quire.Mirror;

namespace Quire.Mpl.Run;

public partial class DynamicType
{
    [MirrorVerified("DynamicTypeFieldNamesDifferentialTests")]
    private static string DelimiterString;

    [MirrorVerified("DynamicTypeFieldNamesDifferentialTests")]
    private static Hashtable NameToTypeTable;

    [MirrorVerified("DynamicTypeFieldNamesDifferentialTests")]
    private static Hashtable TypeToNameTable;

    [MirrorVerified("DynamicTypeFieldNamesDifferentialTests")]
    public static string TuplePrefix;

    [MirrorVerified("DynamicTypeFieldNamesDifferentialTests")]
    private static Hashtable FieldNameTable;

    [MirrorVerified("DynamicTypeFieldNamesDifferentialTests")]
    static DynamicType()
    {
        TuplePrefix = "Mpl.Tuple";
        FieldNameTable = new Hashtable();
        DelimiterString = "_";
        NameToTypeTable = new Hashtable();
        NameToTypeTable.Add("I", typeof(int));
        NameToTypeTable.Add("F", typeof(double));
        NameToTypeTable.Add("O", typeof(object));
        TypeToNameTable = new Hashtable();
        TypeToNameTable.Add(typeof(int), "_I");
        TypeToNameTable.Add(typeof(double), "_F");
        TypeToNameTable.Add(typeof(object), "_O");
    }

    [MirrorVerified("DynamicTypeFieldNamesDifferentialTests")]
    public static string[] FieldNames(int n)
    {
        object obj = FieldNameTable[n];
        if (obj != null)
        {
            return (string[])obj;
        }

        string[] array = new string[n];
        for (int i = 0; i < n; i++)
        {
            array[i] = FieldName(i);
        }

        FieldNameTable[n] = array;
        return array;
    }
}
