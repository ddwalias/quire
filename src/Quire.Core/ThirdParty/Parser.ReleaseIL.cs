#nullable disable
using Quire.Mirror;

namespace Quire.ThirdParty;

public sealed partial class Parser
{
    [MirrorVerified("Release IL")]
    private static string HelpText(ArgumentAttribute attribute, global::System.Reflection.FieldInfo fieldInfo)
    {
        if (attribute == null)
        {
            return null;
        }
        return attribute.HelpText;
    }

    [MirrorVerified("Release IL")]
    private static bool HasHelpText(ArgumentAttribute attribute)
    {
        if (attribute != null)
        {
            return attribute.HasHelpText;
        }
        return false;
    }

    [MirrorVerified("Release IL")]
    public string GetUsageString(int screenWidth)
    {
        ArgumentHelpStrings[] allHelpStrings = GetAllHelpStrings();
        int num = 0;
        int num3;
        int num2;
        string newLine;
        global::System.Text.StringBuilder stringBuilder;
        ArgumentHelpStrings[] array = allHelpStrings;
        int i;
        ArgumentHelpStrings argumentHelpStrings;
        ArgumentHelpStrings argumentHelpStrings2;
        for (i = 0; i < array.Length; i++)
        {
            argumentHelpStrings = array[i];
            num = global::System.Math.Max(num, argumentHelpStrings.syntax.Length);
        }
        num2 = num + 2;
        screenWidth = global::System.Math.Max(screenWidth, 15);
        if (screenWidth < num2 + 10)
        {
            num3 = 5;
        }
        else
        {
            num3 = num2;
        }
        newLine = global::System.Environment.NewLine;
        stringBuilder = new global::System.Text.StringBuilder();
        array = allHelpStrings;
        for (i = 0; i < array.Length; i++)
        {
            argumentHelpStrings2 = array[i];
            int length = argumentHelpStrings2.syntax.Length;
            stringBuilder.Append(argumentHelpStrings2.syntax);
            int currentColumn = length;
            if (length >= num3)
            {
                stringBuilder.Append(newLine);
                currentColumn = 0;
            }
            int num4 = screenWidth - num3;
            int j = 0;
            while (j < argumentHelpStrings2.help.Length)
            {
                stringBuilder.Append(' ', num3 - currentColumn);
                currentColumn = num3;
                int num5 = j + num4;
                if (num5 >= argumentHelpStrings2.help.Length)
                {
                    num5 = argumentHelpStrings2.help.Length;
                }
                else
                {
                    num5 = argumentHelpStrings2.help.LastIndexOf(' ', num5 - 1, global::System.Math.Min(num5 - j, num4));
                    if (num5 <= j)
                    {
                        num5 = j + num4;
                    }
                }
                stringBuilder.Append(argumentHelpStrings2.help, j, num5 - j);
                j = num5;
                AddNewLine(newLine, stringBuilder, ref currentColumn);
                for (; j < argumentHelpStrings2.help.Length && argumentHelpStrings2.help[j] == ' '; j++)
                {
                }
            }
            if (argumentHelpStrings2.help.Length == 0)
            {
                stringBuilder.Append(newLine);
            }
        }
        return stringBuilder.ToString();
    }

    [MirrorVerified("Release IL")]
    public Parser(global::System.Type argumentSpecification, ErrorReporter reporter)
    {
        this.reporter = reporter;
        arguments = new global::System.Collections.ArrayList();
        argumentMap = new global::System.Collections.Hashtable();
        global::System.Reflection.FieldInfo[] fields = argumentSpecification.GetFields();
        foreach (global::System.Reflection.FieldInfo fieldInfo in fields)
        {
            if (!fieldInfo.IsStatic && !fieldInfo.IsInitOnly && !fieldInfo.IsLiteral)
            {
                ArgumentAttribute attribute = GetAttribute(fieldInfo);
                if (attribute is DefaultArgumentAttribute)
                {
                    defaultArgument = new Argument(attribute, fieldInfo, reporter);
                }
                else
                {
                    arguments.Add(new Argument(attribute, fieldInfo, reporter));
                }
            }
        }
        foreach (Argument argument3 in arguments)
        {
            argumentMap[argument3.LongName] = argument3;
            if (argument3.ExplicitShortName)
            {
                if (!global::System.String.IsNullOrEmpty(argument3.ShortName))
                {
                    argumentMap[argument3.ShortName] = argument3;
                }
                else
                {
                    argument3.ClearShortName();
                }
            }
        }
        foreach (Argument argument4 in arguments)
        {
            if (!argument4.ExplicitShortName)
            {
                if (!global::System.String.IsNullOrEmpty(argument4.ShortName) && !argumentMap.ContainsKey(argument4.ShortName))
                {
                    argumentMap[argument4.ShortName] = argument4;
                }
                else
                {
                    argument4.ClearShortName();
                }
            }
        }
    }

