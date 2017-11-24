using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Utilities
{
    internal class CircuitBreaker
    {
        private readonly Action<Exception> onException;
        public bool Open { get; private set; }

        public CircuitBreaker(Action<Exception> onException)
        {
            this.onException = onException;
        }

        public void Protect(Action protectedAction)
        {
            if (Open)
                return;

            try
            {
                protectedAction();
            }
            catch (Exception e)
            {
                onException(e);
                Open = true;
            }
        }
    }
}
