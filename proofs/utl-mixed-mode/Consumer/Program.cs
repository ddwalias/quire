using UtlMixedModeProof;

var result = NativeBridge.Add(19, 23);
Console.WriteLine($"managed consumer received {result}");
return result == 42 ? 0 : 1;
