using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infusion.Packets
{
    public enum DragResult
    {
        CannotLift = 0,
        OutOfRange = 1,
        OutOfSight = 2,
        BelongsToAnother = 3,
        AlreadyHoldingSomething = 4,
        None = 253,
        Success = 254,
        Timeout = 255,
    }
}
