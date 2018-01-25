using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion
{
    internal class Coordinate
    {
        internal static void CheckCoordValue(string argumentName, int value, int minValue, int maxValue)
        {
            if (value < minValue || value > maxValue)
                throw new ArgumentOutOfRangeException(argumentName, $"cannot be less than {minValue} and more than {maxValue}, current value is {value}");
        }
    }
}
