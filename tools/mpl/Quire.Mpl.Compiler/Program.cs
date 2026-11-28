using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;
using MplAssemblyBuilder = System.Reflection.Emit.Quire.Mpl.AssemblyBuilder;
using MplModuleBuilder = System.Reflection.Emit.Quire.Mpl.ModuleBuilder;
using MplILGenerator = System.Reflection.Emit.Quire.Mpl.ILGenerator;
using MplFieldBuilder = System.Reflection.Emit.Quire.Mpl.FieldBuilder;

if (args.Length is 5 or 6 && args[0] == "--interface")
{
    CompileInterfaceModule(Path.GetFullPath(args[1]), Path.GetFullPath(args[2]),
        Path.GetFullPath(args[3]), args[4], args.Length == 6 ? args[5] : args[4]);
    return 0;
}

if (args.Length == 5 && args[0] == "--thin")
{
    CompileThinModule(Path.GetFullPath(args[1]), Path.GetFullPath(args[2]),
        Path.GetFullPath(args[3]), args[4]);
    return 0;
}

if (args.Length != 3)
{
    Console.Error.WriteLine("usage: Quire.Mpl.Compiler <source.mpl> <output.dll> <Mathcad install>\n"
        + "   or: Quire.Mpl.Compiler --interface <source.mpl> <output.dll> <Mathcad install> <Quire project>");
    return 2;
}

var sourcePath = Path.GetFullPath(args[0]);
var outputPath = Path.GetFullPath(args[1]);
var install = Path.GetFullPath(args[2]);
var source = File.ReadAllText(sourcePath);
var program = MplSubsetParser.Parse(source);
Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

switch (program.Module)
{
    case "Hashtable":
        CompileHashtable(program, outputPath, install);
        break;
    case "McdUnitSystemUS":
        CompileUnitSystemUs(program, outputPath, install);
        break;
    default:
        throw new NotSupportedException($"Prototype compiler does not support module {program.Module}.");
}
return 0;

static void CompileInterfaceModule(string sourcePath, string outputPath, string install, string project,
    string typeNamespace)
{
    if (!project.StartsWith("Quire.", StringComparison.Ordinal))
        throw new ArgumentException("Interface output project names start with Quire.", nameof(project));
    var source = File.ReadAllText(sourcePath, Encoding.UTF8).Replace("\r\n", "\n", StringComparison.Ordinal)
        .Replace('\r', '\n');
    const string authoredComment = "-- Interface-only module source: exported declarations define this module contract.\n";
    if (!source.StartsWith(authoredComment, StringComparison.Ordinal))
        throw new InvalidDataException("Expected the interface-only source header comment.");
    var interfaceText = source[authoredComment.Length..];
    if (!interfaceText.EndsWith('\n'))
        throw new InvalidDataException("Interface source must end with one file newline.");
    interfaceText = interfaceText[..^1];
    var moduleName = ParseInterfaceModuleName(interfaceText);
    var originalType = moduleName + ".values";
    var outputType = typeNamespace + ".values";

    var runtime = Assembly.LoadFrom(Path.Combine(install, "mplrun.dll"));
    var moduleTextCtor = runtime.GetType("Mpl.Run.ModuleTextAttribute", throwOnError: true)!
        .GetConstructor([typeof(string)]) ?? throw new MissingMethodException("ModuleTextAttribute..ctor(string)");
    var moduleInterfaceCtor = runtime.GetType("Mpl.Run.ModuleInterfaceAttribute", throwOnError: true)!
        .GetConstructor([typeof(string)]) ?? throw new MissingMethodException("ModuleInterfaceAttribute..ctor(string)");

    var assembly = CreateAssembly(project);
    var module = assembly.DefineDynamicModule(Path.GetFileName(outputPath));
    var values = module.DefineType(outputType, TypeAttributes.Public | TypeAttributes.Class);
    SetStringAttribute(values, moduleTextCtor, "");
    SetStringAttribute(values, moduleInterfaceCtor, interfaceText);
    AddMirrorOf(values, originalType + ", " + moduleName);

    var update = values.DefineMethod("Update", MethodAttributes.Public | MethodAttributes.Static,
        typeof(void), Type.EmptyTypes);
    update.GetILGenerator().Emit(OpCodes.Ret);

    var initializer = values.DefineConstructor(MethodAttributes.Public | MethodAttributes.Static,
        CallingConventions.Standard, Type.EmptyTypes);
    var initializerIl = initializer.GetILGenerator();
    initializerIl.Emit(OpCodes.Call, update);
    initializerIl.Emit(OpCodes.Ret);

    // TypeBuilder appends the object constructor from CreateType after Update and .cctor, matching Mpl order.
    values.CreateType();
    foreach (var constructor in ParseDataConstructors(interfaceText))
        DefineDataConstructorType(module, typeNamespace, moduleName, constructor, install);
    Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
    assembly.Save(outputPath);
}

static IReadOnlyList<MplDataConstructor> ParseDataConstructors(string interfaceText)
{
    var constructors = new List<MplDataConstructor>();
    foreach (var line in interfaceText.Split('\n').Select(line => line.Trim()))
    {
        var declaration = Regex.Match(line, @"\Adata\s+([A-Za-z_][A-Za-z0-9_]*)\s*(?:[a-z][A-Za-z0-9_ ]*)?=\s*(.+);\z",
            RegexOptions.CultureInvariant);
        if (!declaration.Success) continue;
        foreach (var variant in declaration.Groups[2].Value.Split('|').Select(part => part.Trim()))
        {
            var (name, argumentText) = ReadConstructor(variant);
            if (declaration.Groups[1].Value == "Bool" && name == "True" && argumentText.Length == 0)
                constructors.Add(new MplDataConstructor("True", name, [], Singleton: true, UnitAware: false));
            else if (declaration.Groups[1].Value == "List" && name == ":")
                constructors.Add(new MplDataConstructor("_colon", name, [typeof(object), typeof(object)],
                    Singleton: false, UnitAware: false));
            else if (declaration.Groups[1].Value == "Value" && name == "Str"
                && argumentText.StartsWith("Mpl.String", StringComparison.Ordinal))
                constructors.Add(new MplDataConstructor("Str", name, [typeof(object)],
                    Singleton: false, UnitAware: false));
            else if (declaration.Groups[1].Value == "Range" && name == "Range")
            {
                var arguments = ParseTypeArguments(argumentText);
                constructors.Add(new MplDataConstructor("Range", name,
                    arguments.Select(MapMplType).ToArray(), Singleton: false,
                    UnitAware: arguments.Any(argument => argument.StartsWith("Mpl.F8", StringComparison.Ordinal))));
            }
        }
    }
    return constructors;
}

static (string Name, string Arguments) ReadConstructor(string text)
{
    if (text.StartsWith('('))
    {
        var end = text.IndexOf(')');
        if (end < 0) throw new InvalidDataException($"Malformed Mpl data constructor: {text}");
        return (text[1..end], text[(end + 1)..].Trim());
    }
    var space = text.IndexOf(' ');
    return space < 0 ? (text.TrimEnd(';'), "") : (text[..space], text[(space + 1)..].Trim());
}

static IReadOnlyList<string> ParseTypeArguments(string text)
{
    var result = new List<string>();
    for (var position = 0; position < text.Length;)
    {
        while (position < text.Length && char.IsWhiteSpace(text[position])) position++;
        if (position == text.Length) break;
        var start = position;
        if (text[position] == '(')
        {
            var depth = 0;
            do
            {
                if (text[position] == '(') depth++;
                else if (text[position] == ')') depth--;
                position++;
            } while (position < text.Length && depth > 0);
            if (depth != 0) throw new InvalidDataException($"Unbalanced Mpl type: {text}");
            result.Add(text[(start + 1)..(position - 1)].Trim());
        }
        else
        {
            while (position < text.Length && !char.IsWhiteSpace(text[position])) position++;
            result.Add(text[start..position]);
        }
    }
    return result;
}

static Type MapMplType(string type) => type.StartsWith("Mpl.F8", StringComparison.Ordinal) ? typeof(double)
    : type == "Mpl.I4" ? typeof(int) : typeof(object);