    [global::System.Runtime.InteropServices.DllImport("kernel32.dll", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.StdCall, CharSet = global::System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
    private static extern int GetStdHandle(int nStdHandle);

    [global::System.Runtime.InteropServices.DllImport("kernel32.dll", CallingConvention = global::System.Runtime.InteropServices.CallingConvention.StdCall, CharSet = global::System.Runtime.InteropServices.CharSet.Auto, SetLastError = true)]
    private static extern int GetConsoleScreenBufferInfo(int hConsoleOutput, ref CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo);

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
    public static string ArgumentsUsage(global::System.Type argumentType)
    {
        int num = GetConsoleWindowWidth();
        if (num == 0)
        {
            num = 80;
        }
        return ArgumentsUsage(argumentType, num);
    }

    [MirrorVerified("Release IL")]
    public static string ArgumentsUsage(global::System.Type argumentType, int columns)
    {
        return new Parser(argumentType, null).GetUsageString(columns);
    }

    [MirrorVerified("Release IL")]
    public static int GetConsoleWindowWidth()
    {
        CONSOLE_SCREEN_BUFFER_INFO lpConsoleScreenBufferInfo = default;
        GetConsoleScreenBufferInfo(GetStdHandle(-11), ref lpConsoleScreenBufferInfo);
        return lpConsoleScreenBufferInfo.dwSize.x;
    }

    [MirrorVerified("Release IL")]
    public static bool ParseArguments(string[] arguments, object destination, ErrorReporter reporter)
    {
        Parser parser = new Parser(destination.GetType(), reporter);
        return parser.Parse(arguments, destination);
    }

    [MirrorVerified("Release IL")]
    public bool Parse(string[] args, object destination)
    {
        bool flag = ParseArgumentList(args, destination);
        foreach (Argument argument in arguments)
        {
            flag |= argument.Finish(destination);
        }
        if (defaultArgument != null)
        {
            flag |= defaultArgument.Finish(destination);
        }
        return !flag;
    }

    [MirrorVerified("Release IL")]
    private bool ParseArgumentList(string[] args, object destination)
    {
        bool flag = false;
        if (args != null)
        {
            foreach (string text in args)
            {
                int num;
                string text2;
                string value;
                Argument argument;
                string[] args2;
                char ch;
                if (text.Length <= 0)
                {
                    continue;
                }
                ch = text[0];
                switch (ch)
                {
                    case '-':
                    case '/':
                    {
                        num = text.IndexOfAny(new char[3] { ':', '+', '-' }, 1);
                        text2 = text.Substring(1, (num == -1) ? (text.Length - 1) : (num - 1));
                        if (text2.Length + 1 == text.Length)
                        {
                            value = null;
                        }
                        else
                        {
                            if (text.Length <= 1 + text2.Length || text[1 + text2.Length] != ':')
                                goto ValueWithoutColon;
                            value = text.Substring(text2.Length + 2);
                            goto ValueAssigned;
                        ValueWithoutColon:
                            value = text.Substring(text2.Length + 1);
                        ValueAssigned:
                            ;
                        }
                        argument = (Argument)argumentMap[text2];
                        if (argument == null)
                        {
                            ReportUnrecognizedArgument(text);
                            flag = true;
                        }
                        else
                        {
                            flag |= !argument.SetValue(value, destination);
                        }
                        break;
                    }
                    case '@':
                    {
                        flag |= LexFileArguments(text.Substring(1), out args2);
                        flag |= ParseArgumentList(args2, destination);
                        break;
                    }
                    default:
                        if (defaultArgument != null)
                        {
                            flag |= !defaultArgument.SetValue(text, destination);
                            break;
                        }
                        ReportUnrecognizedArgument(text);
                        flag = true;
                        break;
                }
            }
        }
        return flag;
    }

    [MirrorVerified("Release IL")]
    private bool LexFileArguments(string fileName, out string[] arguments)
    {
        string text = null;
        try
        {
            using global::System.IO.FileStream stream = new global::System.IO.FileStream(fileName, global::System.IO.FileMode.Open, global::System.IO.FileAccess.Read);
            text = new global::System.IO.StreamReader(stream).ReadToEnd();
        }
        catch (global::System.Exception ex)
        {
            ReportError(global::System.String.Format("Error: Can't open command line argument file '{0}' : '{1}'", fileName, ex.Message));
            arguments = null;
            return false;
        }
        bool result = false;
        global::System.Collections.ArrayList arrayList = new global::System.Collections.ArrayList();
        global::System.Text.StringBuilder stringBuilder = new global::System.Text.StringBuilder();
        bool flag = false;
        int i = 0;
        try
        {
            while (true)
            {
                while (global::System.Char.IsWhiteSpace(text[i]))
                {
                    i++;
                }
                if (text[i] == '#')
                {
                    for (i++; text[i] != '\n'; i++)
                    {
                    }
                    continue;
                }
                do
                {
                    if (text[i] == '\\')
                    {
                        int num = 1;
                        i++;
                        while (i == text.Length && text[i] == '\\')
                        {
                            num++;
                        }
                        if (i == text.Length || text[i] != '"')
                        {
                            stringBuilder.Append('\\', num);
                            continue;
                        }
                        stringBuilder.Append('\\', num >> 1);
                        if ((num & 1) != 0)
                        {
                            stringBuilder.Append('"');
                        }
                        else
                        {
                            flag = !flag;
                        }
                    }
                    else if (text[i] == '"')
                    {
                        flag = !flag;
                        i++;
                    }
                    else
                    {
                        stringBuilder.Append(text[i]);
                        i++;
                    }
                }
                while (!global::System.Char.IsWhiteSpace(text[i]) | flag);
                arrayList.Add(stringBuilder.ToString());
                stringBuilder.Length = 0;
            }
        }
        catch (global::System.IndexOutOfRangeException)
        {
            if (flag)
            {
                ReportError(global::System.String.Format("Error: Unbalanced '\"' in command line argument file '{0}'", fileName));
                result = true;
            }
            else if (stringBuilder.Length > 0)
            {
                arrayList.Add(stringBuilder.ToString());
            }
        }
        arguments = (string[])arrayList.ToArray(typeof(string));
        return result;
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
