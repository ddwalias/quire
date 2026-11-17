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
        int num2 = numberOfElements / numberOfIntervals;
        int num3 = numberOfElements % numberOfIntervals;
        int num4 = startIndex;
        for (int i = 0; i < numberOfIntervals; i++)
        {
            int num5 = num4 + num2 - 1;
            if (num3 > 0)
            {
                num5++;
                num3--;
            }
            if (num5 > num)
            {
                num5 = num;
            }
            list.Add(new Interval
            {
                index_start = num4,
                index_end = num5
            });
            num4 = num5 + 1;
            if (num4 > num)
            {
                break;
            }
        }
        return list;
    }
}