static void DefineDataConstructorType(MplModuleBuilder module, string typeNamespace, string moduleName,
    MplDataConstructor constructor, string install)
{
    var fullName = typeNamespace + "." + constructor.TypeName;
    var type = module.DefineType(fullName, TypeAttributes.Public | TypeAttributes.Class);
    AddMirrorOf(type, "Mpl." + constructor.TypeName.Replace("_colon", ":", StringComparison.Ordinal) + ", " + moduleName);
    var fields = constructor.ArgumentTypes.Select((fieldType, index) => type.DefineField(
        "Field" + (index + 1), fieldType, FieldAttributes.Public)).ToArray();
    MplFieldBuilder? singleton = null;
    if (constructor.Singleton)
        singleton = type.DefineField("singleton", type, FieldAttributes.Public | FieldAttributes.Static);
    var instanceConstructor = type.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig
        | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard,
        constructor.ArgumentTypes.ToArray());
    var constructorIl = instanceConstructor.GetILGenerator();
    constructorIl.Emit(OpCodes.Ldarg_0);
    constructorIl.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)!);
    for (var index = 0; index < fields.Length; index++)
    {
        constructorIl.Emit(OpCodes.Ldarg_0);
        switch (index + 1)
        {
            case 1: constructorIl.Emit(OpCodes.Ldarg_1); break;
            case 2: constructorIl.Emit(OpCodes.Ldarg_2); break;
            case 3: constructorIl.Emit(OpCodes.Ldarg_3); break;
            default: constructorIl.Emit(OpCodes.Ldarg_S, (byte)(index + 1)); break;
        }
        constructorIl.Emit(OpCodes.Stfld, fields[index]);
    }
    constructorIl.Emit(OpCodes.Ret);

    var toString = type.DefineMethod("ToString", MethodAttributes.Public | MethodAttributes.Virtual,
        typeof(string), Type.EmptyTypes);
    var displayIl = toString.GetILGenerator();
    if (fields.Length == 0)
    {
        displayIl.Emit(OpCodes.Ldstr, constructor.DisplayName);
    }
    else
    {
        displayIl.Emit(OpCodes.Ldstr, "(" + constructor.DisplayName + " ");
        var runtime = Assembly.LoadFrom(Path.Combine(install, "mplrun.dll"));
        var dynamicType = runtime.GetType("Mpl.Run.DynamicType", throwOnError: true)!;
        var concat3 = dynamicType.GetMethod("MplConcat3", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
            binder: null, [typeof(object), typeof(object), typeof(object)], modifiers: null)
            ?? throw new MissingMethodException(dynamicType.FullName, "MplConcat3");
        var concat2 = typeof(string).GetMethod("Concat", BindingFlags.Public | BindingFlags.Static,
            binder: null, [typeof(object), typeof(object)], modifiers: null)
            ?? throw new MissingMethodException(typeof(string).FullName, "Concat(object, object)");
        for (var index = 0; index < fields.Length; index++)
        {
            displayIl.Emit(OpCodes.Ldarg_0);
            displayIl.Emit(OpCodes.Ldfld, fields[index]);
            if (constructor.UnitAware && fields[index].FieldType.IsValueType)
            {
                displayIl.Emit(OpCodes.Box, fields[index].FieldType);
                displayIl.Emit(OpCodes.Ldstr, "$");
                displayIl.Emit(OpCodes.Call, concat2);
            }
            displayIl.Emit(OpCodes.Ldstr, index == fields.Length - 1 ? ")" : " ");
            displayIl.Emit(OpCodes.Call, concat3);
        }
    }
    displayIl.Emit(OpCodes.Ret);

    if (constructor.Singleton)
    {
        var initializer = type.DefineConstructor(MethodAttributes.Public | MethodAttributes.Static,
            CallingConventions.Standard, Type.EmptyTypes);
        var initializerIl = initializer.GetILGenerator();
        initializerIl.Emit(OpCodes.Newobj, instanceConstructor);
        initializerIl.Emit(OpCodes.Stsfld, singleton!);
        initializerIl.Emit(OpCodes.Ret);
    }
    type.CreateType();
}

static void CompileThinModule(string sourcePath, string outputPath, string install, string project)
{
    var source = File.ReadAllText(sourcePath, Encoding.UTF8).Replace("\r\n", "\n", StringComparison.Ordinal)
        .Replace('\r', '\n');
    const string marker = "\n-- implementation\n";
    var parts = source.Split(marker, 2, StringSplitOptions.None);
    if (parts.Length != 2)
        throw new InvalidDataException("Thin-module source needs an -- implementation section after its interface.");
    var interfaceText = parts[0].TrimStart('\uFEFF');
    var commentEnd = interfaceText.IndexOf('\n');
    if (commentEnd < 0 || !interfaceText[..commentEnd].StartsWith("-- ", StringComparison.Ordinal))
        throw new InvalidDataException("Thin-module source needs one authored comment before its interface.");
    interfaceText = interfaceText[(commentEnd + 1)..];
    var moduleName = ParseInterfaceModuleName(interfaceText);
    if (project != "Quire." + moduleName)
        throw new InvalidDataException($"Source module {moduleName} does not map to output project {project}.");

    var bindings = new List<(string Name, string[] Args, string Expression)>();
    foreach (var raw in parts[1].Split('\n'))
    {
        var line = raw.Trim();
        if (line.Length == 0 || line.StartsWith("--", StringComparison.Ordinal)) continue;
        var separator = line.IndexOf(" = ", StringComparison.Ordinal);
        if (separator < 1) throw new InvalidDataException($"Unsupported thin Mpl definition: {line}");
        var head = line[..separator].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        bindings.Add((head[0], head.Skip(1).ToArray(), line[(separator + 3)..]));
    }

    var globalBindings = bindings.Where(binding => binding.Args.Length == 0).ToArray();
    var functions = bindings.Where(binding => binding.Args.Length > 0).ToArray();
    var runtime = Assembly.LoadFrom(Path.Combine(install, "mplrun.dll"));
    var moduleTextCtor = runtime.GetType("Mpl.Run.ModuleTextAttribute", throwOnError: true)!
        .GetConstructor([typeof(string)]) ?? throw new MissingMethodException("ModuleTextAttribute..ctor(string)");
    var moduleInterfaceCtor = runtime.GetType("Mpl.Run.ModuleInterfaceAttribute", throwOnError: true)!
        .GetConstructor([typeof(string)]) ?? throw new MissingMethodException("ModuleInterfaceAttribute..ctor(string)");
    var assembly = CreateAssembly(project);
    var module = assembly.DefineDynamicModule(Path.GetFileName(outputPath));
    var values = module.DefineType(project + ".values", TypeAttributes.Public | TypeAttributes.Class);
    SetStringAttribute(values, moduleTextCtor, "");
    SetStringAttribute(values, moduleInterfaceCtor, interfaceText);
    AddMirrorOf(values, moduleName + ".values, " + moduleName);
    var fields = globalBindings.ToDictionary(binding => binding.Name,
        binding => values.DefineField(binding.Name, typeof(object), FieldAttributes.Public | FieldAttributes.Static),
        StringComparer.Ordinal);

    var update = values.DefineMethod("Update", MethodAttributes.Public | MethodAttributes.Static,
        typeof(void), Type.EmptyTypes);
    var updateIl = update.GetILGenerator();
    foreach (var binding in globalBindings)
    {
        if (double.TryParse(binding.Expression, NumberStyles.Float, CultureInfo.InvariantCulture, out var real))
        {
            updateIl.Emit(OpCodes.Ldc_R8, real);
            updateIl.Emit(OpCodes.Box, typeof(double));
        }
        else
            EmitExternalStaticField(updateIl, binding.Expression, install);
        updateIl.Emit(OpCodes.Stsfld, fields[binding.Name]);
    }
    updateIl.Emit(OpCodes.Ret);

    foreach (var function in functions)
    {
        var method = values.DefineMethod(function.Name, MethodAttributes.Public | MethodAttributes.Static,
            typeof(object), Enumerable.Repeat(typeof(object), function.Args.Length).ToArray());
        var expression = function.Expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (expression.Length < 2 || !expression[0].Contains('.', StringComparison.Ordinal))
            throw new InvalidDataException($"Expected a qualified function application: {function.Expression}");
        var arguments = expression.Skip(1).ToArray();
        var il = method.GetILGenerator();
        foreach (var argument in arguments)
        {
            var argumentIndex = Array.IndexOf(function.Args, argument);
            if (argumentIndex >= 0)
            {
                switch (argumentIndex)
                {
                    case 0: il.Emit(OpCodes.Ldarg_0); break;
                    case 1: il.Emit(OpCodes.Ldarg_1); break;
                    case 2: il.Emit(OpCodes.Ldarg_2); break;
                    case 3: il.Emit(OpCodes.Ldarg_3); break;
                    default: il.Emit(OpCodes.Ldarg, (short)argumentIndex); break;
                }
            }
            else
                EmitExternalStaticField(il, argument, install);
        }
        il.Emit(OpCodes.Tailcall);
        il.Emit(OpCodes.Call, ResolveExternalMethod(expression[0], arguments.Length, install));
        il.Emit(OpCodes.Ret);
    }

    var initializer = values.DefineConstructor(MethodAttributes.Public | MethodAttributes.Static,
        CallingConventions.Standard, Type.EmptyTypes);
    var initializerIl = initializer.GetILGenerator();
    initializerIl.Emit(OpCodes.Call, update);
    initializerIl.Emit(OpCodes.Ret);
    values.CreateType();
    Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
    assembly.Save(outputPath);
}

