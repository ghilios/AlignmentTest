using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlignmentTest.Solver;

public static class MathEx {
    public static (double, double) MedianMAD(this IEnumerable<double> values) {
        var valuesArray = values.ToArray();
        if (valuesArray.Length == 0) {
            return (double.NaN, double.NaN);
        }
        Array.Sort(valuesArray);

        var median = valuesArray.Length % 2 == 0
          ? (valuesArray[valuesArray.Length / 2 - 1] + valuesArray[valuesArray.Length / 2]) / 2.0
          : valuesArray[valuesArray.Length / 2];

        for (int i = 0; i < valuesArray.Length; ++i) {
            valuesArray[i] = Math.Abs(valuesArray[i] - median);
        }
        Array.Sort(valuesArray);

        var mad = 1.483 * valuesArray.Length % 2 == 0
          ? (valuesArray[valuesArray.Length / 2 - 1] + valuesArray[valuesArray.Length / 2]) / 2.0
          : valuesArray[valuesArray.Length / 2];
        return (median, mad);
    }

    public static (double, double) MeanVar(this IEnumerable<double> values) {
        var mean = values.Average();
        var count = values.Count();
        var variance = values.Sum(s => (s - mean) * (s - mean)) / (count - 1);
        return (mean, variance);
    }
}