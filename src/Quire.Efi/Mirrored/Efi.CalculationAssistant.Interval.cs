#nullable disable
#pragma warning disable

using System;
using System.Collections.Generic;

namespace Quire.Efi.CalculationAssistant;

public partial class Interval
{
    public int index_start { get; private set; }

    public int index_end { get; private set; }

    public static List<Interval> splitRange(int startIndex, int numberOfElements, int numberOfIntervals)
    {
        if (numberOfIntervals <= 0)
        {
            throw new ArgumentException("Number of intervals must be greater than 0.");
        }
        if (numberOfElements <= 0)
        {
            throw new ArgumentException("Number of elements must be greater than 0.");
        }
        List<Interval> list = new List<Interval>();
        int num = startIndex + numberOfElements - 1;
        int value = numberOfElements / numberOfIntervals;
        int counter = numberOfElements % numberOfIntervals;
        int otherValue = startIndex;
        for (int i = 0; i < numberOfIntervals; i++)
        {
            int nextValue = otherValue + value - 1;
            if (counter > 0)
            {
                nextValue++;
                counter--;
            }
            if (nextValue > num)
            {
                nextValue = num;
            }
            list.Add(new Interval
            {
                index_start = otherValue,
                index_end = nextValue
            });
            otherValue = nextValue + 1;
            if (otherValue > num)
            {
                break;
            }
        }
        return list;
    }
}
