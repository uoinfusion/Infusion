using System;

namespace Infusion.LegacyApi
{
    public class LegacyException : Exception
    {
        public LegacyException(string message) : base(message)
        {
        }
    }
}