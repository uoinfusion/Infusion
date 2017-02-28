using System;

namespace Infusion.Proxy.LegacyApi
{
    public class CommandInvocationException : Exception
    {
        public CommandInvocationException()
        {
        }

        public CommandInvocationException(string message) : base(message)
        {
        }

        public CommandInvocationException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
