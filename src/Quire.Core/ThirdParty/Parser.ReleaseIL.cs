#nullable disable
using Quire.Mirror;

namespace Quire.ThirdParty;

public sealed partial class Parser
{
    [MirrorVerified("Release IL")]
    public static bool ParseArgumentsWithUsage(string[] arguments, object destination)
    {
        return ParseArgumentsWithUsage(arguments, destination, global::System.Console.Error.WriteLine);
    }

    [MirrorVerified("Release IL")]
    public static bool ParseArguments(string[] arguments, object destination)
    {
        return ParseArguments(arguments, destination, global::System.Console.Error.WriteLine);
    }

    [MirrorVerified("Release IL")]
    public static bool ParseArgumentsWithUsage(string[] arguments, object destination, ErrorReporter reporter)
    {
        if (ParseHelp(arguments) || !ParseArguments(arguments, destination, reporter))
        {
            reporter(ArgumentsUsage(destination.GetType()));
            return false;
        }
        return true;
    }

    [MirrorVerified("Release IL")]
    public static int IndexOf(global::System.Text.StringBuilder text, char value, int startIndex)
    {
        for (int i = startIndex; i < text.Length; i++)
        {
            if (text[i] == value)
            {
                return i;
            }
        }
        return -1;
    }

    [MirrorVerified("Release IL")]
    public static int LastIndexOf(global::System.Text.StringBuilder text, char value, int startIndex)
    {
        for (int num = global::System.Math.Min(startIndex, text.Length - 1); num >= 0; num--)
        {
            if (text[num] == value)
            {
                return num;
            }
        }
        return -1;
    }

    [MirrorVerified("Release IL")]
    private static ArgumentAttribute GetAttribute(global::System.Reflection.FieldInfo fieldInfo)
    {
        object[] attributes = fieldInfo.GetCustomAttributes(typeof(ArgumentAttribute), false);
        if (attributes.Length == 1)
        {
            return (ArgumentAttribute)attributes[0];
        }
        return null;
    }

    [MirrorVerified("Release IL")]
    private void ReportError(string errorMessage)
    {
        if (reporter != null)
        {
            reporter(errorMessage);
        }
    }

    [MirrorVerified("Release IL")]
    private int NumberOfParametersToDisplay()
    {
        int num = arguments.Count;
        if (HasDefaultArgument)
        {
            num++;
        }
        return num;
    }

    [MirrorVerified("Release IL")]
    private static void AddNewLine(string newLine, global::System.Text.StringBuilder builder, ref int currentColumn)
    {
        builder.Append(newLine);
        currentColumn = 0;
    }

    [MirrorVerified("Release IL")]
    private ArgumentHelpStrings[] GetAllHelpStrings()
    {
        ArgumentHelpStrings[] array = new ArgumentHelpStrings[NumberOfParametersToDisplay()];
        int num = 0;
        foreach (Argument argument in arguments)
        {
            array[num] = GetHelpStrings(argument);
            num++;
        }
        if (defaultArgument != null)
        {
            array[num++] = GetHelpStrings(defaultArgument);
        }
        return array;
    }

    [MirrorVerified("Release IL")]
    private static string LongName(ArgumentAttribute attribute, global::System.Reflection.FieldInfo fieldInfo)
    {
        if (attribute != null && !attribute.DefaultLongName)
        {
            return attribute.LongName;
        }
        return fieldInfo.Name;
    }

    [MirrorVerified("Release IL")]
    private static string ShortName(ArgumentAttribute attribute, global::System.Reflection.FieldInfo fieldInfo)
    {
        if (attribute is DefaultArgumentAttribute)
        {
            return null;
        }
        if (!ExplicitShortName(attribute))
        {
            return LongName(attribute, fieldInfo).Substring(0, 1);
        }
        return attribute.ShortName;
    }

    [MirrorVerified("Release IL")]
    private static bool ExplicitShortName(ArgumentAttribute attribute)
    {
        if (attribute != null)
        {
            return !attribute.DefaultShortName;
        }
        return false;
    }

