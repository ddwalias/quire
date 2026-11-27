#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using Quire.Mirror;

namespace Quire.Calculation.Core.Trace2DResults;

public static partial class VectorsListJSonSerializer<T>
{
    private static readonly DataContractJsonSerializer _jSonVectorsListSerializer = new DataContractJsonSerializer(typeof(T[]));

    private static IListResultNode CreateListNode(IList<T> data)
    {
        if (data is IList<double> doubleValues)
        {
            return new DoubleListResultNode(doubleValues);
        }
        if (data is IList<string> data2)
        {
            return new StringListResultNode(data2);
        }
        DebugTools.FailAssertion(typeof(T).Name);
        return null;
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsListJSonSerializer1MechanicalDifferentialTests")]
    public static IList<IListResultNode> Read(Stream stream, int singleVectorLength)
    {
        IList<T> list = Read(stream);
        List<IList<T>> vectors;
        if (singleVectorLength == list.Count)
        {
            vectors = new List<IList<T>> { list };
        }
        else
        {
            vectors = new List<IList<T>>(list.Count / singleVectorLength);
            List<T> currentVector = null;
            int num = 0;
            foreach (T item in list)
            {
                if (num++ % singleVectorLength == 0)
                {
                    currentVector = new List<T>(singleVectorLength);
                    vectors.Add(currentVector);
                }
                currentVector.Add(item);
            }
        }
        return vectors.Select(CreateListNode).ToList();
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsListJSonSerializer1MechanicalDifferentialTests")]
    public static void Write(Stream jsonStream, IList<IListResultNode> nodesListList, int singleVectorLength)
    {
        T[] array = ((nodesListList.Count == 1) ? (((IListResultNode<T>)nodesListList[0]).List as T[]) : null);
        Func<IListResultNode<T>, IList<T>> selector = (IListResultNode<T> vec) => ListResultNode<T>.ExtendList(vec.List, singleVectorLength, vec.NeutralValue);
        array = array ?? nodesListList.Cast<IListResultNode<T>>().SelectMany(selector).ToArray();
        Write(jsonStream, array);
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsListJSonSerializer1MechanicalDifferentialTests")]
    internal static void Write(Stream jsonStream, T[] flatList)
    {
        _jSonVectorsListSerializer.WriteObject(jsonStream, flatList);
    }

    [MirrorVerified("PtcCalculationCoreTrace2DResultsVectorsListJSonSerializer1MechanicalDifferentialTests")]
    internal static IList<T> Read(Stream stream)
    {
        return (IList<T>)_jSonVectorsListSerializer.ReadObject(stream);
    }
}
