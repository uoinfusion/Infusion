using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace UltimaRX.Proxy.InjectionApi
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