    [MirrorVerified("Release IL")]
    private static object DefaultValue(ArgumentAttribute attribute, global::System.Reflection.FieldInfo fieldInfo)
    {
        if (attribute != null && attribute.HasDefaultValue)
        {
            return attribute.DefaultValue;
        }
        return null;
    }

    [MirrorVerified("Release IL")]
    private static global::System.Type ElementType(global::System.Reflection.FieldInfo fieldInfo)
    {
        if (IsCollectionType(fieldInfo.FieldType))
        {
            return fieldInfo.FieldType.GetElementType();
        }
        return null;
    }

    [MirrorVerified("Release IL")]
    private static ArgumentType Flags(ArgumentAttribute attribute, global::System.Reflection.FieldInfo fieldInfo)
    {
        if (attribute != null)
        {
            return attribute.Type;
        }
        if (IsCollectionType(fieldInfo.FieldType))
        {
            return ArgumentType.MultipleUnique;
        }
        return ArgumentType.AtMostOnce;
    }

    [MirrorVerified("Release IL")]
    private static bool IsValidElementType(global::System.Type type)
    {
        if (type != null)
        {
            if (!(type == typeof(int)) && !(type == typeof(uint)) && !(type == typeof(string)) && !(type == typeof(bool)))
            {
                return type.IsEnum;
            }
            return true;
        }
        return false;
    }

    private partial class Argument
    {
        [MirrorVerified("Release IL")]
        public Argument(ArgumentAttribute attribute, global::System.Reflection.FieldInfo fieldInfo, ErrorReporter reporter)
        {
            longName = LongName(attribute, fieldInfo);
            explicitShortName = ExplicitShortName(attribute);
            shortName = ShortName(attribute, fieldInfo);
            hasHelpText = HasHelpText(attribute);
            helpText = HelpText(attribute, fieldInfo);
            defaultValue = DefaultValue(attribute, fieldInfo);
            elementType = ElementType(fieldInfo);
            flags = Flags(attribute, fieldInfo);
            this.fieldInfo = fieldInfo;
            seenValue = false;
            this.reporter = reporter;
            isDefault = attribute != null && attribute is DefaultArgumentAttribute;
            if (IsCollection)
            {
                collectionValues = new global::System.Collections.ArrayList();
            }
            validValues = ValidValues(attribute, fieldInfo);
        }

        [MirrorVerified("Release IL")]
        public bool Finish(object destination)
        {
            if (!SeenValue && HasDefaultValue)
            {
                fieldInfo.SetValue(destination, DefaultValue);
            }
            if (IsCollection)
            {
                fieldInfo.SetValue(destination, collectionValues.ToArray(elementType));
            }
            return ReportMissingRequiredArgument();
        }

        [MirrorVerified("Release IL")]
        private bool ReportMissingRequiredArgument()
        {
            if (IsRequired && !SeenValue)
            {
                if (IsDefault)
                {
                    reporter(global::System.String.Format("Missing required argument '<{0}>'.", LongName));
                }
                else
                {
                    reporter(global::System.String.Format("Missing required argument '/{0}'.", LongName));
                }
                return true;
            }
            return false;
        }

        [MirrorVerified("Release IL")]
        public bool SetValue(string value, object destination)
        {
            if (SeenValue && !AllowMultiple)
            {
                reporter(global::System.String.Format("Duplicate '{0}' argument", LongName));
                return false;
            }
            seenValue = true;
            if (!ParseValue(ValueType, value, out object value2))
            {
                return false;
            }
            if (IsCollection)
            {
                if (Unique && collectionValues.Contains(value2))
                {
                    ReportDuplicateArgumentValue(value);
                    return false;
                }
                collectionValues.Add(value2);
            }
            else if (validValues != "")
            {
                global::System.Collections.Specialized.StringCollection stringCollection = new global::System.Collections.Specialized.StringCollection();
                stringCollection.AddRange(validValues.Split('|'));
                if (!stringCollection.Contains(value))
                {
                    ReportInvalidValue(value);
                    return false;
                }
                fieldInfo.SetValue(destination, value2);
            }
            else
            {
                fieldInfo.SetValue(destination, value2);
            }
            return true;
        }

