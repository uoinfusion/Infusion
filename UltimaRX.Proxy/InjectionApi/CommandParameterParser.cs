using System;
using System.Linq;

namespace Infusion.Proxy.InjectionApi
{
    public class CommandParameterParser
    {
        private readonly char[] separators = {' ' };
        private readonly string parameters;
        private int currentIndex;

        public CommandParameterParser(string parameters)
        {
            this.parameters = parameters;
        }

        public int ParseInt()
        {
            if (currentIndex == parameters.Length)
                throw new CommandInvocationException($"Missing expected number parameter in ${parameters} at position {currentIndex}");

            int nextSeparatorIndex = parameters.IndexOfAny(separators, currentIndex);
            if (nextSeparatorIndex < 0)
                nextSeparatorIndex = parameters.Length;

            string parameterString = parameters.Substring(currentIndex, nextSeparatorIndex - currentIndex);

            try
            {
                int parameterValue = Convert.ToInt32(parameterString);
                currentIndex = nextSeparatorIndex;
                currentIndex = SkipSeparators(nextSeparatorIndex);

                return parameterValue;
            }
            catch (FormatException)
            {
                throw new CommandInvocationException($"Cannot convert '{parameterString}' to a number (int32) at position {currentIndex}.");
            }
        }

        private int SkipSeparators(int index)
        {
            while (index < parameters.Length && separators.Contains(parameters[index]))
                index++;

            return index;
        }
    }
}
