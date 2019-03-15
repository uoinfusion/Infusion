using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi
{
    public sealed class Configuration
    {
        public void Register<T>(string name, Expression<Func<T,T>> expression)
        {
        }
    }
}
