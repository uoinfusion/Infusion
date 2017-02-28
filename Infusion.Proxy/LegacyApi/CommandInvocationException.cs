using System;

namespace Infusion.Proxy.InjectionApi
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
