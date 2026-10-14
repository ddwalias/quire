using System.Diagnostics;
using System.Reflection;
using System.Security.Cryptography;
using System.Text.Json;

namespace Quire.Differential.Tests;

/// <summary>One directly invoked method pair, bound to a real test and the two compiled artifacts.</summary>
public static class MethodComparison
{
    private static readonly object FileLock = new();
    private static readonly Dictionary<string, string> Hashes = new();

    public static void Compare(string caseId, string contract, MethodBase quire, object? quireTarget, object?[] quireArgs,
        MethodBase original, object? originalTarget, object?[] originalArgs,
        Func<object?, string>? quireShow = null, Func<object?, string>? originalShow = null,
        Func<object, string>? quireState = null, Func<object, string>? originalState = null)
    {
        if (string.IsNullOrWhiteSpace(caseId)) throw new HarnessException("A direct comparison needs a case description");
        var test = new StackTrace().GetFrames().Select(f => f.GetMethod()).OfType<MethodInfo>()
            .FirstOrDefault(m => m.GetCustomAttributesData().Any(a =>
                a.AttributeType.Name is "FactAttribute" or "SkippableFactAttribute" or "TheoryAttribute" or "SkippableTheoryAttribute"))
            ?? throw new HarnessException("Direct comparison must execute inside an identifiable xUnit test");
        if (quire.Module.Assembly.IsDynamic || original.Module.Assembly.IsDynamic)
            throw new HarnessException("A dynamic probe method cannot validate an artifact method");
        ValidateTarget(quire, quireTarget);
        ValidateTarget(original, originalTarget);
        bool stateRequired = !quire.IsStatic && quire is not ConstructorInfo;
        if (stateRequired && (quireTarget == null || originalTarget == null || quireState == null || originalState == null))
            throw new HarnessException("Instance method comparisons require explicit before/after target-state renderers");
        bool Scalar(MethodBase method) => method is MethodInfo info
            && (info.ReturnType.IsPrimitive || info.ReturnType == typeof(string) || info.ReturnType == typeof(void));
        if ((!Scalar(quire) && quireShow == null) || (!Scalar(original) && originalShow == null))
            throw new HarnessException("Object, array and constructor results require explicit state renderers");
        string? targetBefore = stateRequired ? quireState!(quireTarget!) : null;
        Assert.Equal(stateRequired ? originalState!(originalTarget!) : null, targetBefore);
        quireShow ??= Reflect.Text;
        originalShow ??= Reflect.Text;
        var qInputs = quireArgs.Select(quireShow).ToArray();
        var mInputs = originalArgs.Select(originalShow).ToArray();
        Assert.Equal(mInputs, qInputs);
        string q = Reflect.Outcome(() => Invoke(quire, quireTarget, quireArgs), quireShow);
        string m = Reflect.Outcome(() => Invoke(original, originalTarget, originalArgs), originalShow);
        // Mismatches after invocation are recorded with the test and Quire method; mutation counts a kill only when
        // that test also failed, so a swallowed mismatch kills nothing.
        void Same<T>(T expected, T actual)
        {
            try { Assert.Equal(expected, actual); }
            catch (Xunit.Sdk.XunitException) { RecordKill(test, quire); throw; }
        }
        Same(m, q);
        // Mutated ref/out arguments are observable results too.
        var qAfter = quireArgs.Select(quireShow).ToArray();
        var mAfter = originalArgs.Select(originalShow).ToArray();
        Same(mAfter, qAfter);
        string? targetAfter = stateRequired ? quireState!(quireTarget!) : null;
        Same(stateRequired ? originalState!(originalTarget!) : null, targetAfter);
        string? directory = Environment.GetEnvironmentVariable("QUIRE_METHOD_CASES");
        if (directory == null) return;
        string run = Environment.GetEnvironmentVariable("QUIRE_VALIDATION_RUN")
            ?? throw new HarnessException("Method case recording requires a validation run ID");
        string repo = FindRepo();
        string evidenceDir = Path.Combine(repo, "docs", "mirror", "evidence");
        string contractPath = Path.GetFullPath(Path.Combine(evidenceDir, contract + ".md"));
        if (!contractPath.StartsWith(evidenceDir + Path.DirectorySeparatorChar, StringComparison.Ordinal))
            throw new HarnessException("Contract must be a record inside docs/mirror/evidence");
        using var contractStream = File.OpenRead(contractPath);
        string contractHash = Convert.ToHexString(SHA256.HashData(contractStream)).ToLowerInvariant();
        var record = new { schema = 1, run, test = test.DeclaringType!.FullName + "." + test.Name,
            case_id = caseId, contract, contract_sha256 = contractHash, inputs = qInputs, outcome = q, arguments_after = qAfter,
            state_checked = stateRequired, target_before = targetBefore, target_after = targetAfter,
            quire = Identity(quire), original = Identity(original) };
        lock (FileLock)
        {
            Directory.CreateDirectory(directory);
            File.AppendAllText(Path.Combine(directory, $"cases-{Environment.ProcessId}.jsonl"), JsonSerializer.Serialize(record) + "\n");
        }
    }

