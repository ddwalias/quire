#nullable disable
using System;
using System.Collections.Generic;
using System.Globalization;
using Quire.Calculation.Core;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class NodeHandleMapper
{
    private List<Pair<ICalculatable, uint>> _nodeHandleList;

    private const string HandlePrefix = ":0x";

    public NodeHandleMapper()
    {
        Clear();
    }

    public void Clear()
    {
        _nodeHandleList = new List<Pair<ICalculatable, uint>>();
    }

    public string GetReplacedRl(ICalculatable equationControl)
    {
        return GetReplacedRl(equationControl, equationControl.Rl);
    }

    public static Pair<string[], uint[]> GetNodesAndHandles(string rl)
    {
        if (DebugTools.AssertNotNull(rl, null).Failed)
        {
            return new Pair<string[], uint[]>(new string[0], new uint[0]);
        }
        string[] array = rl.Split(new string[1] { ":0x" }, StringSplitOptions.None);
        uint[] array2 = new uint[array.Length - 1];
        for (int i = 0; i < array2.Length; i++)
        {
            int num = i + 1;
            array2[i] = uint.Parse(array[num].Substring(0, 8), NumberStyles.HexNumber);
            array[num] = array[num].Substring(8);
        }
        return new Pair<string[], uint[]>(array, array2);
    }

    public string GetReplacedRl(ICalculatable equationControl, string rl)
    {
        Pair<string[], uint[]> nodesAndHandles = GetNodesAndHandles(rl);
        string[] first = nodesAndHandles.First;
        uint[] second = nodesAndHandles.Second;
        for (int i = 0; i < second.Length; i++)
        {
            first[i] += string.Format("{0}{1:x8}", ":0x", Map(equationControl, second[i]));
        }
        return string.Join(string.Empty, first);
    }

    public uint Map(ICalculatable equationControl, uint originalNodeHandle)
    {
        _nodeHandleList.Add(new Pair<ICalculatable, uint>(equationControl, originalNodeHandle));
        return (uint)_nodeHandleList.Count;
    }

    public uint GetMappedNodeNandle(uint generatedNodeHandle)
    {
        return _nodeHandleList[GetIndexFromHandle(generatedNodeHandle)].Second;
    }

    public ICalculatable GetMappedEquation(uint generatedNodeHandle)
    {
        int indexFromHandle = GetIndexFromHandle(generatedNodeHandle);
        if (indexFromHandle >= 0 && indexFromHandle < _nodeHandleList.Count)
        {
            return _nodeHandleList[indexFromHandle].First;
        }
        return null;
    }

    private static int GetIndexFromHandle(uint generatedNodeHandle)
    {
        return (int)(generatedNodeHandle - 1);
    }
}
