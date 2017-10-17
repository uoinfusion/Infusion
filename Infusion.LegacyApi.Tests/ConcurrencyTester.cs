using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Tests
{

    public static class ConcurrencyTester
    {
#if CONCURRENCY_TESTS
        private const int ConcurrencyIterations = 10000;
#else
        private const int ConcurrencyIterations = 1;
#endif

        public static void Run(Action testedAction)
        {
            for (int i = 0; i < ConcurrencyIterations; i++)
            {
                testedAction();
            }
        }
    }
}