static void EmitExternalStaticField(MplILGenerator il, string qualifiedName, string install)
{
    var field = ResolveExternalField(qualifiedName, install);
    il.Emit(OpCodes.Ldsfld, field);
}

static FieldInfo ResolveExternalField(string qualifiedName, string install)
{
    var parts = qualifiedName.Split('.', 2);
    if (parts.Length != 2) throw new InvalidDataException($"Expected module.field: {qualifiedName}");
    var assembly = Assembly.LoadFrom(Path.Combine(install, parts[0] + ".dll"));
    var type = assembly.GetType(parts[0] + ".values", throwOnError: true)!;
    return type.GetField(parts[1], BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
        ?? throw new MissingFieldException(type.FullName, parts[1]);
}

static MethodInfo ResolveExternalMethod(string qualifiedName, int parameterCount, string install)
{
    var parts = qualifiedName.Split('.', 2);
    if (parts.Length != 2) throw new InvalidDataException($"Expected module.method: {qualifiedName}");
    var assembly = Assembly.LoadFrom(Path.Combine(install, parts[0] + ".dll"));
    var type = assembly.GetType(parts[0] + ".values", throwOnError: true)!;
    var method = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
        .SingleOrDefault(candidate => candidate.Name == parts[1]
            && candidate.GetParameters().Length == parameterCount
            && candidate.GetParameters().All(parameter => parameter.ParameterType == typeof(object)));
    return method ?? throw new MissingMethodException(type.FullName, parts[1]);
}

static void SetStringAttribute(System.Reflection.Emit.Quire.Mpl.TypeBuilder type,
    ConstructorInfo constructor, string value)
{
    var bytes = Encoding.UTF8.GetBytes(value);
    using var blob = new MemoryStream();
    blob.WriteByte(1);
    blob.WriteByte(0);
    if (bytes.Length < 0x80)
    {
        blob.WriteByte((byte)bytes.Length);
    }
    else if (bytes.Length < 0x4000)
    {
        blob.WriteByte((byte)((bytes.Length >> 8) | 0x80));
        blob.WriteByte((byte)bytes.Length);
    }
    else if (bytes.Length <= 0x1fffffff)
    {
        blob.WriteByte((byte)((bytes.Length >> 24) | 0xc0));
        blob.WriteByte((byte)(bytes.Length >> 16));
        blob.WriteByte((byte)(bytes.Length >> 8));
        blob.WriteByte((byte)bytes.Length);
    }
    else
    {
        throw new InvalidDataException("Custom-attribute string exceeds the CLI length limit.");
    }
    blob.Write(bytes);
    type.SetCustomAttribute(constructor, blob.ToArray());
}

static string ParseInterfaceModuleName(string interfaceText)
{
    var match = Regex.Match(interfaceText, "\\Amodule ([A-Za-z_][A-Za-z0-9_]*) \\(", RegexOptions.CultureInvariant);
    if (!match.Success || !interfaceText.Contains(" where\n{\n", StringComparison.Ordinal)
        || !interfaceText.EndsWith("\n}", StringComparison.Ordinal))
        throw new InvalidDataException("Expected a brace-bodied Mpl module interface.");
    var declarations = interfaceText.Split('\n').Skip(2).SkipLast(1)
        .Select(line => line.Trim()).Where(line => line.Length > 0).ToList();
    if (declarations.Any(line => !line.StartsWith("import ", StringComparison.Ordinal)
        && !line.StartsWith("data ", StringComparison.Ordinal)
        && !Regex.IsMatch(line, @"\Ainfix(?:l|r)?\s+[0-9]+\s+.+;\z", RegexOptions.CultureInvariant)
        && !line.StartsWith("|", StringComparison.Ordinal)
        && !line.Contains(" :: ", StringComparison.Ordinal)))
        throw new InvalidDataException("Interface-only sources support imports, data declarations, and signatures only.");
    if (declarations.Any(line => line.Contains(" = ", StringComparison.Ordinal)
        && !line.StartsWith("data ", StringComparison.Ordinal)))
        throw new InvalidDataException("Interface-only modules cannot contain value definitions.");
    return match.Groups[1].Value;
}

static void CompileHashtable(MplModule program, string output, string install)
{
    Require(program, "Update", "() = ()");
    Require(program, "mlookupInv", "table key = hlookup (Map.Field2 table) key");
    var assembly = CreateAssembly("Quire.Mpl.Modules");
    var module = assembly.DefineDynamicModule("Quire.Mpl.Modules.dll");
    var values = module.DefineType("Quire.Hashtable.values", TypeAttributes.Public | TypeAttributes.Class);
    AddMirrorOf(values, "Hashtable.values, Hashtable");
    var mapHashSet = values.DefineField("$$5d_64c00f80", typeof(object), FieldAttributes.Public | FieldAttributes.Static);
    MplCompilerContext.Fields["$$5d_64c00f80"] = mapHashSet;
    var dAllowType = module.DefineType("Quire.Hashtable.DAllow", TypeAttributes.Public | TypeAttributes.Class);
    AddMirrorOf(dAllowType, "Hashtable.DAllow, Hashtable");
    var dAllowSingleton = dAllowType.DefineField("singleton", dAllowType, FieldAttributes.Public | FieldAttributes.Static);
    var dAllowCtor = dAllowType.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig
        | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard, Type.EmptyTypes);
    dAllowCtor.GetILGenerator().Emit(OpCodes.Ldarg_0);
    dAllowCtor.GetILGenerator().Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)!);
    dAllowCtor.GetILGenerator().Emit(OpCodes.Ret);
    var dAllowInitializer = dAllowType.DefineConstructor(MethodAttributes.Public | MethodAttributes.Static,
        CallingConventions.Standard, Type.EmptyTypes);
    dAllowInitializer.GetILGenerator().Emit(OpCodes.Newobj, dAllowCtor);
    dAllowInitializer.GetILGenerator().Emit(OpCodes.Stsfld, dAllowSingleton);
    dAllowInitializer.GetILGenerator().Emit(OpCodes.Ret);
    var mapType = module.DefineType("Quire.Hashtable.Map", TypeAttributes.Public | TypeAttributes.Class);
    var field1 = mapType.DefineField("Field1", typeof(object), FieldAttributes.Public);
    var field2 = mapType.DefineField("Field2", typeof(object), FieldAttributes.Public);
    var mapCtor = mapType.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig
        | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName, CallingConventions.Standard,
        [typeof(object), typeof(object)]);
    MplCompilerContext.Constructors["Map"] = mapCtor;
    var mapCtorIl = mapCtor.GetILGenerator();
    mapCtorIl.Emit(OpCodes.Ldarg_0);
    mapCtorIl.Emit(OpCodes.Call, typeof(object).GetConstructor(Type.EmptyTypes)!);
    mapCtorIl.Emit(OpCodes.Ldarg_0);
    mapCtorIl.Emit(OpCodes.Ldarg_1);
    mapCtorIl.Emit(OpCodes.Stfld, field1);
    mapCtorIl.Emit(OpCodes.Ldarg_0);
    mapCtorIl.Emit(OpCodes.Ldarg_2);
    mapCtorIl.Emit(OpCodes.Stfld, field2);
    mapCtorIl.Emit(OpCodes.Ret);
    AddMirrorOf(mapType, "Hashtable.Map, Hashtable");
    var update = values.DefineMethod("Update", MethodAttributes.Public | MethodAttributes.Static,
        typeof(void), Type.EmptyTypes);
    update.GetILGenerator().Emit(OpCodes.Ret);
    var hlookup = values.DefineMethod("hlookup", MethodAttributes.Public | MethodAttributes.Static,
        typeof(object), [typeof(object), typeof(object)]);
    CompileExpressionBody(program, "hlookup", hlookup, install);
    var mlookupInv = values.DefineMethod("mlookupInv", MethodAttributes.Public | MethodAttributes.Static,
        typeof(object), [typeof(object), typeof(object)]);
    var il = mlookupInv.GetILGenerator();
    var valueLocal = il.DeclareLocal(typeof(object));
    var mapLocal = il.DeclareLocal(mapType);
    il.Emit(OpCodes.Ldarg_0);
    il.Emit(OpCodes.Stloc, valueLocal);
    il.Emit(OpCodes.Ldloc, valueLocal);
    il.Emit(OpCodes.Stloc, mapLocal);
    il.Emit(OpCodes.Ldloc, mapLocal);
    il.Emit(OpCodes.Ldfld, field2);
    il.Emit(OpCodes.Ldarg_1);
    il.Emit(OpCodes.Tailcall);
    il.Emit(OpCodes.Call, hlookup);
    il.Emit(OpCodes.Ret);
    var mlookup = values.DefineMethod("mlookup", MethodAttributes.Public | MethodAttributes.Static,
        typeof(object), [typeof(object), typeof(object)]);
    var blookupInv = values.DefineMethod("blookupInv", MethodAttributes.Public | MethodAttributes.Static,
        typeof(object), [typeof(object), typeof(object)]);
    var blookup = values.DefineMethod("blookup", MethodAttributes.Public | MethodAttributes.Static,
        typeof(object), [typeof(object), typeof(object)]);
    var safeAdd = values.DefineMethod("safe_add", MethodAttributes.Public | MethodAttributes.Static,
        typeof(object), [typeof(object), typeof(object), typeof(object)]);
    var addWithCheck = values.DefineMethod("add_with_check", MethodAttributes.Public | MethodAttributes.Static,
        typeof(object), [typeof(object), typeof(object), typeof(object), typeof(object)]);
    var localMethods = new Dictionary<string, System.Reflection.Emit.Quire.Mpl.MethodBuilder>(StringComparer.Ordinal)
    {
        ["hlookup"] = hlookup,
        ["safe_add"] = safeAdd,
        ["add_with_check"] = addWithCheck,
    };
    CompileExpressionBody(program, "safe_add", safeAdd, install, localMethods);
    CompileExpressionBody(program, "add_with_check", addWithCheck, install, localMethods);
    var addEntries = values.DefineMethod("add_entries", MethodAttributes.Public | MethodAttributes.Static,
        typeof(object), [typeof(object), typeof(object), typeof(object)]);
    var makeMap = values.DefineMethod("make_map", MethodAttributes.Public | MethodAttributes.Static,
        typeof(object), [typeof(object), typeof(object)]);
    var makeManyToOne = values.DefineMethod("make_many_to_one_map", MethodAttributes.Public | MethodAttributes.Static,
        typeof(object), [typeof(object)]);
    var makeBijective = values.DefineMethod("make_bijective_map", MethodAttributes.Public | MethodAttributes.Static,
        typeof(object), [typeof(object)]);
    localMethods["add_entries"] = addEntries;
    localMethods["make_map"] = makeMap;
    var aliases = new Dictionary<string, Type>(StringComparer.Ordinal)
    {
        ["array"] = typeof(object[]),
        ["Map"] = mapType,
        ["int"] = typeof(int),
        ["Hashtable"] = typeof(System.Collections.Hashtable),
        ["DAllow"] = dAllowType,
    };
    CompileExpressionBody(program, "mlookup", mlookup, install, localMethods, aliases);
    CompileExpressionBody(program, "blookupInv", blookupInv, install, localMethods, aliases);
    CompileExpressionBody(program, "blookup", blookup, install, localMethods, aliases);
    CompileExpressionBody(program, "add_entries", addEntries, install, localMethods, aliases);
    CompileExpressionBody(program, "make_map", makeMap, install, localMethods, aliases);
    CompileExpressionBody(program, "make_many_to_one_map", makeManyToOne, install, localMethods, aliases);
    CompileExpressionBody(program, "make_bijective_map", makeBijective, install, localMethods, aliases);
    var hashSetContains = values.DefineMethod("hash_set_contains", MethodAttributes.Public | MethodAttributes.Static,
        typeof(object), [typeof(object), typeof(object)]);
    CompileExpressionBody(program, "hash_set_contains", hashSetContains, install, localMethods);
    var makeHashMap = values.DefineMethod("make_hash_map", MethodAttributes.Public | MethodAttributes.Static,
        typeof(object), [typeof(object)]);
    CompileExpressionBody(program, "make_hash_map", makeHashMap, install, localMethods, aliases);
    var makeHashSet = values.DefineMethod("make_hash_set", MethodAttributes.Public | MethodAttributes.Static,
        typeof(object), [typeof(object)]);
    localMethods["make_hash_map"] = makeHashMap;
    CompileExpressionBody(program, "make_hash_set", makeHashSet, install, localMethods, aliases);
    var mapHashSetAdapter = values.DefineMethod("$$5d_64c00f8", MethodAttributes.Public | MethodAttributes.Static,
        typeof(object), [typeof(object)]);
    CompileExpressionBody(program, "$$5d_64c00f8", mapHashSetAdapter, install, localMethods, aliases);
    var initializer = values.DefineConstructor(MethodAttributes.Public | MethodAttributes.Static,
        CallingConventions.Standard, Type.EmptyTypes);
    var initializerIl = initializer.GetILGenerator();
    var runtime = Assembly.LoadFrom(Path.Combine(install, "mplrun.dll"));
    var genericDelegate = runtime.GetType("Mpl.Run.F`2", throwOnError: true)!.MakeGenericType(typeof(object), typeof(object));
    var delegateCtor = genericDelegate.GetConstructor([typeof(object), typeof(IntPtr)])
        ?? throw new MissingMethodException(genericDelegate.FullName, ".ctor(object, IntPtr)");
    var makeDelegate = runtime.GetType("Mpl.Run.DelegateFunction", throwOnError: true)!
        .GetMethod("make", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
            binder: null, [typeof(Delegate), typeof(int)], modifiers: null)
        ?? throw new MissingMethodException("Mpl.Run.DelegateFunction.make(Delegate, int)");
    initializerIl.Emit(OpCodes.Ldnull);
    initializerIl.Emit(OpCodes.Ldftn, mapHashSetAdapter);
    initializerIl.Emit(OpCodes.Newobj, delegateCtor);
    initializerIl.Emit(OpCodes.Ldc_I4_1);
    initializerIl.Emit(OpCodes.Call, makeDelegate);
    initializerIl.Emit(OpCodes.Stsfld, mapHashSet);
    initializerIl.Emit(OpCodes.Call, update);
    initializerIl.Emit(OpCodes.Ret);
    mapType.CreateType();
    values.CreateType();
    assembly.Save(output);
}