    /// <summary>Under mutation testing, names the test and the Quire method whose comparison failed.</summary>
    private static void RecordKill(MethodInfo test, MethodBase quire)
    {
        string? path = Environment.GetEnvironmentVariable("QUIRE_MUTATION_KILLS");
        if (path == null) return;
        string line = $"{test.DeclaringType!.FullName}.{test.Name}|{quire.Module.ModuleVersionId}:{quire.MetadataToken}\n";
        lock (FileLock) File.AppendAllText(path, line);
    }

    private static string FindRepo()
    {
        for (var dir = new DirectoryInfo(Path.GetDirectoryName(typeof(MethodComparison).Assembly.Location)!); dir != null; dir = dir.Parent)
            if (File.Exists(Path.Combine(dir.FullName, "Quire.slnx"))) return dir.FullName;
        throw new HarnessException("Cannot locate repository for the contract record");
    }

    private static void ValidateTarget(MethodBase method, object? target)
    {
        if (method.ContainsGenericParameters) throw new HarnessException("Close generic methods before comparing them");
        if (method is MethodInfo { IsVirtual: true, IsFinal: false } virtualMethod && target != null)
        {
            MethodInfo? actual = null;
            if (method.DeclaringType!.IsInterface)
            {
                var map = target.GetType().GetInterfaceMap(method.DeclaringType);
                int index = Array.FindIndex(map.InterfaceMethods, m => m.MetadataToken == method.MetadataToken && m.Module == method.Module);
                if (index >= 0) actual = map.TargetMethods[index];
            }
            else
            {
                var slot = virtualMethod.GetBaseDefinition();
                for (var type = target.GetType(); type != null && actual == null; type = type.BaseType)
                    actual = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                        .FirstOrDefault(m => m.GetBaseDefinition().MetadataToken == slot.MetadataToken && m.GetBaseDefinition().Module == slot.Module);
            }
            if (actual == null || actual.MetadataToken != method.MetadataToken || actual.Module != method.Module)
                throw new HarnessException("Virtual dispatch would invoke another declaration; select the actual override");
        }
    }

    private static object? Invoke(MethodBase method, object? target, object?[] args)
    {
        try { return method is ConstructorInfo ctor && !ctor.IsStatic ? ctor.Invoke(args) : method.Invoke(target, args); }
        catch (TargetInvocationException e) when (e.InnerException != null)
        {
            System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw();
            throw;
        }
        catch (Exception e) { throw new HarnessException($"Direct invocation failed: {method}", e); }
    }

    private static object Identity(MethodBase method)
    {
        string path = method.Module.Assembly.Location;
        string hash;
        lock (FileLock)
        {
            if (!Hashes.TryGetValue(path, out hash!))
            {
                using var stream = File.OpenRead(path);
                Hashes[path] = hash = Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
            }
        }
        return new { assembly = method.Module.Assembly.GetName().Name, sha256 = hash,
            mvid = method.Module.ModuleVersionId.ToString(), token = method.MetadataToken };
    }
}
