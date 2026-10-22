using Quire.utl_mgd;
using Quire.Engine.Misc;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using var ev = new SafeEvent("port-spike");
ev.Set();
if (!ev.WaitOneWithTimeout(100))
    return 1;
ev.Reset();
if (ev.WaitOneWithTimeout(0))
    return 2;

Console.WriteLine("managed consumer loaded the C++/CLI Utl spike and exercised its Win32 event");

var interruptControl = new InterruptControl();
InterruptControl.ThreadInitialize(interruptControl);
InterruptControl.ProhibitInterruption();
InterruptControl.AllowInterruption();
InterruptControl.ThreadInitialize(null);

Console.WriteLine("managed consumer exercised the C++/CLI interrupt control");

using var namedMutex = new SafeMutex($"UtlPortSpikeTest-{Guid.NewGuid():N}", 0);
namedMutex.WaitOne();
namedMutex.ReleaseMutex();

Console.WriteLine("managed consumer exercised the C++/CLI named mutex");

var names = new SortedStringSet(new[] { "Zulu", "Alpha" });
if (names.Count != 2 || !names.Contains("Alpha"))
    return 3;
var namesCopy = new SortedStringSet(names);
if (!names.IsSubsetOf(namesCopy) || !names.SetEquals(namesCopy))
    return 4;
var copiedNames = new string[names.Count];
names.CopyTo(copiedNames);
if (copiedNames[0] != "Alpha")
    return 5;
names.Add("Beta");
if (!names.GetEnumerator().MoveNext())
    return 6;

Console.WriteLine("managed consumer exercised the C++/CLI sorted string set");

if (Str.RemoveWhite(" a \t b ") != "ab")
    return 7;

using var finished = new ManualResetEventSlim();
ThreadingUtils.Spawn(new ConsumerFunction(finished));
if (!finished.Wait(5000))
    return 8;

Console.WriteLine("managed consumer exercised the C++/CLI string and thread helpers");

var readWriteLock = new SafeReadWriteLock();
readWriteLock.WaitReadLock();
readWriteLock.ReleaseReadLock();
readWriteLock.WaitWriteLock();
readWriteLock.ReleaseWriteLock();

Console.WriteLine("managed consumer exercised the C++/CLI read/write lock");

var labels = LabelTrie.InternLabels("scope{ beta, alpha }");
if (labels.ToString() != "{alpha,beta}" || !labels.Unifies(LabelTrie.InternLabels(new[] { "alpha", "beta" }, false)))
    return 9;
if (!LabelTrie.WithWild(labels, true).Wild())
    return 10;

Console.WriteLine("managed consumer exercised the C++/CLI label trie");
if (LabelUtils.SplitName("scope{alpha}") != "scope" ||
    LabelUtils.Canonicalize("scope{beta,alpha}") != "scope{alpha,beta}")
    return 11;

Console.WriteLine("managed consumer exercised the C++/CLI label utilities");

if (StringUtils.StringQuote("a\"b") != "\"a\\\"b\"")
    return 12;
var pair = new Pair<int, string>(7, "seven");
var triple = new Triple<int, string, bool>(3, "three", true);
if (pair.First != 7 || pair.Second != "seven" || triple.Third != true)
    return 13;
var values = new HashSet<string> { "one", "two" };
var valuesArray = HashSetUtils.HashSetToArray(ref values);
if (valuesArray.Length != 2 || !valuesArray.Contains("one") || !valuesArray.Contains("two"))
    return 14;

Console.WriteLine("managed consumer exercised the ported StringUtils, Pair, Triple, and HashSetUtils types");

IDictionary initial = new Hashtable { ["scope{beta,alpha}"] = 4 };
var labeled = new LabeledDictionary(initial);
if (labeled.Count != 1 || (int)labeled["scope{alpha,beta}"]! != 4 || labeled.IsReadOnly || labeled.IsFixedSize)
    return 15;
labeled["extra{beta,alpha}"] = 7;
if (!labeled.Contains("extra{alpha,beta}") || (int)labeled["extra{alpha,beta}"]! != 7)
    return 16;
var labeledCopy = labeled.Copy();
if (labeledCopy.Count != 2 || labeledCopy["scope{alpha,beta}"] is null)
    return 17;

Console.WriteLine("managed consumer exercised the transplanted LabeledDictionary methods");

var monitor = new object();
SafeMonitor.Enter(monitor);
if (!SafeMonitor.TryEnter(monitor))
    return 18;
SafeMonitor.Exit(monitor);
SafeMonitor.Exit(monitor);

Console.WriteLine("managed consumer exercised the unmanaged critical-section SafeMonitor");
return 0;

sealed class ConsumerFunction : FunctionBase
{
    private readonly ManualResetEventSlim _finished;

    public ConsumerFunction(ManualResetEventSlim finished) => _finished = finished;

    public override object? Invoke(object?[]? args)
    {
        _finished.Set();
        return null;
    }
}
