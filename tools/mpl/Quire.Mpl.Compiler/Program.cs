using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using MplAssemblyBuilder = System.Reflection.Emit.Quire.Mpl.AssemblyBuilder;

if (args.Length == 5 && args[0] == "--interface")
{
    CompileInterfaceModule(Path.GetFullPath(args[1]), Path.GetFullPath(args[2]),
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
        CompileHashtable(program, outputPath);
        break;
    case "McdUnitSystemUS":
        CompileUnitSystemUs(program, outputPath, install);
        break;
    default:
        throw new NotSupportedException($"Prototype compiler does not support module {program.Module}.");
}
return 0;

static void CompileInterfaceModule(string sourcePath, string outputPath, string install, string project)
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
    var outputType = project + ".values";

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
    Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
    assembly.Save(outputPath);
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
        && !line.StartsWith("|", StringComparison.Ordinal)
        && !line.Contains(" :: ", StringComparison.Ordinal)))
        throw new InvalidDataException("Interface-only sources support imports, data declarations, and signatures only.");
    if (declarations.Any(line => line.Contains(" = ", StringComparison.Ordinal)
        && !line.StartsWith("data ", StringComparison.Ordinal)))
        throw new InvalidDataException("Interface-only modules cannot contain value definitions.");
    if (!declarations.Any(line => line.Contains(" :: ", StringComparison.Ordinal)))
        throw new InvalidDataException("Interface-only source must declare at least one signature.");
    return match.Groups[1].Value;
}

static void CompileHashtable(MplModule program, string output)
{
    Require(program, "Update", "() = ()");
    Require(program, "mlookupInv", "table key = hlookup (Map.Field2 table) key");
    var assembly = CreateAssembly("Quire.Mpl.Modules");
    var module = assembly.DefineDynamicModule("Quire.Mpl.Modules.dll");
    var mapType = module.DefineType("Quire.Hashtable.Map", TypeAttributes.Public | TypeAttributes.Class);
    var field = mapType.DefineField("Field2", typeof(object), FieldAttributes.Public);
    AddMirrorOf(mapType, "Hashtable.Map, Hashtable");
    var values = module.DefineType("Quire.Hashtable.values", TypeAttributes.Public | TypeAttributes.Class);
    AddMirrorOf(values, "Hashtable.values, Hashtable");
    var update = values.DefineMethod("Update", MethodAttributes.Public | MethodAttributes.Static,
        typeof(void), Type.EmptyTypes);
    update.GetILGenerator().Emit(OpCodes.Ret);
    var hlookup = values.DefineMethod("hlookup", MethodAttributes.Public | MethodAttributes.Static,
        typeof(object), [typeof(object), typeof(object)]);
    hlookup.GetILGenerator().Emit(OpCodes.Ldnull);
    hlookup.GetILGenerator().Emit(OpCodes.Ret);
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
    il.Emit(OpCodes.Ldfld, field);
    il.Emit(OpCodes.Ldarg_1);
    il.Emit(OpCodes.Tailcall);
    il.Emit(OpCodes.Call, hlookup);
    il.Emit(OpCodes.Ret);
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

internal sealed record MplModule(string Module, Dictionary<string, string> Functions);

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
