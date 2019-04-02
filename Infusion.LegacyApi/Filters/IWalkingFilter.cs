using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.LegacyApi.Filters
{
    public interface IWalkingFilter
    {
        void Enable();
        void Disable();
    }
}