static void CompileUnitSystemUs(MplModule program, string output, string install)
{
    Require(program, "op_L", "value = McdRun.times_unit value McdUnitSystemSI.mc_ft");
    var si = Assembly.LoadFrom(Path.Combine(install, "mcdunitsystemsi.dll"));
    var mcdRun = Assembly.LoadFrom(Path.Combine(install, "mcdrun.dll"));
    var siValues = si.GetType("McdUnitSystemSI.values", throwOnError: true)!;
    var runValues = mcdRun.GetType("McdRun.values", throwOnError: true)!;
    var feet = siValues.GetField("mc_ft", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
        ?? throw new MissingFieldException(siValues.FullName, "mc_ft");
    var timesUnit = runValues.GetMethod("times_unit", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
        binder: null, [typeof(object), typeof(object)], modifiers: null)
        ?? throw new MissingMethodException(runValues.FullName, "times_unit");
    var assembly = CreateAssembly("Quire.McdUnitSystemUS");
    var module = assembly.DefineDynamicModule("Quire.McdUnitSystemUS.dll");
    var values = module.DefineType("Quire.McdUnitSystemUS.values", TypeAttributes.Public | TypeAttributes.Class);
    AddMirrorOf(values, "McdUnitSystemUS.values, McdUnitSystemUS");
    var opL = values.DefineMethod("op_L", MethodAttributes.Public | MethodAttributes.Static,
        typeof(object), [typeof(object)]);
    var il = opL.GetILGenerator();
    il.Emit(OpCodes.Ldarg_0);
    il.Emit(OpCodes.Ldsfld, feet);
    il.Emit(OpCodes.Tailcall);
    il.Emit(OpCodes.Call, timesUnit);
    il.Emit(OpCodes.Ret);
    values.CreateType();
    assembly.Save(output);
}

static MplAssemblyBuilder CreateAssembly(string name) =>
    MplAssemblyBuilder.DefineDynamicAssembly(new AssemblyName(name), (AssemblyBuilderAccess)0);

static void AddMirrorOf(System.Reflection.Emit.Quire.Mpl.TypeBuilder type, string originalType)
{
    var mirrorAssembly = Assembly.LoadFrom(Path.Combine(AppContext.BaseDirectory, "Quire.Mirror.dll"));
    var attributeType = mirrorAssembly.GetType("Quire.Mirror.MirrorOfAttribute", throwOnError: true)!;
    var constructor = attributeType.GetConstructor([typeof(string)])
        ?? throw new MissingMethodException(attributeType.FullName, ".ctor(string)");
    var text = Encoding.UTF8.GetBytes(originalType);
    if (text.Length >= 0x80) throw new InvalidOperationException("prototype MirrorOf string is too long");
    var blob = new byte[text.Length + 5];
    blob[0] = 1;
    blob[2] = (byte)text.Length;
    text.CopyTo(blob, 3);
    type.SetCustomAttribute(constructor, blob);
}

static void Require(MplModule program, string name, string body)
{
    if (!program.Functions.TryGetValue(name, out var actual) || actual != body)
        throw new InvalidDataException($"{program.Module}.{name} must have body: {body}");
}

static void CompileExpressionBody(MplModule program, string name,
    System.Reflection.Emit.Quire.Mpl.MethodBuilder method, string install,
    Dictionary<string, System.Reflection.Emit.Quire.Mpl.MethodBuilder>? methods = null,
    IReadOnlyDictionary<string, Type>? typeAliases = null)
{
    if (!program.Functions.TryGetValue(name, out var body))
        throw new InvalidDataException($"Missing Mpl definition {name}.");
    var bodyStart = body.IndexOf(" = ", StringComparison.Ordinal);
    if (bodyStart < 0) throw new InvalidDataException($"Missing function arguments in definition {name}.");
    var arguments = body[..bodyStart].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    var expression = MplExpressionParser.Parse(body[(bodyStart + 3)..]);
    var il = method.GetILGenerator();
    var locals = new Dictionary<string, System.Reflection.Emit.Quire.Mpl.LocalBuilder>(StringComparer.Ordinal);
    EmitReturningExpression(expression, il, locals, arguments,
        methods ?? new Dictionary<string, System.Reflection.Emit.Quire.Mpl.MethodBuilder>(StringComparer.Ordinal),
        typeAliases ?? new Dictionary<string, Type>(StringComparer.Ordinal), install, name);
}

static void EmitReturningExpression(MplExpr expression,
    System.Reflection.Emit.Quire.Mpl.ILGenerator il,
    Dictionary<string, System.Reflection.Emit.Quire.Mpl.LocalBuilder> locals, string[] arguments,
    Dictionary<string, System.Reflection.Emit.Quire.Mpl.MethodBuilder> methods,
    IReadOnlyDictionary<string, Type> typeAliases, string install, string methodName)
{
    switch (expression)
    {
        case MplLet let:
            EmitValue(let.Value, il, locals, arguments, methods, typeAliases, install);
            var localType = let.TypeName is not null && typeAliases.TryGetValue(let.TypeName, out var declaredType)
                ? declaredType : typeof(object);
            var local = il.DeclareLocal(localType);
            locals.Add(let.Name, local);
            il.Emit(OpCodes.Stloc, local);
            EmitReturningExpression(let.Body, il, locals, arguments, methods, typeAliases, install, methodName);
            return;
        case MplIfNull conditional:
            EmitNullBoolean(conditional.Value, il, locals, install);
            var elseLabel = il.DefineLabel();
            il.Emit(OpCodes.Brfalse, elseLabel);
            EmitValue(conditional.WhenNull, il, locals, arguments, methods, typeAliases, install);
            il.Emit(OpCodes.Ret);
            il.MarkLabel(elseLabel);
            EmitValue(conditional.WhenNonNull, il, locals, arguments, methods, typeAliases, install);
            il.Emit(OpCodes.Ret);
            return;
        case MplIfTruthy conditional:
        {
            var thenLabel = il.DefineLabel();
            if (conditional.Condition is MplCall)
            {
                EmitValue(conditional.Condition, il, locals, arguments, methods, typeAliases, install);
                il.Emit(OpCodes.Dup);
                il.Emit(OpCodes.Brtrue, thenLabel);
                il.Emit(OpCodes.Pop);
                EmitReturningExpression(conditional.WhenFalse, il, locals, arguments, methods, typeAliases, install, methodName);
                il.MarkLabel(thenLabel);
                il.Emit(OpCodes.Pop);
                EmitReturningExpression(conditional.WhenTrue, il, locals, arguments, methods, typeAliases, install, methodName);
            }
            else
            {
                EmitValue(conditional.Condition, il, locals, arguments, methods, typeAliases, install);
                il.Emit(OpCodes.Brtrue, thenLabel);
                EmitReturningExpression(conditional.WhenFalse, il, locals, arguments, methods, typeAliases, install, methodName);
                il.MarkLabel(thenLabel);
                EmitReturningExpression(conditional.WhenTrue, il, locals, arguments, methods, typeAliases, install, methodName);
            }
            return;
        }
        case MplSequence sequence:
            foreach (var item in sequence.Expressions.Take(sequence.Expressions.Count - 1))
                EmitStatement(item, il, locals, arguments, methods, typeAliases, install);
            EmitReturningExpression(sequence.Expressions[^1], il, locals, arguments, methods, typeAliases, install, methodName);
            return;
        case MplCaseCons pattern:
        {
            var runtime = Assembly.LoadFrom(Path.Combine(install, "mplrun.dll"));
            var consType = runtime.GetType("Mpl.Run.Cons", throwOnError: true)!;
            var emptyLabel = il.DefineLabel();
            EmitValue(pattern.Value, il, locals, arguments, methods, typeAliases, install);
            il.Emit(OpCodes.Dup);
            il.Emit(OpCodes.Brfalse, emptyLabel);
            var consLocal = il.DeclareLocal(consType);
            locals.Add(pattern.Name, consLocal);
            il.Emit(OpCodes.Stloc, consLocal);
            EmitReturningExpression(pattern.WhenCons, il, locals, arguments, methods, typeAliases, install, methodName);
            il.MarkLabel(emptyLabel);
            il.Emit(OpCodes.Pop);
            EmitReturningExpression(pattern.WhenEmpty, il, locals, arguments, methods, typeAliases, install, methodName);
            return;
        }
        case MplForList loop:
            EmitForList(loop, il, locals, arguments, methods, typeAliases, install);
            return;
        default:
            EmitTailValue(expression, il, locals, arguments, methods, typeAliases, install, methodName);
            return;
    }
}

static void EmitForList(MplForList loop,
    System.Reflection.Emit.Quire.Mpl.ILGenerator il,
    Dictionary<string, System.Reflection.Emit.Quire.Mpl.LocalBuilder> locals, string[] arguments,
    Dictionary<string, System.Reflection.Emit.Quire.Mpl.MethodBuilder> methods,
    IReadOnlyDictionary<string, Type> typeAliases, string install)
{
    var runtime = Assembly.LoadFrom(Path.Combine(install, "mplrun.dll"));
    var consType = runtime.GetType("Mpl.Run.Cons", throwOnError: true)!;
    var collectionLocal = il.DeclareLocal(typeof(object));
    var resultLocal = il.DeclareLocal(typeof(object));
    var consLocal = il.DeclareLocal(consType);
    var elementLocal = il.DeclareLocal(typeof(object[]));
    var capacityLocal = il.DeclareLocal(typeof(int));
    locals.Add(loop.CollectionName, collectionLocal);
    locals.Add(loop.ResultName, resultLocal);
    locals.Add(loop.ElementName, elementLocal);

    var initialize = il.DefineLabel();
    var head = il.DefineLabel();
    var exitLoop = il.DefineLabel();
    il.Emit(OpCodes.Br, initialize);
    il.MarkLabel(head);
    il.Emit(OpCodes.Ldloc, collectionLocal);
    il.Emit(OpCodes.Dup);
    il.Emit(OpCodes.Brfalse, exitLoop);
    il.Emit(OpCodes.Stloc, consLocal);
    var consField = consType.GetField("Field1", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        ?? throw new MissingFieldException(consType.FullName, "Field1");
    il.Emit(OpCodes.Ldloc, consLocal);
    il.Emit(OpCodes.Ldfld, consField);
    il.Emit(OpCodes.Stloc, elementLocal);
    EmitStatement(loop.Action, il, locals, arguments, methods, typeAliases, install);
    var tailField = consType.GetField("Field2", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
        ?? throw new MissingFieldException(consType.FullName, "Field2");
    il.Emit(OpCodes.Ldloc, consLocal);
    il.Emit(OpCodes.Ldfld, tailField);
    il.Emit(OpCodes.Ldloc, resultLocal);
    il.Emit(OpCodes.Stloc, resultLocal);
    il.Emit(OpCodes.Stloc, collectionLocal);
    il.Emit(OpCodes.Br, head);

    il.MarkLabel(exitLoop);
    il.Emit(OpCodes.Pop);
    il.Emit(OpCodes.Ldloc, resultLocal);
    il.Emit(OpCodes.Ret);

    il.MarkLabel(initialize);
    locals.Remove(loop.CollectionName);
    locals.Remove(loop.ResultName);
    locals.Remove(loop.ElementName);
    EmitValue(loop.Capacity, il, locals, arguments, methods, typeAliases, install);
    il.Emit(OpCodes.Stloc, capacityLocal);
    EmitValue(loop.Collection, il, locals, arguments, methods, typeAliases, install);
    var hashtableType = typeof(System.Collections.Hashtable);
    var hashtableCtor = hashtableType.GetConstructor([typeof(int)])
        ?? throw new MissingMethodException(hashtableType.FullName, ".ctor(int)");
    il.Emit(OpCodes.Ldloc, capacityLocal);
    il.Emit(OpCodes.Newobj, hashtableCtor);
    il.Emit(OpCodes.Stloc, resultLocal);
    il.Emit(OpCodes.Stloc, collectionLocal);
    il.Emit(OpCodes.Br, head);
    locals.Add(loop.CollectionName, collectionLocal);
    locals.Add(loop.ResultName, resultLocal);
    locals.Add(loop.ElementName, elementLocal);
}

static void EmitTailValue(MplExpr expression,
    System.Reflection.Emit.Quire.Mpl.ILGenerator il,
    Dictionary<string, System.Reflection.Emit.Quire.Mpl.LocalBuilder> locals, string[] arguments,
    Dictionary<string, System.Reflection.Emit.Quire.Mpl.MethodBuilder> methods,
    IReadOnlyDictionary<string, Type> typeAliases, string install, string methodName)
{
    if (expression is MplTail tailExpression)
    {
        if (tailExpression.Expression is not MplCall explicitCall)
            throw new InvalidDataException("tail applies to a function application.");
        EmitCall(explicitCall, il, locals, arguments, methods, typeAliases, install, tail: true);
        il.Emit(OpCodes.Ret);
        return;
    }
    if (expression is MplCall call && call.Function is MplVariable { Name: "Just" })
    {
        EmitValue(expression, il, locals, arguments, methods, typeAliases, install);
        il.Emit(OpCodes.Ret);
        return;
    }
    if (expression is MplCall tailCall)
    {
        EmitCall(tailCall, il, locals, arguments, methods, typeAliases, install, tail: false);
        il.Emit(OpCodes.Ret);
        return;
    }
    EmitValue(expression, il, locals, arguments, methods, typeAliases, install);
    il.Emit(OpCodes.Ret);
}

static void EmitStatement(MplExpr expression,
    System.Reflection.Emit.Quire.Mpl.ILGenerator il,
    Dictionary<string, System.Reflection.Emit.Quire.Mpl.LocalBuilder> locals, string[] arguments,
    Dictionary<string, System.Reflection.Emit.Quire.Mpl.MethodBuilder> methods,
    IReadOnlyDictionary<string, Type> typeAliases, string install)
{
    if (expression is MplSetIndex set)
    {
        EmitValue(set.Table, il, locals, arguments, methods, typeAliases, install);
        EmitValue(set.Key, il, locals, arguments, methods, typeAliases, install);
        EmitValue(set.Value, il, locals, arguments, methods, typeAliases, install);
        var setItem = typeof(System.Collections.Hashtable).GetMethod("set_Item",
            BindingFlags.Public | BindingFlags.Instance, binder: null, [typeof(object), typeof(object)], modifiers: null)
            ?? throw new MissingMethodException("System.Collections.Hashtable.set_Item");
        il.Emit(OpCodes.Call, setItem);
        return;
    }
    if (expression is MplCall call)
    {
        EmitCall(call, il, locals, arguments, methods, typeAliases, install, tail: false);
        if (CallReturnType(call, methods, install) != typeof(void)) il.Emit(OpCodes.Pop);
        return;
    }
    throw new NotSupportedException($"Statement lowering is not implemented for {expression}.");
}

static Type CallReturnType(MplCall call,
    Dictionary<string, System.Reflection.Emit.Quire.Mpl.MethodBuilder> methods, string install)
{
    if (call.Function is MplVariable { Name: "Just" }) return typeof(object);
    if (call.Function is MplVariable local && methods.TryGetValue(local.Name, out var method)) return method.ReturnType;
    var name = ((MplVariable)call.Function).Name;
    var dot = name.LastIndexOf('.');
    var assemblyName = name[..dot];
    var methodName = name[(dot + 1)..];
    var assembly = Assembly.LoadFrom(Path.Combine(install, assemblyName + ".dll"));
    var type = assembly.GetType(assemblyName + ".values", throwOnError: true)!;
    var external = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
        .Single(candidate => candidate.Name == methodName && candidate.GetParameters().Length == call.Arguments.Count);
    return external.ReturnType;
}

static void EmitCall(MplCall call,
    System.Reflection.Emit.Quire.Mpl.ILGenerator il,
    Dictionary<string, System.Reflection.Emit.Quire.Mpl.LocalBuilder> locals, string[] arguments,
    Dictionary<string, System.Reflection.Emit.Quire.Mpl.MethodBuilder> methods,
    IReadOnlyDictionary<string, Type> typeAliases, string install, bool tail)
{
    if (call.Function is not MplVariable function) throw new NotSupportedException("Function values are not supported yet.");
    foreach (var argument in call.Arguments) EmitValue(argument, il, locals, arguments, methods, typeAliases, install);
    if (function.Name == "Just")
    {
        var prelude = Assembly.LoadFrom(Path.Combine(install, "MplPrelude.dll"));
        var justCtor = prelude.GetType("MplPrelude.Just", throwOnError: true)!
            .GetConstructor([typeof(object)]) ?? throw new MissingMethodException("MplPrelude.Just..ctor(object)");
        il.Emit(OpCodes.Newobj, justCtor);
        return;
    }
    if (methods.TryGetValue(function.Name, out var localMethod))
    {
        if (tail) il.Emit(OpCodes.Tailcall);
        il.Emit(OpCodes.Call, localMethod);
        return;
    }
    if (function.Name == "length")
    {
        var prelude = Assembly.LoadFrom(Path.Combine(install, "MplPrelude.dll"));
        var values = prelude.GetType("MplPrelude.values", throwOnError: true)!;
        var length = values.GetMethod("length", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
            binder: null, [typeof(object)], modifiers: null)
            ?? throw new MissingMethodException(values.FullName, "length");
        if (tail) il.Emit(OpCodes.Tailcall);
        il.Emit(OpCodes.Call, length);
        il.Emit(OpCodes.Unbox, typeof(int));
        il.Emit(OpCodes.Ldobj, typeof(int));
        return;
    }
    if (function.Name == "AppException.Raise")
    {
        var runtime = Assembly.LoadFrom(Path.Combine(install, "mplrun.dll"));
        var appException = runtime.GetType("Mpl.Run.AppException", throwOnError: true)!;
        var raise = appException.GetMethod("Raise", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
            binder: null, [typeof(string)], modifiers: null) ?? throw new MissingMethodException(appException.FullName, "Raise");
        if (tail) il.Emit(OpCodes.Tailcall);
        il.Emit(OpCodes.Call, raise);
        return;
    }
    if (function.Name.Contains('.', StringComparison.Ordinal))
    {
        var dot = function.Name.LastIndexOf('.');
        var assemblyName = function.Name[..dot];
        var memberName = function.Name[(dot + 1)..];
        var assembly = Assembly.LoadFrom(Path.Combine(install, assemblyName + ".dll"));
        var type = assembly.GetType(assemblyName + ".values", throwOnError: true)!;
        var external = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
            .Single(candidate => candidate.Name == memberName && candidate.GetParameters().Length == call.Arguments.Count
                && candidate.GetParameters().All(parameter => parameter.ParameterType == typeof(object)));
        if (tail) il.Emit(OpCodes.Tailcall);
        il.Emit(OpCodes.Call, external);
        return;
    }
    throw new NotSupportedException($"Call target {function.Name} has not been lowered.");
}

static void EmitNullBoolean(string name,
    System.Reflection.Emit.Quire.Mpl.ILGenerator il,
    Dictionary<string, System.Reflection.Emit.Quire.Mpl.LocalBuilder> locals, string install)
{
    var local = locals[name];
    var nonNull = il.DefineLabel();
    var end = il.DefineLabel();
    var runtime = Assembly.LoadFrom(Path.Combine(install, "mplrun.dll"));
    var truth = runtime.GetType("Mpl.Run.True", throwOnError: true)!
        .GetField("singleton", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
        ?? throw new MissingFieldException("Mpl.Run.True", "singleton");
    il.Emit(OpCodes.Ldloc, local);
    il.Emit(OpCodes.Dup);
    il.Emit(OpCodes.Brtrue, nonNull);
    il.Emit(OpCodes.Pop);
    il.Emit(OpCodes.Ldsfld, truth);
    il.Emit(OpCodes.Br, end);
    il.MarkLabel(nonNull);
    il.Emit(OpCodes.Pop);
    il.Emit(OpCodes.Ldnull);
    il.MarkLabel(end);
}

static void EmitValue(MplExpr expression,
    System.Reflection.Emit.Quire.Mpl.ILGenerator il,
    Dictionary<string, System.Reflection.Emit.Quire.Mpl.LocalBuilder> locals, string[] arguments,
    Dictionary<string, System.Reflection.Emit.Quire.Mpl.MethodBuilder> methods,
    IReadOnlyDictionary<string, Type> typeAliases, string install)
{
    switch (expression)
    {
        case MplVariable variable when locals.TryGetValue(variable.Name, out var local):
            il.Emit(OpCodes.Ldloc, local);
            return;
        case MplVariable variable when Array.IndexOf(arguments, variable.Name) is var argumentIndex && argumentIndex >= 0:
            EmitLoadArgument(il, argumentIndex);
            return;
        case MplVariable { Name: "True.singleton" }:
            var runtime = Assembly.LoadFrom(Path.Combine(install, "mplrun.dll"));
            var truth = runtime.GetType("Mpl.Run.True", throwOnError: true)!
                .GetField("singleton", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new MissingFieldException("Mpl.Run.True", "singleton");
            il.Emit(OpCodes.Ldsfld, truth);
            return;
        case MplVariable { Name: "DAllow.singleton" }:
            il.Emit(OpCodes.Ldsfld, typeAliases["DAllow"].GetField("singleton",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static)
                ?? throw new MissingFieldException("Quire.Hashtable.DAllow", "singleton"));
            return;
        case MplVariable variable when MplCompilerContext.Fields.TryGetValue(variable.Name, out var staticField):
            il.Emit(OpCodes.Ldsfld, staticField);
            return;
        case MplNull:
            il.Emit(OpCodes.Ldnull);
            return;
        case MplIfTruthy conditional:
        {
            var elseLabel = il.DefineLabel();
            var endLabel = il.DefineLabel();
            EmitValue(conditional.Condition, il, locals, arguments, methods, typeAliases, install);
            il.Emit(OpCodes.Brfalse, elseLabel);
            EmitValue(conditional.WhenTrue, il, locals, arguments, methods, typeAliases, install);
            il.Emit(OpCodes.Br, endLabel);
            il.MarkLabel(elseLabel);
            EmitValue(conditional.WhenFalse, il, locals, arguments, methods, typeAliases, install);
            il.MarkLabel(endLabel);
            return;
        }
        case MplNew creation:
        {
            var type = typeAliases.TryGetValue(creation.TypeName, out var aliasedType)
                ? aliasedType : throw new InvalidDataException($"Unknown constructor type {creation.TypeName}.");
            var argumentTypes = creation.Arguments.Select(argument => ExpressionType(argument, locals, typeAliases)).ToArray();
            foreach (var argument in creation.Arguments)
                EmitValue(argument, il, locals, arguments, methods, typeAliases, install);
            var constructor = MplCompilerContext.Constructors.TryGetValue(creation.TypeName, out var emittedConstructor)
                ? emittedConstructor
                : type.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                    .Single(candidate => candidate.GetParameters().Length == argumentTypes.Length
                        && candidate.GetParameters().Select(parameter => parameter.ParameterType).Zip(argumentTypes)
                        .All(pair => pair.First == pair.Second || pair.First == typeof(object)
                            || pair.First.IsAssignableFrom(pair.Second)));
            il.Emit(OpCodes.Newobj, constructor);
            return;
        }
        case MplInteger integer:
            switch (integer.Value)
            {
                case 0: il.Emit(OpCodes.Ldc_I4_0); break;
                case 1: il.Emit(OpCodes.Ldc_I4_1); break;
                default: il.Emit(OpCodes.Ldc_I4, integer.Value); break;
            }
            return;
        case MplField field:
            EmitValue(field.Receiver, il, locals, arguments, methods, typeAliases, install);
            il.Emit(OpCodes.Ldfld, ResolveField(field.Receiver, field.Name, locals, arguments, typeAliases, install));
            return;
        case MplIndex index:
        {
            var tableType = ExpressionType(index.Table, locals, typeAliases);
            EmitValue(index.Table, il, locals, arguments, methods, typeAliases, install);
            EmitValue(index.Key, il, locals, arguments, methods, typeAliases, install);
            if (tableType == typeof(object[]))
            {
                il.Emit(OpCodes.Ldelem_Ref);
                return;
            }
            var getItem = typeof(System.Collections.Hashtable).GetMethod("get_Item",
                BindingFlags.Public | BindingFlags.Instance, binder: null, [typeof(object)], modifiers: null)
                ?? throw new MissingMethodException("System.Collections.Hashtable.get_Item");
            il.Emit(OpCodes.Call, getItem);
            return;
        }
        case MplString value:
            il.Emit(OpCodes.Ldstr, value.Value);
            return;
        case MplArray array:
            EmitInteger(array.Items.Count, il);
            il.Emit(OpCodes.Newarr, typeof(object));
            for (var i = 0; i < array.Items.Count; i++)
            {
                il.Emit(OpCodes.Dup);
                EmitInteger(i, il);
                EmitValue(array.Items[i], il, locals, arguments, methods, typeAliases, install);
                il.Emit(OpCodes.Stelem_Ref);
            }
            return;
        case MplCall call:
            EmitCall(call, il, locals, arguments, methods, typeAliases, install, tail: false);
            return;
        default:
            throw new NotSupportedException($"Expression lowering is not implemented for {expression}.");
    }
}

static Type ExpressionType(MplExpr expression,
    Dictionary<string, System.Reflection.Emit.Quire.Mpl.LocalBuilder> locals,
    IReadOnlyDictionary<string, Type> typeAliases)
{
    return expression switch
    {
        MplVariable variable when locals.TryGetValue(variable.Name, out var local) => local.LocalType,
        MplVariable variable when typeAliases.TryGetValue(variable.Name, out var type) => type,
        MplInteger => typeof(int),
        MplString => typeof(string),
        MplArray => typeof(object[]),
        MplNew creation when typeAliases.TryGetValue(creation.TypeName, out var constructedType) => constructedType,
        MplIndex => typeof(object),
        _ => typeof(object),
    };
}

static FieldInfo ResolveField(MplExpr receiver, string name,
    Dictionary<string, System.Reflection.Emit.Quire.Mpl.LocalBuilder> locals, string[] arguments,
    IReadOnlyDictionary<string, Type> typeAliases, string install)
{
    Type type;
    if (receiver is MplVariable variable && locals.TryGetValue(variable.Name, out var local))
        type = local.LocalType;
    else if (receiver is MplVariable alias && typeAliases.TryGetValue(alias.Name, out var aliasType))
        type = aliasType;
    else
        type = ExpressionType(receiver, locals, typeAliases);
    return type.GetField(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
        ?? throw new MissingFieldException(type.FullName, name);
}

static void EmitLoadArgument(System.Reflection.Emit.Quire.Mpl.ILGenerator il, int index)
{
    switch (index)
    {
        case 0: il.Emit(OpCodes.Ldarg_0); break;
        case 1: il.Emit(OpCodes.Ldarg_1); break;
        case 2: il.Emit(OpCodes.Ldarg_2); break;
        case 3: il.Emit(OpCodes.Ldarg_3); break;
        default: il.Emit(OpCodes.Ldarg, (short)index); break;
    }
}

static void EmitInteger(int value, System.Reflection.Emit.Quire.Mpl.ILGenerator il)
{
    switch (value)
    {
        case 0: il.Emit(OpCodes.Ldc_I4_0); break;
        case 1: il.Emit(OpCodes.Ldc_I4_1); break;
        case 2: il.Emit(OpCodes.Ldc_I4_2); break;
        default: il.Emit(OpCodes.Ldc_I4, value); break;
    }
}

internal sealed record MplModule(string Module, Dictionary<string, string> Functions);
internal sealed record MplDataConstructor(string TypeName, string DisplayName, IReadOnlyList<Type> ArgumentTypes,
    bool Singleton, bool UnitAware);

internal abstract record MplExpr;
internal sealed record MplVariable(string Name) : MplExpr;
internal sealed record MplNull : MplExpr;
internal sealed record MplIndex(MplExpr Table, MplExpr Key) : MplExpr;
internal sealed record MplLet(string Name, string? TypeName, MplExpr Value, MplExpr Body) : MplExpr;
internal sealed record MplIfNull(string Value, MplExpr WhenNull, MplExpr WhenNonNull) : MplExpr;
internal sealed record MplIfTruthy(MplExpr Condition, MplExpr WhenTrue, MplExpr WhenFalse) : MplExpr;
internal sealed record MplCall(MplExpr Function, IReadOnlyList<MplExpr> Arguments) : MplExpr;
internal sealed record MplSetIndex(MplExpr Table, MplExpr Key, MplExpr Value) : MplExpr;
internal sealed record MplSequence(IReadOnlyList<MplExpr> Expressions) : MplExpr;
internal sealed record MplString(string Value) : MplExpr;
internal sealed record MplInteger(int Value) : MplExpr;
internal sealed record MplArray(IReadOnlyList<MplExpr> Items) : MplExpr;
internal sealed record MplField(MplExpr Receiver, string Name) : MplExpr;
internal sealed record MplCaseCons(MplExpr Value, string Name, MplExpr WhenCons, MplExpr WhenEmpty) : MplExpr;
internal sealed record MplTail(MplExpr Expression) : MplExpr;
internal sealed record MplNew(string TypeName, IReadOnlyList<MplExpr> Arguments) : MplExpr;
internal sealed record MplForList(string ElementName, MplExpr Collection, string CollectionName,
    MplExpr Capacity, string ResultName, MplExpr Action) : MplExpr;

internal static class MplExpressionParser
{
    public static MplExpr Parse(string source) => new Parser(source).Parse();

    private sealed class Parser
    {
        private readonly string[] _tokens;
        private int _position;

        public Parser(string source) => _tokens = Regex.Matches(source,
                """==|:=|->|[A-Za-z_$][A-Za-z0-9_.$]*|[0-9]+|"[^"]*"|\[|\]|\(|\)|,|;|\||:|=""")
            .Select(match => match.Value).ToArray();

        public MplExpr Parse()
        {
            var expression = ParseExpression();
            if (_position != _tokens.Length) throw new InvalidDataException("Trailing tokens in Mpl expression.");
            return expression;
        }

        private MplExpr ParseExpression()
        {
            if (Take("tail")) return new MplTail(ParseExpression());
            if (Take("case"))
            {
                var value = ReadIdentifier();
                Expect("of");
                Expect("Cons");
                var name = ReadIdentifier();
                Expect("->");
                var whenCons = ParseExpression();
                Expect("|");
                Expect("null");
                Expect("->");
                return new MplCaseCons(new MplVariable(value), name, whenCons, ParseExpression());
            }
            if (Take("let"))
            {
                var name = ReadIdentifier();
                string? typeName = null;
                if (Take(":")) typeName = ReadIdentifier();
                Expect("=");
                var value = ParseExpression();
                Expect("in");
                return new MplLet(name, typeName, value, ParseExpression());
            }
            if (Take("if"))
            {
                var condition = ParseApplication();
                if (Take("=="))
                {
                    Expect("null");
                    Expect("then");
                    var whenNull = ParseExpression();
                    Expect("else");
                    return new MplIfNull(RequireVariable(condition), whenNull, ParseExpression());
                }
                Expect("then");
                var whenTrue = ParseExpression();
                Expect("else");
                return new MplIfTruthy(condition, whenTrue, ParseExpression());
            }
            if (Take("for"))
            {
                var elementName = ReadIdentifier();
                Expect("in");
                var collection = ParseApplication();
                Expect("using");
                var capacity = ParseApplication();
                Expect("into");
                var resultName = ReadIdentifier();
                Expect("do");
                return new MplForList(elementName, collection, RequireVariable(collection), capacity,
                    resultName, ParseExpression());
            }
            var expressions = new List<MplExpr> { ParseApplication() };
            while (Take(";")) expressions.Add(ParseApplication());
            return expressions.Count == 1 ? expressions[0] : new MplSequence(expressions);
        }

        private MplExpr ParseApplication()
        {
            var function = ParseAtom();
            if (Take(":="))
            {
                if (function is not MplIndex index) throw new InvalidDataException("Assignment target must be an indexed map.");
                return new MplSetIndex(index.Table, index.Key, ParseAtom());
            }
            var arguments = new List<MplExpr>();
            while (_position < _tokens.Length && _tokens[_position] is not ("in" or "then" or "else" or "=="
                or "using" or "into" or "do" or ")" or "]" or "," or ";" or "|"))
                arguments.Add(ParseAtom());
            return arguments.Count == 0 ? function : new MplCall(function, arguments);
        }

        private MplExpr ParseAtom()
        {
            if (Take("new"))
            {
                var typeName = ReadIdentifier();
                Expect("(");
                var constructorArguments = new List<MplExpr>();
                if (!Take(")"))
                {
                    do constructorArguments.Add(ParseExpression()); while (Take(","));
                    Expect(")");
                }
                return new MplNew(typeName, constructorArguments);
            }
            if (Take("("))
            {
                var expression = ParseExpression();
                Expect(")");
                return expression;
            }
            if (Take("["))
            {
                var items = new List<MplExpr> { ParseExpression() };
                while (Take(",")) items.Add(ParseExpression());
                Expect("]");
                return new MplArray(items);
            }
            if (Take("null")) return new MplNull();
            if (_position < _tokens.Length && _tokens[_position].StartsWith('"'))
                return new MplString(_tokens[_position++][1..^1]);
            if (_position < _tokens.Length && int.TryParse(_tokens[_position], out var integer))
            {
                _position++;
                return new MplInteger(integer);
            }
            var name = ReadIdentifier();
            MplExpr variable = name.Contains('.', StringComparison.Ordinal) && char.IsLower(name[0])
                ? new MplField(new MplVariable(name[..name.IndexOf('.')]), name[(name.IndexOf('.') + 1)..])
                : new MplVariable(name);
            if (Take("["))
            {
                var key = ParseAtom();
                Expect("]");
                return new MplIndex(variable, key);
            }
            return variable;
        }

        private static string RequireVariable(MplExpr expression) => expression is MplVariable variable
            ? variable.Name : throw new InvalidDataException("Null comparison currently requires a variable.");

        private string ReadIdentifier()
        {
            if (_position >= _tokens.Length || !Regex.IsMatch(_tokens[_position], @"\A[A-Za-z_$][A-Za-z0-9_.$]*\z"))
                throw new InvalidDataException($"Expected an identifier in Mpl expression at token {_position}: "
                    + (_position < _tokens.Length ? _tokens[_position] : "<end>"));
            return _tokens[_position++];
        }

        private bool Take(string token)
        {
            if (_position >= _tokens.Length || _tokens[_position] != token) return false;
            _position++;
            return true;
        }

        private void Expect(string token)
        {
            if (!Take(token)) throw new InvalidDataException($"Expected '{token}' in Mpl expression.");
        }
    }
}

internal static class MplCompilerContext
{
    public static Dictionary<string, ConstructorInfo> Constructors { get; } = new(StringComparer.Ordinal);
    public static Dictionary<string, System.Reflection.Emit.Quire.Mpl.FieldBuilder> Fields { get; } = new(StringComparer.Ordinal);
}

internal static class MplSubsetParser
{
    public static MplModule Parse(string source)
    {
        string? module = null;
        var functions = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var raw in source.Split('\n'))
        {
            var line = raw.Trim();
            if (line.Length == 0 || line.StartsWith("--", StringComparison.Ordinal)) continue;
            if (line.StartsWith("module ", StringComparison.Ordinal))
            {
                if (module != null) throw new InvalidDataException("A prototype source has exactly one module.");
                var header = line[7..].Trim();
                if (header.EndsWith(" where", StringComparison.Ordinal)) header = header[..^6].TrimEnd();
                var exports = header.IndexOf('(');
                if (exports >= 0) header = header[..exports].TrimEnd();
                module = header;
                continue;
            }
            var separator = line.IndexOf(" = ", StringComparison.Ordinal);
            if (separator < 1) throw new InvalidDataException($"Unsupported Mpl declaration: {line}");
            var head = line[..separator].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (head.Length < 2) throw new InvalidDataException($"Expected a function name and arguments: {line}");
            var name = head[0];
            var arguments = string.Join(' ', head.Skip(1));
            var body = line[(separator + 3)..];
            if (!functions.TryAdd(name, $"{arguments} = {body}"))
                throw new InvalidDataException($"Duplicate function declaration {name}.");
        }
        return new MplModule(module ?? throw new InvalidDataException("Missing module declaration."), functions);
    }
}