        [MirrorVerified("Release IL")]
        private bool ParseValue(global::System.Type type, string stringData, out object value)
        {
            if ((stringData != null || type == typeof(bool)) && (stringData == null || stringData.Length > 0))
            {
                try
                {
                    if (type == typeof(string))
                    {
                        value = stringData;
                        return true;
                    }
                    if (type == typeof(bool))
                    {
                        if (stringData == null || stringData == "+")
                        {
                            value = true;
                            return true;
                        }
                        if (stringData == "-")
                        {
                            value = false;
                            return true;
                        }
                    }
                    else if (type == typeof(int))
                    {
                        value = int.Parse(stringData);
                        return true;
                    }
                    else if (type == typeof(uint))
                    {
                        value = int.Parse(stringData);
                        return true;
                    }
                    else
                    {
                        value = global::System.Enum.Parse(type, stringData, true);
                        return true;
                    }
                }
                catch
                {
                }
            }
            ReportBadArgumentValue(stringData);
            value = null;
            return false;
        }

        [MirrorVerified("Release IL")]
        private void AppendValue(global::System.Text.StringBuilder builder, object value)
        {
            if (value is string || value is int || value is uint || value.GetType().IsEnum)
            {
                builder.Append(value.ToString());
                return;
            }
            if (value is bool)
            {
                builder.Append(((bool)value) ? "+" : "-");
                return;
            }
            bool first = true;
            foreach (object item in (global::System.Array)value)
            {
                if (!first)
                {
                    builder.Append(", ");
                }
                AppendValue(builder, item);
                first = false;
            }
        }

        [MirrorVerified("Release IL")]
        private string GetValidValues()
        {
            string text = "";
            string text2 = validValues;
            foreach (object obj in text2)
            {
                text += string.Format("\"{0}\" ", obj.ToString());
            }
            return text;
        }

        [MirrorVerified("Release IL")]
        public string FullHelpText
        {
            get
            {
                global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
                if (HasHelpText)
                {
                    stringBuilder.Append(HelpText);
                }
                if (HasDefaultValue)
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append(" ");
                    }
                    stringBuilder.Append("Default value:'");
                    AppendValue(stringBuilder, DefaultValue);
                    stringBuilder.Append('\'');
                }
                if (HasShortName)
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append(" ");
                    }
                    stringBuilder.Append("(short form /");
                    stringBuilder.Append(ShortName);
                    stringBuilder.Append(")");
                }
                return stringBuilder.ToString();
            }
        }

        [MirrorVerified("Release IL")]
        public string SyntaxHelp
        {
            get
            {
                global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
                if (IsDefault)
                {
                    stringBuilder.Append("<");
                    stringBuilder.Append(LongName);
                    stringBuilder.Append(">");
                }
                else
                {
                    stringBuilder.Append("/");
                    stringBuilder.Append(LongName);
                    global::System.Type valueType = ValueType;
                    if (valueType == typeof(int))
                    {
                        stringBuilder.Append(":<int>");
                    }
                    else if (valueType == typeof(uint))
                    {
                        stringBuilder.Append(":<uint>");
                    }
                    else if (valueType == typeof(bool))
                    {
                        stringBuilder.Append("[+|-]");
                    }
                    else if (valueType == typeof(string))
                    {
                        stringBuilder.Append(":<string>");
                    }
                    else
                    {
                        stringBuilder.Append(":{");
                        bool flag = true;
                        global::System.Reflection.FieldInfo[] fields = valueType.GetFields();
                        foreach (global::System.Reflection.FieldInfo fieldInfo in fields)
                        {
                            if (fieldInfo.IsStatic)
                            {
                                if (flag)
                                {
                                    flag = false;
                                }
                                else
                                {
                                    stringBuilder.Append('|');
                                }
                                stringBuilder.Append(fieldInfo.Name);
                            }
                        }
                        stringBuilder.Append('}');
                    }
                }
                return stringBuilder.ToString();
            }
        }
    }
}
